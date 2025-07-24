import { Routes, Route } from 'react-router-dom';
import Layout from './components/Layout';
import Home from './pages/Home';
import AddRecord from './pages/AddRecord';
import Wallets from './pages/Wallets';
import Categories from './pages/Categories';

function App() {
  return (
    <Layout>
      <Routes>
        <Route path="/" element={<Home />} />
        <Route path="/add" element={<AddRecord />} />
        <Route path="/wallets" element={<Wallets />} />
        <Route path="/categories" element={<Categories />} />
      </Routes>
    </Layout>
  );
}

export default App;