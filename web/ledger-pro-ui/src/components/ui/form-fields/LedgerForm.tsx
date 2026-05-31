import { Field, FieldGroup, FieldSet } from '../field';

interface LedgerFormProps {
    children: React.ReactNode;
    onSubmit: (e: React.SubmitEvent<HTMLFormElement>) => void;
}

export function LedgerForm({ children, onSubmit }: LedgerFormProps) {
    return (
        <form onSubmit={onSubmit} noValidate>
            <FieldGroup>
                {children}
            </FieldGroup>
        </form>
    );
}

export function LedgerFormBody({ children }: { children: React.ReactNode }) {
    return (
        <FieldSet>
            <FieldGroup>
                {children}
            </FieldGroup>
        </FieldSet>
    );
}

export function LedgerFormFooter({ children }: { children: React.ReactNode }) {
    return (
        <Field orientation="horizontal" className="justify-end">
            {children}
        </Field>
    );
}