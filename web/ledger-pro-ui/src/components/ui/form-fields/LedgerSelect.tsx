// import type { FieldError } from 'react-hook-form';
// import { InputBase } from './InputBase';
// import { inputStyles } from '../../theme';
import { useController, type Control } from 'react-hook-form';
import { Field, FieldLabel, FieldError } from '../field';
import { Select, SelectContent, SelectGroup, SelectTrigger, SelectValue, SelectItem } from '../select';

export interface SelectOption {
    value: string;
    label: string;
}

interface LedgerSelectProps<T> extends React.SelectHTMLAttributes<HTMLSelectElement> {
    name: string;
    control: Control<any>;
    label?: string;
    options: readonly T[];
    placeholder?: string;
    readOnly?: boolean;
}

export function LedgerSelect<T extends string | SelectOption>({ name, control, label, options, placeholder, readOnly }: LedgerSelectProps<T>) {
    const { field, fieldState: { error } } = useController({ name, control });

    // Helper to check if options are simple strings or objects with value/label
    const isSelectOption = (option: any): option is SelectOption => {
        return typeof option !== 'string';
    }

    return (
        <Field>
            {label && <FieldLabel>{label}</FieldLabel>}
            <Select value={field.value} onValueChange={field.onChange} disabled={readOnly}>
                <SelectTrigger>
                    <SelectValue placeholder={placeholder || 'Select an option...'} />
                </SelectTrigger>
                <SelectContent>
                    <SelectGroup>
                        {options.map((option) => {
                            // Extract value and label based on whether it's a string or an object
                            const value = isSelectOption(option) ? option.value : option as string;
                            const label = isSelectOption(option) ? option.label : option as string;
                            
                            return (
                                <SelectItem key={value} value={value}>
                                    {label}
                                </SelectItem>
                            );
                        })}
                    </SelectGroup>
                </SelectContent>
            </Select>
            {error && <FieldError>{error.message}</FieldError>}
        </Field>        
    );
}