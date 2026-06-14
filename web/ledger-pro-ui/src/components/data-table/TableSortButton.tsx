import { ArrowUpDown } from "lucide-react";
import { Button } from "../ui/button";

interface TableSortButtonProps {
    toggle: () => void;
    columnName: string;
}

export function TableSortButton({ toggle, columnName }: TableSortButtonProps) {
    return (
        <Button variant="headerButton" onClick={toggle} className="p-0">
            {columnName}
            <ArrowUpDown className="ml-2 h-4 w-4" />
        </Button>
    );
}