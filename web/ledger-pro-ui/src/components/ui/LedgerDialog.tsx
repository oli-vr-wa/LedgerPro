import { Button } from "@/components/ui/button";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog";

interface LedgerDialogProps {
    title: string;
    children: React.ReactNode;
    isOpen: boolean;
    setIsOpen: (open: boolean) => void;
}

export function LedgerDialog({ title, children, isOpen, setIsOpen }: LedgerDialogProps) {
    return (
        <Dialog open={isOpen} onOpenChange={setIsOpen}>
            <DialogTrigger asChild>
                <Button
                    className="fixed bottom-8 p-6 pt-8 pb-8 right-8 bg-blue-600 text-white rounded-full shadow-lg hover:bg-blue-700 transition hover:cursor-pointer">
                    <svg className="w-12 h-12" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={4} d="M12 4v16m8-8H4" />
                    </svg>
                </Button>
            </DialogTrigger>
            <DialogContent>
                <DialogHeader>
                    <DialogTitle>{title}</DialogTitle>
                </DialogHeader>
                {children}
            </DialogContent>
        </Dialog>
    );
}