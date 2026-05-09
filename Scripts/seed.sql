-- 1. Create a Bank Source (e.g., NAB)
INSERT INTO BankSources (Id, BankName, BankType, AccountName, AccountNumber) 
VALUES ('6f81b16c-2f3b-4c5d-8e9f-1a2b3c4d5e6f', 'National Australian Bank (NAB)', 1, 'NAB Personal', '123456789'); -- 1 for NAB

-- 2. Create a General Ledger Account for Grocery Expenses
INSERT INTO GeneralLedgerAccounts (Id, Name, AccountType, Description)
VALUES ('5000', 'Grocery Expenses', 4, 'Expenses for grocery shopping'); -- 4 for Expense  

-- 3. Create a Mapping Rule
INSERT INTO BankTransactionMappings (Id, SearchTerm, MatchStrategy, TargetGeneralLedgerAccountId, DescriptionTemplate, ReferenceTemplate, Priority)
VALUES ('7f81b16c-2f3b-4c5d-8e9f-1a2b3c4d5e6f', 'WOOLWORTHS', 0, '5000', 'Grocery Expense - Woolworths', 'GROCER-WOOLIES', 1);