import { Outlet } from "react-router-dom";

export function GeneralLedgerLayout() {
    return (
    <div>
        <h1 className="text-2xl font-bold mb-6">General Ledger</h1>
        <Outlet />
    </div>
    );
}