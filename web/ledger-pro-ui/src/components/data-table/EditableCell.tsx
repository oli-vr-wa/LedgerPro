import { useController } from "react-hook-form";
import { Input } from "../ui/Input";
import { LedgerSelect } from "../ui/form-fields/LedgerSelect";

interface CellProps {
    name: string;
    control: any; // Replace 'any' with the appropriate type from react-hook-form
    error?: string;
    type?: 'text' | 'select' | 'number' | 'date'; // Extend as needed
    options?: { value: string; label: string }[]; // For select type
    onEnter?: () => void; // Callback for Enter key press
}

export const EditableCell = ({ name, control, error, type = 'text', options, onEnter }: CellProps) => {
    const { field } = useController({ name, control });

    return (
        <div className="relative">
            {type === 'select' && options ? (
                <LedgerSelect 
                    name={name}
                    control={control as any}
                    options={options!}
                    placeholder="Select an option"
                    className={error ? 'border-destructive bg-destructive/10 h-8' : 'h-8'}
                />
            ) : (
                <Input
                    {...field}
                    type={type}
                    step={type === 'number' ? '0.01' : undefined}
                    value={field.value || ''}
                    onChange={(e) => {
                        const value = type === 'number' ? parseFloat(e.target.value) : e.target.value;
                        field.onChange(type === 'number' && isNaN(value as number) ? '' : value);
                    }}
                    onKeyDown={(e) => {
                        if (e.key === 'Enter' && onEnter) {
                            e.preventDefault();
                            onEnter();
                        }
                    }}
                    className={error ? 'border-destructive bg-destructive/10 h-8' : 'h-8'}
                />
            )}
            {/* {error && type !== 'select' && <span className="text-destructive text-sm absolute -bottom-4 left-0">{error}</span>} */}
        </div>
    );
}