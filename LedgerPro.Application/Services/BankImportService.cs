using LedgerPro.Application.Interfaces;
using LedgerPro.Application.DTOs;
using LedgerPro.Core.Common;
using LedgerPro.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace LedgerPro.Application.Services
{
    /// <summary>
    /// Service responsible for importing bank statements and storing the transactions in the database.
    /// </summary>
    public class BankImportService : IBankImportService
    {
        private readonly IBankStatementParser _bankStatementParser;
        
        private readonly ILedgerDbContext _dbContext;

        private readonly ITransactionMatchService _transactionMatchService;

        /// <summary>
        /// Initializes a new instance of the <see cref="BankImportService"/> class.
        /// </summary>
        /// <param name="bankStatementParser">The bank statement parser.</param>
        /// <param name="transactionMatchService">The transaction match service.</param>
        /// <param name="dbContext">The database context.</param>
        public BankImportService(IBankStatementParser bankStatementParser, ITransactionMatchService transactionMatchService, ILedgerDbContext dbContext)
        {
            _bankStatementParser = bankStatementParser;
            _dbContext = dbContext;
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
            var bankSource = await _dbContext.BankSources.FindAsync(request.BankSourceId);

            if (bankSource == null)            
                return Result<int>.Failure($"Bank source with ID {request.BankSourceId} not found.");
            
            // Parse the bank statement
            var parseResult = _bankStatementParser.Parse(request.FileStream, request.BankSourceId, bankSource.BankType);

            if (parseResult.IsFailure)            
                return Result<int>.Failure(parseResult.Error);            
            
            var transactions = parseResult.Value!;

            // Get all the BankTransactionMappings
            var mappings = await _dbContext.BankTransactionMappings.ToListAsync();

            // Attempt to match each transaction and create corresponding GeneralLedgerItems
            foreach (var transaction in transactions)
            {
                var glItem = _transactionMatchService.MatchAndCreateLedgerItem(transaction, mappings);

                if (glItem != null)
                {
                    _dbContext.GeneralLedgerItems.Add(glItem);
                }
            }

            // Save the transactions to the database
            _dbContext.BankTransactions.AddRange(transactions);
            var count = await _dbContext.SaveChangesAsync();

            return Result<int>.Success(transactions.Count());
        }
    }
}