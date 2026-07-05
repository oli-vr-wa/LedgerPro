import { BrowserRouter, Routes, Route, useParams } from 'react-router-dom';
import { DashboardLayout } from './layouts/DashboardLayout';
import { BankSources } from './pages/bank-sources/BankSources';
import { GeneralLedgerAccounts } from './pages/general-ledger-accounts/GeneralLedgerAccounts';
import { BankTransactionMappings } from './pages/bank-transaction-mappings/BankTransactionMappings';
import { BankTransactionsLayout } from './layouts/BankTransactionsLayout';
import { BankAccountSelection } from './pages/bank-account-selection/BankAccountSelection';
import { BankSourceTransactionsLayout } from './layouts/BankSourceTransactionsLayout';
import { BankTransactionsUpload } from './pages/bank-transactions-upload/BankTransactionsUpload';
import { BankTransactionsYearSelection } from './pages/BankTransactionsYearSelection';
import { GeneralLedgerYearsOverview } from './pages/general-ledger-years-overview/GeneralLedgerYearsOverview';
import { GeneralLedgerLayout } from './layouts/GeneralLedgerLayout';
import { GeneralLedgerAccountsYearTotals } from './pages/general-ledger-accounts-year-totals/GeneralLedgerAccountsYearTotals';
import { GeneralLedgerItemEntries } from './pages/general-ledger-item-entries/GeneralLedgerItemEntries';

// Mock components for pages
const DashboardPage = () => <div>Dashboard Content</div>;
const SettingsPage = () => <div>Settings Content</div>;

// Wrapper components to access useParams
const GeneralLedgerAccountsYearTotalsWrapper = () => {
  const { yearEnding } = useParams();
  return <GeneralLedgerAccountsYearTotals financialYear={Number(yearEnding)} />;
};

const GeneralLedgerItemEntriesWrapper = () => {
  const { financialYear, accountId } = useParams();
  return <GeneralLedgerItemEntries financialYear={Number(financialYear)} accountId={Number(accountId)} />;
};

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<DashboardLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="banksources" element={<BankSources />} />
          <Route path="generalLedgerAccounts" element={<GeneralLedgerAccounts />} />
          <Route path="bankTransactionMappings" element={<BankTransactionMappings />} />
          <Route path="transactions" element={<BankTransactionsLayout />}>
            <Route index element={<BankAccountSelection />} />
            <Route path=":bankSourceId/*" element={<BankSourceTransactionsLayout />}>
              <Route index element={<BankTransactionsYearSelection />} />
              <Route path="upload" element={<BankTransactionsUpload />} />
            </Route>
          </Route>
          <Route path="generalLedger" element={<GeneralLedgerLayout />}>
            <Route index element={<GeneralLedgerYearsOverview />} />
            <Route path=":yearEnding" element={<GeneralLedgerAccountsYearTotalsWrapper />} />
            <Route path=":financialYear/account/:accountId" element={<GeneralLedgerItemEntriesWrapper />} />
          </Route>
          <Route path="settings" element={<SettingsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;