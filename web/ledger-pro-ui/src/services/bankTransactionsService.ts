import api from './api';
import type { BankTransaction } from '../types/bank-transaction.types';
import type { BankTransactionsYearRow } from '@/types/bank-transactions-year-row.types';

export const bankTransactionsService = {
    getTransactionsByBankSourceId: (bankSourceId: string) => api.get<BankTransaction[]>(`/banktransactions/${bankSourceId}/transactions`),
    getTransactionsFinancialYearsOverview: (bankSourceId: string) => api.get<BankTransactionsYearRow[]>(`/banktransactions/${bankSourceId}/transactions/financial-years-overview`)
};