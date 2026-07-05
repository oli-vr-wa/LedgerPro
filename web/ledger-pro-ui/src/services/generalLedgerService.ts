import api from "./api";

export const generalLedgerService = {
    getGeneralLedgerFinancialYearsOverview: () => api.get(`/ledger/financial-years-overview`),
    getGeneralLedgerAccountsForFinancialYear: (financialYear: number) => api.get(`/ledger/accounts/financial-year/${financialYear}`),
    getGeneralLedgerItemsForFinancialYearAndAccount: (financialYear: number, accountId: number) => api.get(`/ledger/items/financial-year/${financialYear}/account/${accountId}`)
};