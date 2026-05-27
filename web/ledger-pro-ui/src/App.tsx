import { TestApi } from './components/TestApi';

function App() {
  return (
    <div className="min-h-screen flex items-top justify-center bg-gray-100 pt-8">      
      <h1 className="text-4xl font-bold text-gray-800">
        LedgerPro
      </h1>
      <TestApi />
    </div>
  );
}

export default App;