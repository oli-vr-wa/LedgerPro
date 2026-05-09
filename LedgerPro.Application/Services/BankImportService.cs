using LedgerPro.Application.Interfaces;
using LedgerPro.Application.DTOs;
using LedgerPro.Core.Common;
using LedgerPro.Core.Interfaces;
using LedgerPro.Core.Entities;

namespace LedgerPro.Application.Services
{
    /// <summary>
    /// Service responsible for importing bank statements and storing the transactions in the database.
    /// </summary>
    public class BankImportService : IBankImportService
    {
        private readonly IBankStatementParser _bankStatementParser;
        
        private readonly IBankRepository _bankRepository;

        private readonly ITransactionMatchService _transactionMatchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankImportService"/> class.
        /// </summary>
        /// <param name="bankStatementParser">The bank statement parser.</param>
        /// <param name="transactionMatchService">The transaction match service.</param>
        /// <param name="bankRepository">The bank repository.</param>
        public BankImportService(IBankStatementParser bankStatementParser, ITransactionMatchService transactionMatchService, IBankRepository bankRepository)
        {
            _bankStatementParser = bankStatementParser;
            _bankRepository = bankRepository;
            _transactionMatchService = transactionMatchService;
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
            var bankSource = await _bankRepository.GetBankSourceByIdAsync(request.BankSourceId);

            if (bankSource == null)            
                return Result<int>.Failure($"Bank source with ID {request.BankSourceId} not found.");
            
            // Parse the bank statement
            var parseResult = _bankStatementParser.Parse(request.FileStream, request.BankSourceId, bankSource.BankType);

            if (parseResult.IsFailure)            
                return Result<int>.Failure(parseResult.Error);            
            
            var transactions = parseResult.Value!;

            // Get all the BankTransactionMappings
            var mappings = await _bankRepository.GetBankTransactionMappingsAsync();

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

            // Save the transactions to the database
            await _bankRepository.AddTransactionsAsync(transactions);
            await _bankRepository.AddGLItemsAsync(ledgerItemsToAdd);
            await _bankRepository.SaveChangesAsync();

            return Result<int>.Success(transactions.Count());
        }
    }
}