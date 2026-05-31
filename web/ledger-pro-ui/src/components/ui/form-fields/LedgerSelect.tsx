// import type { FieldError } from 'react-hook-form';
// import { InputBase } from './InputBase';
// import { inputStyles } from '../../theme';
import { useController, type Control } from 'react-hook-form';
import { Field, FieldLabel, FieldError } from '../field';
import { Select, SelectContent, SelectGroup, SelectTrigger, SelectValue, SelectItem } from '../select';

interface LedgerSelectProps extends React.SelectHTMLAttributes<HTMLSelectElement> {
    name: string;
    control: Control<any>;
    label?: string;
    options: readonly string[];
    placeholder?: string;
}

export function LedgerSelect({ name, control, label, options, placeholder } : LedgerSelectProps) {
    const { field, fieldState: { error } } = useController({ name, control });

    return (
        <Field>
            {label && <FieldLabel>{label}</FieldLabel>}
            <Select value={field.value} onValueChange={field.onChange}>
                <SelectTrigger>
                    <SelectValue placeholder={placeholder || 'Select an option...'} />
                </SelectTrigger>
                <SelectContent>
                    <SelectGroup>
                        {options.map((option) => (
                            <SelectItem key={option} value={option}>
                                {option}
                            </SelectItem>
                        ))}
                    </SelectGroup>
                </SelectContent>
            </Select>
            {error && <FieldError>{error.message}</FieldError>}
        </Field>        
    );
}