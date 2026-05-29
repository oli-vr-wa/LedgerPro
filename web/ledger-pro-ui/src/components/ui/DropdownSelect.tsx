import type { FieldError } from 'react-hook-form';
import { InputBase } from './InputBase';
import { inputStyles } from '../../theme';

interface DropdownSelectProps<T extends readonly string[]> extends React.SelectHTMLAttributes<HTMLSelectElement> {
    options: T;
    error?: FieldError | any;
    placeholder?: string;
}

export function DropdownSelect<T extends readonly string[]>({ options, placeholder, error, children, ...props }: DropdownSelectProps<T>) {
    return (
        <InputBase error={error}>
            <select className={inputStyles} {...props}>
                <option value="">
                    {placeholder || 'Select an option...'}
                </option>
                {options.map((option) => (
                    <option key={option} value={option}>
                        {option}
                    </option>
                ))}
                {children}
            </select>
        </InputBase>
    );
}