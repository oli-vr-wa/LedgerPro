import api from './api';
import type { GeneralLedgerAccount } from '../types/generalLedgerAccount';

export const generalLedgerAccountService = {
    getAll: () => api.get<GeneralLedgerAccount[]>('/ledger/accounts'),
    create: (data: Omit<GeneralLedgerAccount, 'id'>) => api.post<GeneralLedgerAccount>('/ledger/account', data)
};