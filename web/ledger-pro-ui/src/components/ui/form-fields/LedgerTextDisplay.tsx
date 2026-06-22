import { Field, FieldContent, FieldDescription, FieldLabel } from "../field";

interface LedgerTextDisplayProps {
    label?: string;
    value: string;
}

export const LedgerTextDisplay: React.FC<LedgerTextDisplayProps> = ({ label, value }) => (
    <Field>
        <FieldContent>
            {label && <FieldLabel>{label}</FieldLabel>}
            <FieldDescription>{value}</FieldDescription>
        </FieldContent>
    </Field>
)