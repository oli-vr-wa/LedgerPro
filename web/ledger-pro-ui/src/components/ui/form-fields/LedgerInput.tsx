import React from 'react';
import { Field, FieldLabel, FieldError } from '../field';
import { Input } from '../input';

interface LedgerInputProps extends React.InputHTMLAttributes<HTMLInputElement> {
    label?: string;
    error?: string; // Optional error message for validation feedback
}

export const LedgerInput = React.forwardRef<HTMLInputElement, LedgerInputProps>(({ 
    label, error, ...props }, ref) =>  (
        <Field>
            {label && <FieldLabel>{label}</FieldLabel>}
            <Input ref={ref} {...props} />
            {error && <FieldError>{error}</FieldError>}
        </Field>
    )
);