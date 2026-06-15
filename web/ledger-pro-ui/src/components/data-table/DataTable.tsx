import { type ColumnDef, type SortingState , flexRender, getCoreRowModel, useReactTable, getSortedRowModel, getPaginationRowModel, getFilteredRowModel } from "@tanstack/react-table";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { useMemo, useState } from "react";
import { DataTablePagination } from "./DataTablePagination";

interface DataTableProps<TData, Tvalue> {
    columns: ColumnDef<TData, Tvalue>[];
    data: TData[];
    onRowClick?: (row: TData) => void; // Optional click handler for rows
    loading?: boolean; // Optional loading state
}

export function DataTable<TData, Tvalue>({ columns, data, onRowClick, loading }: DataTableProps<TData, Tvalue>) {
    const tableData = useMemo(() => data ?? [], [data]);
    const pageSize = 20; // Default page size
    const [sorting, setSorting] = useState<SortingState>([]);
    const [pagination, setPagination] = useState({
        pageIndex: 0,
        pageSize: pageSize,
    });
    
    const table = useReactTable({
        data: tableData,
        columns,
        state: {
            sorting,
            pagination,
        },
        autoResetPageIndex: false, // Prevent resetting page index when data changes
        onSortingChange: setSorting,
        onPaginationChange: setPagination,
        getSortedRowModel: getSortedRowModel(),
        getCoreRowModel: getCoreRowModel(),
        getPaginationRowModel: getPaginationRowModel(),
        getFilteredRowModel: getFilteredRowModel(),
    });

    return (
        <div className="bg-white shadow rounded-lg overflow-hidden">
            <Table>
                <TableHeader className="bg-blue-header">
                    {table.getHeaderGroups().map((headerGroup) => (
                        <TableRow key={headerGroup.id}>
                            {headerGroup.headers.map((header) => {
                                return (
                                    <TableHead key={header.id}>
                                        {header.isPlaceholder ? null : flexRender(header.column.columnDef.header, header.getContext())}
                                    </TableHead>
                                );
                            })}
                        </TableRow>
                    ))}
                </TableHeader>
                <TableBody>
                    {table.getRowModel().rows?.length ? (
                        table.getRowModel().rows.map((row) => (
                            <TableRow
                                key={row.id}
                                data-state={row.getIsSelected() && "selected"}
                                onClick={() => onRowClick?.(row.original)}
                            >
                                {row.getVisibleCells().map((cell) => (
                                    <TableCell key={cell.id}>
                                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                                    </TableCell>
                                ))}
                            </TableRow>
                        ))
                    ) : (
                        <TableRow>
                            <TableCell colSpan={columns.length} className="h-24 text-center">
                                {loading ? "Loading..." : "No records."}
                            </TableCell>
                        </TableRow>
                     )}                    
                </TableBody>
            </Table>
            
            {loading ? null : <DataTablePagination table={table} pagination={pagination} setCurrentPage={setPagination} />}
        </div>
    );
}