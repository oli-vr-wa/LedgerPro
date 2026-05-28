import { Outlet, NavLink } from "react-router-dom";

export function DashboardLayout() {

    const getLinkClass = ({ isActive }: { isActive: boolean }) =>
        isActive 
            ? "text-blue-400 font-bold border-l-4 border-blue-400 pl-2" 
            : "text-white hover:text-blue-300 pl-3";

    return (
        <div className="flex min-h-screen bg-slate-300">
            {/* Sidebar */}
            <nav className="w-64 bg-slate-900 text-white p-5">
                <h2 className="text-2xl font-bold mb-6">LedgerPro</h2>
                <ul className="space-y-4">
                    <li><NavLink to="/" end className={getLinkClass}>Dashboard</NavLink></li>
                    <li><NavLink to="/banksources" className={getLinkClass}>Bank Sources</NavLink></li>
                    <li><NavLink to="/settings" className={getLinkClass}>Settings</NavLink></li>
                </ul>                
            </nav>

            {/* Main Content */}
            <main className="flex-1 p-10">
                <Outlet />
            </main>
        </div>
    );
}