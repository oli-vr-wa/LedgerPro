import * as z from 'zod';
import { GENERAL_LEDGER_ACCOUNT_TYPES } from '../types/general-ledger-account.types';

export const glAccountSchema = z.object({
    id: z.string().regex(/^\d{4}$/, "Account Code must be a 4-digit number"),
    name: z.string().min(1, "Account Name is required"),
    description: z.string().optional(),
    accountType: z.enum(GENERAL_LEDGER_ACCOUNT_TYPES).or(z.literal('')) // Allow empty string for placeholder option
}).refine((data) => data.accountType !== '', {
    message: "Account Type is required",
    path: ['accountType']
});

export type glAccountFormData = z.infer<typeof glAccountSchema>;