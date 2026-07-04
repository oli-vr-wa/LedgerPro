import api from "./api";

export const generalLedgerService = {
    getGeneralLedgerFinancialYearsOverview: () => api.get(`/ledger/financial-years-overview`),
};