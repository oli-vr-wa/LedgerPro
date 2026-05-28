import api from './api';
import type { BankSource } from '../types/bankSource';

export const bankSourceService = {
    getAll: () => api.get<BankSource[]>('/banksources'),
    create: (data: Omit<BankSource, 'id'>) => api.post<BankSource>('/banksources', data)
};