import api from './api';
import type { BankSource } from '../types/bank-source.types';
import type { BankSourceTransactionsRow } from '@/types/bank-source-transactions-row.types';
import type { StatementImportRow } from '@/types/statement-import-row.type';

export const bankSourceService = {
    getAll: () => api.get<BankSource[]>('/banksources'),
    getBankSourceById: (id: string) => api.get<BankSource>(`/banksource/${id}`),
    create: (data: Omit<BankSource, 'id' | 'generalLedgerAccountId'>) => api.post<BankSource>('/banksource', data),
    update: (id: string, data: Omit<BankSource, 'id' | 'generalLedgerAccountId'>) => api.put<BankSource>(`/banksource/${id}`, data),
    delete: (id: string) => api.delete(`/banksource/${id}`),
    getTransactionsOverview: () => api.get<BankSourceTransactionsRow[]>('/banksources/transactions-overview'),
    getStatementImports: (id: string) => api.get<StatementImportRow[]>(`/banksources/${id}/statement-imports`),
    importStatement: (id: string, file: File) => { 
        const formData = new FormData();
        formData.append('file', file);
        return api.post(`/banksources/${id}/import-statements`, formData, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        });
    }
};