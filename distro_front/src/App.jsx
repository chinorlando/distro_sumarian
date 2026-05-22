import React, { useState } from 'react';
import AdicionCuentas from './views/AdicionCuentas';
import Operaciones from './views/Operaciones';
import Transferencias from './views/Transferencias';
import ConsultaSaldos from './views/ConsultaSaldos';
import Maestros from './views/Maestros';

function App() {
  const [activeTab, setActiveTab] = useState('saldos');

  const renderView = () => {
    switch (activeTab) {
      case 'config': return <Maestros />;
      case 'registro': return <AdicionCuentas />;
      case 'operaciones': return <Operaciones />;
      case 'transferencias': return <Transferencias />;
      case 'saldos': return <ConsultaSaldos />;
      default: return <ConsultaSaldos />;
    }
  };

  return (
    <div style={styles.app}>
      <header style={styles.header}>
        <h1 style={styles.logo}>BANCO DE CRÉDITO BCP</h1>
        <nav style={styles.nav}>
          <button style={activeTab === 'config' ? styles.activeBtn : styles.btn} onClick={() => setActiveTab('config')}>0. Config</button>
          <button style={activeTab === 'registro' ? styles.activeBtn : styles.btn} onClick={() => setActiveTab('registro')}>1. Registro</button>
          <button style={activeTab === 'operaciones' ? styles.activeBtn : styles.btn} onClick={() => setActiveTab('operaciones')}>2. Operaciones</button>
          <button style={activeTab === 'transferencias' ? styles.activeBtn : styles.btn} onClick={() => setActiveTab('transferencias')}>3. Transferencias</button>
          <button style={activeTab === 'saldos' ? styles.activeBtn : styles.btn} onClick={() => setActiveTab('saldos')}>4/5. Saldos & Historial</button>
        </nav>
      </header>

      <main style={styles.main}>
        {renderView()}
      </main>

      <footer style={styles.footer}>
        <p>&copy; 2026 - Prueba Técnica BCP - Orlando Mamani Molina</p>
      </footer>
    </div>
  );
}

const styles = {
  app: { fontFamily: 'Arial, sans-serif', minHeight: '100vh', display: 'flex', flexDirection: 'column', backgroundColor: '#f4f4f4' },
  header: { backgroundColor: '#003366', color: 'white', padding: '1rem', textAlign: 'center' },
  logo: { margin: 0, fontSize: '1.5rem', marginBottom: '15px' },
  nav: { display: 'flex', justifyContent: 'center', gap: '10px', flexWrap: 'wrap' },
  btn: { padding: '10px 15px', cursor: 'pointer', border: 'none', backgroundColor: '#004a99', color: 'white', borderRadius: '4px' },
  activeBtn: { padding: '10px 15px', cursor: 'pointer', border: 'none', backgroundColor: '#ff6600', color: 'white', borderRadius: '4px', fontWeight: 'bold' },
  main: { flex: 1, padding: '20px', maxWidth: '1000px', margin: '0 auto', width: '100%' },
  footer: { textAlign: 'center', padding: '10px', backgroundColor: '#eee', fontSize: '0.8rem' }
};

export default App;
