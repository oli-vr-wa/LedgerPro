import { BANK_TRANSACTION_MAPPING_STRATEGIES } from '@/types/bank-transaction-mapping.types';
import * as z from 'zod';

export const bankTransactionMappingSchema = z.object({
    searchTerm: z.string().min(3, "Search Term is required"),
    matchStrategy: z.enum(BANK_TRANSACTION_MAPPING_STRATEGIES).or(z.literal('')), // Allow empty string for placeholder option
    targetGeneralLedgerAccountId: z.string().min(1, "Please select a General Ledger Account"),
    descriptionTemplate: z.string().min(5, "Description Template is required"),
    referenceTemplate: z.string().min(5, "Reference Template is required"),
    priority: z.number().min(1, "Priority is required")
}).refine((data) => data.matchStrategy !== '', {
    message: "Match Strategy is required",  
    path: ['matchStrategy']
});  

export type BankTransactionMappingFormData = z.infer<typeof bankTransactionMappingSchema>;