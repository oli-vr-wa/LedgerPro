import { useEffect, useState } from "react";
import api from "../services/api";

export function TestApi() {
    const [data, setData] = useState<string>("Loading...");

    useEffect(() => {
        api.get("/banksources/")
            .then((response) => setData(response.data))
            .catch((error) => {
                console.error("API Error:", error);
                setData("Failed to connect to API");
            });
        }, []);

    return (
        <div className="p-4 border rounded shadow-md mt-4">
            <h2 className="text-xl font-semibold">API Test</h2>
            <p className="mt-2">Status: {data}</p>
        </div>
    );
}