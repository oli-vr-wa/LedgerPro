import api from './api';
import type { BankTransaction } from '../types/bank-transaction.types';
import type { BankTransactionsYearRow } from '@/types/bank-transactions-year-row.types';
import type { BankTransactionCategorize } from '@/types/bank-transaction-categorize.types';

export const bankTransactionsService = {
    getTransactionsByBankSourceId: (bankSourceId: string) => api.get<BankTransaction[]>(`/banktransactions/${bankSourceId}/transactions`),
    getTransactionsFinancialYearsOverview: (bankSourceId: string) => api.get<BankTransactionsYearRow[]>(`/banktransactions/${bankSourceId}/transactions/financial-years-overview`),
    getTransactionsByBankSourceIdAndYear: (bankSourceId: string, year: number) => api.get<BankTransaction[]>(`/banktransactions/${bankSourceId}/transactions/${year}`),
    categorizeTransaction: (categorizeDto: BankTransactionCategorize) => api.post(`/banktransactions/categorize`, categorizeDto),
    uncategorizeTransaction: (bankTransactionId: string) => api.post(`/banktransactions/uncategorize/${bankTransactionId}`),
    reconcileTransaction: (bankTransactionId: string) => api.post(`/banktransactions/reconcile/${bankTransactionId}`),
    unreconcileTransaction: (bankTransactionId: string) => api.post(`/banktransactions/unreconcile/${bankTransactionId}`)
};