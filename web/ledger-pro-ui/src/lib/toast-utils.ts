import { toast } from 'sonner';
import { type ApiErrorResponse } from '@/services/types/api-error-response.type';

/**
 * Displays a toast notification based on the API response.
 * If an error object is provided, it will show an error toast with details from the API error response. Otherwise, it will show a success toast with the provided message.
 * @param title The title of the toast notification. 
 * @param message An optional message to display in the toast notification.
 * @param error An optional error object that may contain details about the API error. If provided, an error toast will be shown; otherwise, a success toast will be displayed. 
 */
export const showApiToast = (title: string, message?: string, error?: any) => {
    if (error) {
        const errorData = error?.error?.response?.data as ApiErrorResponse;
        const errorMessage = errorData?.detail ?? message ?? "An unexpected error occurred.";
        
        toast.error(title, { description: errorMessage });
    } else {
        toast.success(title, { description: message });
    }
}