import { Outlet, useNavigate, useLocation, useParams } from "react-router-dom";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Button } from "@/components/ui/button";
import { ArrowLeft } from "lucide-react";
import { useEffect, useState } from "react";
import { bankSourceService } from "@/services/bankSourceService";

export function BankSourceTransactionsLayout() {
    const [activeTab, setActiveTab] = useState('years');
    const { bankSourceId } = useParams();
    const navigate = useNavigate();
    const location = useLocation();

    const displayName = location.state?.displayName || 'Bank Transactions';

    //const activeTab = location.pathname.includes('upload') ? 'upload' : 'years';

    // Fetch the bank source details if the display name is not available in the location state (e.g., on page refresh)
    // You can use the bankSourceId to fetch the details from your API and set the display name accordingly.
    useEffect(() => {
        // Set the active tab based on the current URL path
        setActiveTab(location.pathname.includes('upload') ? 'upload' : 'years');

        if (!displayName) {
            // Fetch bank source details using bankSourceId and set displayName state using the bankSourceService.
            bankSourceService.getBankSourceById(bankSourceId!)
                .then(response => {
                    const bankSource = response.data;
                    const fetchedDisplayName = `${bankSource.bankName} - ${bankSource.accountName}`;
                    // Update the location state with the fetched display name
                    navigate(location.pathname, { state: { displayName: fetchedDisplayName }, replace: true });
                })
                .catch(error => {
                    console.error("Failed to fetch bank source details:", error);
                });
        }
    }, [bankSourceId, displayName, location.pathname, navigate]);

    const handleTabChange = (value: string) => {
        setActiveTab(value);
        navigate(value === 'years' ? `/transactions/${bankSourceId}` : `/transactions/${bankSourceId}/upload`);
    };

    return (
        <div>
            {/* Back button to return to Account Selection */}
            <Button variant="ghost" onClick={() => navigate("/transactions")} className="mb-2">
                <ArrowLeft className="mr-2 h-4 w-4" /> Back to Accounts
            </Button>

            <h2 className="text-2xl font-bold mb-4">Bank Transactions for {displayName}</h2>

            <Tabs
                defaultValue={activeTab} 
                value={activeTab} 
                onValueChange={handleTabChange}>
                <TabsList className="bg-blue-header/50">
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