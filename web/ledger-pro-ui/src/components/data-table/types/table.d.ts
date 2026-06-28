import '@tanstack/react-table';
import { Control, FieldErrors, UseFormRegister } from "react-hook-form";

declare module '@tanstack/react-table' {
    interface TableMeta<TData extends RowData> {
        control?: Control<any>; // Add the control property to TableMeta
        register?: UseFormRegister<any>; // Add the register property to TableMeta
        errors?: FieldErrors<any>; // Add the errors property to TableMeta
        removeRow?: (index: number) => void; // Add the removeRow property to TableMeta
        removeRowById?: (rowId: string) => void; // Optional stable-id row removal callback
        onRowEnter?: (row: TData) => void; // Add the onRowEnter property to TableMeta
    }
}
