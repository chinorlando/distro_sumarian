import React, { useState } from 'react';
import Saldos from './components/Saldos';
import RegistroCuentas from './components/RegistroCuentas';
import Operaciones from './components/Operaciones';
import Transferencias from './components/Transferencias';
import Configuracion from './components/Configuracion';

function App() {
  const [tab, setTab] = useState('saldos');

  return (
    <div style={{ padding: '20px', fontFamily: 'Arial, sans-serif', maxWidth: '800px', margin: '0 auto' }}>
      <h2 style={{ color: '#002D72' }}>BANCO BCP</h2>
      
      {/* Menú de Pestañas */}
      <div style={{ marginBottom: '20px', display: 'flex', gap: '8px' }}>
        <button onClick={() => setTab('saldos')} style={{ padding: '8px 12px', cursor: 'pointer', fontWeight: tab === 'saldos' ? 'bold' : 'normal' }}>
          4/5. Consulta de Saldos
        </button>
        <button onClick={() => setTab('registro')} style={{ padding: '8px 12px', cursor: 'pointer', fontWeight: tab === 'registro' ? 'bold' : 'normal' }}>
          1. Crear Cuenta
        </button>
        <button onClick={() => setTab('operaciones')} style={{ padding: '8px 12px', cursor: 'pointer', fontWeight: tab === 'operaciones' ? 'bold' : 'normal' }}>
          2. Abonos/Retiros
        </button>
        <button onClick={() => setTab('transferencias')} style={{ padding: '8px 12px', cursor: 'pointer', fontWeight: tab === 'transferencias' ? 'bold' : 'normal' }}>
          3. Transferencias
        </button>
        <button onClick={() => setTab('config')} style={{ padding: '8px 12px', cursor: 'pointer', fontWeight: tab === 'config' ? 'bold' : 'normal' }}>
          0. Configuración
        </button>
      </div>

      <hr />

      {/* Renderizado de Componentes según la pestaña activa */}
      {tab === 'saldos' && <Saldos />}
      {tab === 'registro' && <RegistroCuentas />}
      {tab === 'operaciones' && <Operaciones />}
      {tab === 'transferencias' && <Transferencias />}
      {tab === 'config' && <Configuracion />}
    </div>
  );
}

export default App;
