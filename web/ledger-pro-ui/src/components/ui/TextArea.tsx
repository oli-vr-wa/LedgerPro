import type { FieldError } from 'react-hook-form';
import { InputBase } from './InputBase';
import { inputStyles } from '../../theme';

interface TextAreaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
    placeholder: string;
    error?: FieldError;
}

export function TextArea({ placeholder, error, ...props }: TextAreaProps) {
    return (
        <InputBase error={error}>
            <textarea placeholder={placeholder} className={inputStyles} {...props} />
        </InputBase>
    );
}