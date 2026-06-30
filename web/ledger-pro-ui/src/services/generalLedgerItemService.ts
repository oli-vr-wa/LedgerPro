import api from './api';
import type { GeneralLedgerItemTransaction } from '../types/general-ledger-item-transaction.types';

export const generalLedgerItemService = {
    getAllForBankTransaction: (bankTransactionId: string) => api.get<GeneralLedgerItemTransaction[]>(`/ledger/items/bank-transaction/${bankTransactionId}`),
};