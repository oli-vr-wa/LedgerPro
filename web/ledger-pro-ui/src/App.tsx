import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { DashboardLayout } from './layouts/DashboardLayout';

// Mock components for pages
const DashboardPage = () => <div>Dashboard Content</div>;
const BankSourcesPage = () => <div>Bank Sources Content</div>;
const SettingsPage = () => <div>Settings Content</div>;

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<DashboardLayout />}>
          <Route index element={<DashboardPage />} />
          <Route path="banksources" element={<BankSourcesPage />} />
          <Route path="settings" element={<SettingsPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;