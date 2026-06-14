// Date formatting utility for data table cells - dd/mm/yyyy format
export const formatDate = (dateString: string | Date) => {
    if (!dateString) return "-";
    const date = new Date(dateString);

    return new Intl.DateTimeFormat('en-AU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric'
    }).format(date);
};

// Date & time formatting utility for data table cells - dd/mm/yyyy hh:mm format
export const formatDateTime = (dateString: string | Date) => {
    if (!dateString) return "-";
    const date = new Date(dateString);

    return new Intl.DateTimeFormat('en-AU', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit',
        hour12: false
    }).format(date);
};

// Currency formatting utility for data table cells - formats number as currency string
export const formatCurrency = (amount: number) => {
    if (amount === null || amount === undefined) return "-";

    return new Intl.NumberFormat('en-AU', {
        style: 'currency',
        currency: 'AUD'
    }).format(amount);
};