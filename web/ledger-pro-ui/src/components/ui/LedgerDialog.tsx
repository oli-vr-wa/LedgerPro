import { Button } from "@/components/ui/button";
import {
    Dialog,
    DialogContent,
    DialogDescription,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog";

interface LedgerDialogProps {
    title: string;
    description?: string;
    children: React.ReactNode;
    isOpen: boolean;
    setIsOpen: (open: boolean) => void;
    showTrigger?: boolean;
    size?: 'small' | 'medium' | 'large';
}

const dialogSizeClasses = {
    small: "w-[90vw] sm:max-w-sm",
    medium: "w-[95vw] sm:max-w-4xl",
    large: "w-[95vw] sm:max-w-7xl",
} as const;

export function LedgerDialog({ title, description, children, isOpen, setIsOpen, showTrigger = true, size = 'small' }: LedgerDialogProps) {    
    return (
        <Dialog open={isOpen} onOpenChange={setIsOpen}>
            {showTrigger && (
                <DialogTrigger asChild>
                    <Button
                        className="fixed bottom-8 p-6 pt-8 pb-8 right-8 bg-blue-600 text-white rounded-full shadow-lg hover:bg-blue-700 transition hover:cursor-pointer">
                        <svg className="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={4} d="M12 4v16m8-8H4" />
                        </svg>
                    </Button>
                </DialogTrigger>
            )}
            <DialogContent onInteractOutside={(e) => e.preventDefault()} className={dialogSizeClasses[size]}>
                <DialogHeader>
                    <DialogTitle>{title}</DialogTitle>
                    <DialogDescription>{description}</DialogDescription>
                </DialogHeader>
                {children}
            </DialogContent>
        </Dialog>
    );
}