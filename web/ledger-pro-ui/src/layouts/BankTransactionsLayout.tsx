import { Outlet } from "react-router-dom";

export function BankTransactionsLayout() {
    return (
    <div>
        <h1 className="text-2xl font-bold mb-6">Bank Transactions</h1>

        <main>
            <Outlet />
        </main>
    </div>
    );
}

