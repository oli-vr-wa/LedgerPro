import { ArrowUpDown } from "lucide-react";
import { Button } from "./button";

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

export const sortableHeader = (column: any, columnName: string) => {
    const toggleSorting = () => {
        column.toggleSorting(column.getIsSorted() === "asc");
    };
    return <TableSortButton toggle={toggleSorting} columnName={columnName} />;
};