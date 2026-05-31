import React from 'react';
import { Field, FieldLabel, FieldError } from '../field';
import { Textarea } from '../textarea';

interface LedgerTextareaProps extends React.TextareaHTMLAttributes<HTMLTextAreaElement> {
    label?: string;
    error?: string; // Optional error message for validation feedback
}

export const LedgerTextarea = React.forwardRef<HTMLTextAreaElement, LedgerTextareaProps>(({ 
    label, error, ...props }, ref) =>  (
        <Field>
            {label && <FieldLabel>{label}</FieldLabel>}
            <Textarea ref={ref} {...props} />
            {error && <FieldError>{error}</FieldError>}
        </Field>
    )
);
  