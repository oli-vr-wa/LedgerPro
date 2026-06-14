import { TableSortButton } from "../TableSortButton";

export const sortableHeader = (column: any, columnName: string) => {
    const toggleSorting = () => {
        column.toggleSorting(column.getIsSorted() === "asc");
    };
    return <TableSortButton toggle={toggleSorting} columnName={columnName} />;
};