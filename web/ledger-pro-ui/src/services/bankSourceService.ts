import api from './api';
import type { BankSource } from '../types/bank-source.types';
import type { BankSourceTransactionsRow } from '@/types/bank-source-transactions-row.types';

export const bankSourceService = {
    getAll: () => api.get<BankSource[]>('/banksources'),
    getBankSourceById: (id: string) => api.get<BankSource>(`/banksource/${id}`),
    create: (data: Omit<BankSource, 'id' | 'generalLedgerAccountId'>) => api.post<BankSource>('/banksource', data),
    update: (id: string, data: Omit<BankSource, 'id' | 'generalLedgerAccountId'>) => api.put<BankSource>(`/banksource/${id}`, data),
    delete: (id: string) => api.delete(`/banksource/${id}`),
    getTransactionsOverview: () => api.get<BankSourceTransactionsRow[]>('/banksources/transactions-overview')
};