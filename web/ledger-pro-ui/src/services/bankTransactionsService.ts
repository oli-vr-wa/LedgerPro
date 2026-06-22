import api from './api';
import type { BankTransaction } from '../types/bank-transaction.types';
import type { BankTransactionsYearRow } from '@/types/bank-transactions-year-row.types';
import type { BankTransactionCategorize } from '@/types/bank-transaction-categorize.types';

export const bankTransactionsService = {
    getTransactionsByBankSourceId: (bankSourceId: string) => api.get<BankTransaction[]>(`/banktransactions/${bankSourceId}/transactions`),
    getTransactionsFinancialYearsOverview: (bankSourceId: string) => api.get<BankTransactionsYearRow[]>(`/banktransactions/${bankSourceId}/transactions/financial-years-overview`),
    getTransactionsByBankSourceIdAndYear: (bankSourceId: string, year: number) => api.get<BankTransaction[]>(`/banktransactions/${bankSourceId}/transactions/${year}`),
    categorizeTransaction: (transactionId: string, data: BankTransactionCategorize) => api.put(`/banktransactions/transactions/${transactionId}/categorize`, data),
};