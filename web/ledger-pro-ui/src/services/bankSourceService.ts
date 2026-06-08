import api from './api';
import type { BankSource } from '../types/bank-source.types';

export const bankSourceService = {
    getAll: () => api.get<BankSource[]>('/banksources'),
    create: (data: Omit<BankSource, 'id' | 'generalLedgerAccountId'>) => api.post<BankSource>('/banksource', data),
    update: (id: string, data: Omit<BankSource, 'id' | 'generalLedgerAccountId'>) => api.put<BankSource>(`/banksource/${id}`, data),
    delete: (id: string) => api.delete(`/banksource/${id}`)
};