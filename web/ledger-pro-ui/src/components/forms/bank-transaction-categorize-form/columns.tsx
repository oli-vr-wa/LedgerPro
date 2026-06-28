import { EditableCell } from "@/components/data-table/EditableCell";
import { Button } from "@/components/ui/button";
import type { BankTransactionCategorizeFormData } from "@/schemas/transaction-categorize-item.schemas";
import type { GeneralLedgerAccount } from "@/types/general-ledger-account.types";
import type { ColumnDef } from "@tanstack/react-table";
import { Trash2 } from "lucide-react";

export const getItemsColumns = (): ColumnDef<BankTransactionCategorizeFormData['items'][number]>[] => [
        {
            accessorKey: 'description',
            header: 'Description',
            cell: ({ row, table }) => {
                const meta = table.options.meta!;                
                return (
                    <EditableCell
                        name={`items.${row.index}.description`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.description?.message as string}
                    />
                )
            }
        },
        {
            accessorKey: 'reference',
            header: 'Reference',
            cell: ({ row, table }) => {
                const meta = table.options.meta!;
                return (
                    <EditableCell
                        name={`items.${row.index}.reference`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.reference?.message as string}
                    />
                )
            }
        },
        {
            accessorKey: 'generalLedgerAccountId',
            header: 'GL Classification',
            cell: ({ row, table }) => {
                const meta = table.options.meta as any;
                return (
                    <EditableCell
                        name={`items.${row.index}.generalLedgerAccountId`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.generalLedgerAccountId?.message as string}
                        type="select"
                        options={meta.accounts?.map((account: GeneralLedgerAccount) => ({ value: account.id.toString(), label: `${account.id} - ${account.name}` })) ?? []}
                    />
                )
            }
        },
        {
            accessorKey: 'amount',
            header: 'Amount',
            cell: ({ row, table }) => {
                const meta = table.options.meta!;
                return (
                    <EditableCell
                        name={`items.${row.index}.amount`}
                        control={meta.control}
                        error={(meta.errors?.items as any)?.[row.index]?.amount?.message as string}
                        type="number"
                    />
                )
            }
        },
        {
            id: 'actions',
            header: 'Actions',
            cell: ({ row, table }) => (
                <Button
                    variant="ghost"
                    size="icon"
                    onClick={() => table.options.meta?.removeRowById?.(row.id)}>
                        <Trash2 className="h-4 w-4 text-muted-foreground hover:text-destructive" />
                </Button>
            )
        }
    ];