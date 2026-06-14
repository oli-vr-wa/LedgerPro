import { Button } from "@/components/ui/button";
import type { BankTransactionsYearRow } from "@/types/bank-transactions-year-row.types";
import { ArrowLeft } from "lucide-react";
import { useState } from "react";
import { useParams } from "react-router-dom";
import { BankTransactionsYear } from "./bank-transactions-year/BankTransactionsYear";
import { BankTransactionsYearsOverview } from "./bank-transactions-years-overview/BankTransactionsYearsOverview";

export function BankTransactionsYearSelection() {
    const [yearSelection, setYearSelection] = useState<number | null>(null);
    const { bankSourceId } = useParams<{ bankSourceId: string }>();

    const handleYearSelect = (bankTransactionsYearRow: BankTransactionsYearRow) => {
        setYearSelection(bankTransactionsYearRow.yearEnding);
    }

    if (!bankSourceId) {
        return <div className="pt-4">Bank Source ID not provided.</div>;
    }

    if (bankSourceId && yearSelection !== null) {
        return (
            <div className="pt-4">
                <Button variant="ghost" onClick={() => setYearSelection(null)} className="mb-2">
                    <ArrowLeft className="mr-2 h-4 w-4" /> Back to Financial Years Overview
                </Button>

                <BankTransactionsYear bankSourceId={bankSourceId!} year={yearSelection!} />
            </div>
        );
    }
    else if (bankSourceId && yearSelection === null) {
        return <BankTransactionsYearsOverview bankSourceId={bankSourceId!} onRowClick={handleYearSelect} />;
    }
}