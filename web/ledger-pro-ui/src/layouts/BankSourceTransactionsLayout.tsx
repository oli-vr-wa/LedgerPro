import { Outlet, useNavigate, useLocation, useParams } from "react-router-dom";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";

export function BankSourceTransactionsLayout() {
    const { bankSourceId } = useParams();
    const navigate = useNavigate();
    const location = useLocation();

    const displayName = location.state?.displayName || 'Bank Transactions';

    const activeTab = location.pathname.includes('upload') ? 'upload' : 'years';

    return (
        <div>
            {/* Back button to return to Account Selection */}
            <Button variant="ghost" onClick={() => navigate("/transactions")} className="mb-2">
                <ArrowLeft className="mr-2 h-4 w-4" /> Back to Accounts
            </Button>

            <h2 className="text-2xl font-bold mb-4">Bank Transactions for {displayName}</h2>

            <Tabs
                defaultValue={activeTab} 
                onValueChange={(value) => navigate(value === 'years' ? `/transactions/${bankSourceId}` : `/transactions/${bankSourceId}/upload`)}>
                <TabsList className="bg-blue-header">
                    <TabsTrigger value="years">Financial Years</TabsTrigger>
                    <TabsTrigger value="upload">Upload Statements</TabsTrigger>
                </TabsList>
            </Tabs>

             <main>
                <Outlet />
            </main>
        </div>
    );
}