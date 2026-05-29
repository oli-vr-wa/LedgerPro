import type { FieldError } from 'react-hook-form';
import { InputBase } from './InputBase';
import { inputStyles } from '../../theme';

interface InputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    placeholder: string;
    error?: FieldError;
}

export function Input({ placeholder, error, ...props }: InputProps) {
    return (
        <InputBase error={error}>
            <input placeholder={placeholder} className={inputStyles} {...props} />            
        </InputBase>
    );
}