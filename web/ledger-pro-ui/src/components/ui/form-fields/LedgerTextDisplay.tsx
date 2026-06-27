import React from "react";
import { Field, FieldContent, FieldDescription, FieldLabel } from "../field";

interface LedgerTextDisplayProps {
    label?: string;
    value: string;
}

export const LedgerTextDisplay = React.forwardRef<HTMLDivElement, LedgerTextDisplayProps>(({ label, value, ...props }, ref) => (
    <Field ref={ref}>
        <FieldContent {...props}>
            {label && <FieldLabel>{label}</FieldLabel>}
            <FieldDescription>{value}</FieldDescription>
        </FieldContent>
    </Field>
));