export function InputBase({error, children}: any) {
    return (
        <div>
            {children}
            {error?.message && <span className="text-red-500 text-sm">{error.message}</span>}
        </div>
    );
}