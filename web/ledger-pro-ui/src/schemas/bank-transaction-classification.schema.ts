import z from "zod";

export const bankTransactionClassificationSchema = z.object({
    generalLedgerClassification: z.string().min(1, "General Ledger Classification is required")
});

export type BankTransactionClassificationFormData = z.infer<typeof bankTransactionClassificationSchema>;