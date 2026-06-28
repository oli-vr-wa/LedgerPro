import z from "zod";

const transactionCategorizeItemSchema = z.object({
    generalLedgerAccountId: z.string().min(1, "Please select a General Ledger Account"),
    description: z.string().min(1, "Description is required"),
    reference: z.string().optional(),
    amount: z.number()
})

export const createCategorizeSplitSchema = (transactionAmount: number) => {
    return z.object({
        items: z.array(transactionCategorizeItemSchema).min(1, "At least one item is required")
    }).refine(
        (data) => {
            const amountSum = data.items.reduce((sum, item) => sum + ((item.amount * 100) || 0), 0);
            return amountSum === transactionAmount * 100;
        },
        {
            message: "The sum of the amounts must equal the transaction amount",    
            path: ['items']
        }
    );
}

export type BankTransactionCategorizeFormData = z.infer<ReturnType<typeof createCategorizeSplitSchema>>;