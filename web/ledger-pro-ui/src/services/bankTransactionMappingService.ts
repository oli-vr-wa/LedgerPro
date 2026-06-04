import api from './api';
import type { BankTransactionMapping } from '../types/bank-transaction-mapping.types';

export const bankTransactionMappingService = {
    getAll: () => api.get<BankTransactionMapping[]>('/banktransactions/mappings'),
    create: (data: Omit<BankTransactionMapping, 'id'>) => api.post<BankTransactionMapping>('/banktransactions/mappings', data),
    //update: (id: string, data: Omit<BankTransactionMapping, 'id'>) => api.put<BankTransactionMapping>(`/banktransactions/mapping/${id}`, data),
    //delete: (id: string) => api.delete(`/banktransactions/mapping/${id}`)
};