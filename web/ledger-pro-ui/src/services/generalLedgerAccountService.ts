import api from './api';
import type { GeneralLedgerAccount } from '../types/general-ledger-account.types';

export const generalLedgerAccountService = {
    getAll: () => api.get<GeneralLedgerAccount[]>('/ledger/accounts'),
    create: (data: Omit<GeneralLedgerAccount, 'id'>) => api.post<GeneralLedgerAccount>('/ledger/account', data),
    update: (id: number, data: Omit<GeneralLedgerAccount, 'id'>) => api.put<GeneralLedgerAccount>(`/ledger/account/${id}`, data),
    delete: (id: number) => api.delete(`/ledger/account/${id}`)
};