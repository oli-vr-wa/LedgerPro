import { BANK_TYPES } from '@/types/bank-source.types';
import * as z from 'zod';

export const bankSourceSchema = z.object({
    bankName: z.string().min(1, "Bank Name is required"),
    accountName: z.string().min(1, "Account Name is required"),
    accountNumber: z.string().regex(/^\d{6,9}$/, "Account Number is required"),
    bankType: z.enum(BANK_TYPES).or(z.literal('')) // Allow empty string for placeholder option
}).refine((data) => data.bankType !== '', {
    message: "Bank Type is required",
    path: ['bankType']
});

export type BankSourceFormData = z.infer<typeof bankSourceSchema>;