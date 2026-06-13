import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { DashboardLayout } from './layouts/DashboardLayout';
import { BankSources } from './pages/BankSources';
import { GeneralLedgerAccounts } from './pages/GeneralLedgerAccounts';
import { BankTransactionMappings } from './pages/BankTransactionMappings';
import { BankTransactionsLayout } from './layouts/BankTransactionsLayout';
import { BankAccountSelection } from './pages/BankAccountSelection';
import { BankSourceTransactionsLayout } from './layouts/BankSourceTransactionsLayout';
import { BankTransactionsUpload } from './pages/BankTransactionsUpload';
import { BankTransactionsYearSelection } from './pages/BankTransactionsYearSelection';

// Mock components for pages
const DashboardPage = () => <div>Dashboard Content</div>;
const SettingsPage = () => <div>Settings Content</div>;

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
          <Route path="settings" element={<SettingsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;