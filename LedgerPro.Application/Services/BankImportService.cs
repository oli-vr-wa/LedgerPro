using LedgerPro.Application.Interfaces;
using LedgerPro.Application.DTOs;
using LedgerPro.Core.Common;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Services;

/// <summary>
/// Service responsible for importing bank statements and storing the transactions in the database.
/// </summary>
public class BankImportService : IBankImportService
{
    private readonly IBankStatementParser _bankStatementParser;        
    private readonly IBankSourceRepository _bankSourceRepository;
    private readonly IBankTransactionRepository _bankTransactionRepository;
    private readonly ITransactionMatchService _transactionMatchService;
    private readonly IGeneralLedgerRepository _generalLedgerRepository;
    private readonly IFileHasher _fileHasher;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Initializes a new instance of the <see cref="BankImportService"/> class.
    /// </summary>
    /// <param name="bankStatementParser">The bank statement parser.</param>
    /// <param name="transactionMatchService">The transaction match service.</param>
    /// <param name="bankSourceRepository">The bank source repository.</param>
    /// <param name="fileHasher">The file hasher.</param>
    public BankImportService(
        IBankStatementParser bankStatementParser, 
        ITransactionMatchService transactionMatchService, 
        IBankSourceRepository bankSourceRepository, 
        IBankTransactionRepository bankTransactionRepository,
        IGeneralLedgerRepository generalLedgerRepository,
        IFileHasher fileHasher,
        IUnitOfWork unitOfWork)
    {
        _bankStatementParser = bankStatementParser;
        _bankSourceRepository = bankSourceRepository;
        _transactionMatchService = transactionMatchService;
        _bankTransactionRepository = bankTransactionRepository;
        _generalLedgerRepository = generalLedgerRepository;
        _fileHasher = fileHasher;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Imports a bank statement from the given request storing the transactions in the database.
    /// If a bank transaction matches a mapping rule, a corresponding GeneralLedgerItem will be created and the transaction will be marked as Categorized.
    /// </summary>
    /// <param name="request">The upload bank statement request.</param>
    /// <returns>The result containing the number of imported transactions.</returns>
    public async Task<Result<int>> ImportBankStatementAsync(UploadBankStatementRequest request)
    {
        // Find the bank source
        var bankSource = await _bankSourceRepository.GetBankSourceByIdAsync(request.BankSourceId);

        if (bankSource == null)                                       
            return Result<int>.Failure($"Bank source with ID {request.BankSourceId} not found.");                        

        // Calculate the file hash
        var fileHash = await _fileHasher.CalculateHashAsync(request.FileStream);

        // Parse the bank statement
        var parseResult = _bankStatementParser.Parse(request.FileStream, request.BankSourceId, bankSource.BankType);

        if (parseResult.IsFailure)            
            return Result<int>.Failure(parseResult.Error);            
        
        var transactions = parseResult.Value!;

        // Create Statement Import record
        var statementImport = new StatementImport
        {
            BankSourceId = request.BankSourceId,
            ImportDate = DateTime.Now,
            FileHash = fileHash,
            FileName = request.FileName,
            TransactionCount = transactions.Count(),
            Transactions = transactions.ToList() // Establish the relationship between the StatementImport and BankTransactions
        };            

        // Get all the BankTransactionMappings
        var mappings = await _bankTransactionRepository.GetBankTransactionMappingsAsync();

        // Ledger Items to be added to the database
        var ledgerItemsToAdd = new List<GeneralLedgerItem>();

        // Attempt to match each transaction and create corresponding GeneralLedgerItems
        foreach (var transaction in transactions)
        {
            var glItem = _transactionMatchService.MatchAndCreateLedgerItem(transaction, mappings);

            if (glItem != null)
            {
                ledgerItemsToAdd.Add(glItem);                    
            }
        }

        // Save the transactions & general ledger items to the database
        await _bankTransactionRepository.AddStatementImportAsync(statementImport);
        await _bankTransactionRepository.AddTransactionsAsync(transactions);
        await _generalLedgerRepository.AddGeneralLedgerItemsAsync(ledgerItemsToAdd);
        await _unitOfWork.CommitAsync();

        return Result<int>.Success(transactions.Count());
    }
}
