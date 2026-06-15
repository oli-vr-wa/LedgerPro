import type { Table } from "@tanstack/react-table";
import { Button } from "@/components/ui/button";

interface DataTablePaginationProps<TData> {
    table: Table<TData>;
    pagination: {
        pageIndex: number;
        pageSize: number;
    };
    setCurrentPage: (pagination: { pageIndex: number; pageSize: number }) => void;
}

export function DataTablePagination<TData>({ table, pagination, setCurrentPage }: DataTablePaginationProps<TData>) {
    // Calculate Rows Count
    const pageIndex = pagination.pageIndex; // Use the currentPage prop instead of table.getState().pagination.pageIndex
    const pageSize = pagination.pageSize;
    const totalRows = table.options.data.length;
    
    const startRow = pageIndex * pageSize + 1;
    const endRow = Math.min((pageIndex + 1) * pageSize, totalRows);

    // Generate Page Numbers Array
    const pageCount = Math.ceil(totalRows / pageSize);
    const pageNumbers = Array.from({ length: pageCount }, (_, i) => i);

    // If there is no data, don't render pagination
    if (totalRows === 0 || pageCount === 1) return null;

    return (
        <div className="flex items-center justify-between px-2 py-2 bg-blue-header/20">
            {/* Left side: Row count summary */}
            <div className="text-sm text-muted-foreground">
                Showing {startRow} to {endRow} of {totalRows} rows
            </div>

            {/* Right side: Numbered Pagination */}
            <div className="flex items-center space-x-2">
                <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage({ pageIndex: pagination.pageIndex - 1, pageSize: pagination.pageSize })}
                    disabled={pagination.pageIndex === 0}
                >
                    Previous
                </Button>

                {pageNumbers.map((pageNumber) => (
                    <Button
                        key={pageNumber}
                        variant={pageIndex === pageNumber ? "default" : "outline"}
                        size="sm"
                        onClick={() => setCurrentPage({ pageIndex: pageNumber, pageSize: pagination.pageSize })}
                        className="w-8 p-0" // Makes the buttons nicely square
                    >
                        {pageNumber + 1}
                    </Button>
                ))}

                <Button
                    variant="outline"
                    size="sm"
                    onClick={() => setCurrentPage({ pageIndex: pagination.pageIndex + 1, pageSize: pagination.pageSize })}
                    disabled={pagination.pageIndex === pageCount - 1}
                >
                    Next
                </Button>
            </div>
        </div>
    );
}