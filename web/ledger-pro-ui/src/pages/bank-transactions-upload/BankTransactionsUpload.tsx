import { DataTable } from "@/components/data-table/DataTable";
import { bankSourceService } from "@/services/bankSourceService";
import type { StatementImportRow } from "@/types/statement-import-row.type";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { columns } from "./columns";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { UploadCloud } from "lucide-react";

export function BankTransactionsUpload() {
    const [file, setFile] = useState<File | null>(null);
    const [uploading, setUploading] = useState(false);
    const [statementImports, setStatementImports] = useState<StatementImportRow[]>([]);
    const { bankSourceId } = useParams();

    useEffect(() => {
        if (!bankSourceId) return;
        bankSourceService.getStatementImports(bankSourceId)
            .then(response => setStatementImports(response.data))
            .catch(error => console.error('Error fetching statement imports:', error));
    }, [bankSourceId]);

    const handleFileChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        if (event.target.files && event.target.files.length > 0) {
            setFile(event.target.files[0]);
        }
    };

    const handleUpload = () => {       
        console.log('Uploading file:', file); 
        if (!file || !bankSourceId) return;

        setUploading(true);
        bankSourceService.importStatement(bankSourceId, file)
            .then(() => {
                // Refresh file state after successful upload
                setFile(null);
                // Refresh the list of statement imports after successful upload
                return bankSourceService.getStatementImports(bankSourceId);
            })
            .then(response => setStatementImports(response.data))
            .catch(error => console.error('Error uploading statement:', error))
            .finally(() => setUploading(false));
    };

    return (
        <div className="pt-4 space-y-6">
            
            <Card>
                <CardHeader>
                    <CardTitle>Upload Bank Statement</CardTitle>
                    <CardDescription>Choose a file to upload your bank statement in CSV format.</CardDescription>
                </CardHeader>
                <CardContent>
                    <div className="flex items-center space-x-4">
                        <div className="grid w-full max-w-sm items-center gap-1.5">
                            <Input className="cursor-pointer" type="file" accept=".csv" onChange={handleFileChange} disabled={uploading} />
                        </div>
                        <Button variant="submit" onClick={handleUpload} disabled={uploading}>
                            <UploadCloud className="w-4 h-4 mr-1" />
                            {uploading ? 'Uploading...' : 'Upload'}
                        </Button>
                    </div>
                </CardContent>
            </Card>

            <DataTable columns={columns} data={statementImports} />
        </div>
    );
}