import React, { useState } from 'react';
import { useMaestros } from '../viewmodels/useMaestros';

const Maestros = () => {
  const { monedas, agregarMoneda, agregarTipoCambio, loading, mensaje } = useMaestros();
  
  const [fMoneda, setFMoneda] = useState({ codigo: '', nombre: '', simbolo: '' });
  const [fTC, setFTC] = useState({ 
    fecha: new Date().toISOString().split('T')[0], 
    monedaOrigen: '', 
    monedaDestino: '', 
    tasa: 0 
  });

  const handleGuardarMoneda = async () => {
    await agregarMoneda(fMoneda);
    setFMoneda({ codigo: '', nombre: '', simbolo: '' });
  };

  const handleGuardarTasa = async () => {
    await agregarTipoCambio(fTC);
    setFTC({ ...fTC, tasa: 0 });
  };

  return (
    <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap' }}>
      {/* Formulario Moneda */}
      <div style={styles.card}>
        <h3 style={styles.cardTitle}>Nueva Moneda</h3>
        <div style={styles.field}>
          <label style={styles.label}>Código:</label>
          <input placeholder="Ej: BOB" value={fMoneda.codigo} onChange={e => setFMoneda({...fMoneda, codigo: e.target.value.toUpperCase()})} style={styles.input} />
        </div>
        <div style={styles.field}>
          <label style={styles.label}>Nombre:</label>
          <input placeholder="Ej: Bolivianos" value={fMoneda.nombre} onChange={e => setFMoneda({...fMoneda, nombre: e.target.value})} style={styles.input} />
        </div>
        <div style={styles.field}>
          <label style={styles.label}>Símbolo:</label>
          <input placeholder="Ej: Bs." value={fMoneda.simbolo} onChange={e => setFMoneda({...fMoneda, simbolo: e.target.value})} style={styles.input} />
        </div>
        <button onClick={handleGuardarMoneda} disabled={loading || !fMoneda.codigo} style={styles.btn}>Guardar Moneda</button>
      </div>

      {/* Formulario Tipo de Cambio */}
      <div style={styles.card}>
        <h3 style={styles.cardTitle}>Configurar Tasa de Cambio</h3>
        
        <div style={styles.field}>
          <label style={styles.label}>Moneda de Origen:</label>
          <select 
            value={fTC.monedaOrigen} 
            onChange={e => setFTC({...fTC, monedaOrigen: e.target.value})} 
            style={styles.input}
          >
            <option value="">Seleccione origen...</option>
            {monedas.map(m => (
              <option key={m.codigo} value={m.codigo}>
                {m.codigo} - {m.nombre}
              </option>
            ))}
          </select>
        </div>

        <div style={styles.field}>
          <label style={styles.label}>Moneda de Destino:</label>
          <select 
            value={fTC.monedaDestino} 
            onChange={e => setFTC({...fTC, monedaDestino: e.target.value})} 
            style={styles.input}
          >
            <option value="">Seleccione destino...</option>
            {monedas
              .filter(m => m.codigo !== fTC.monedaOrigen)
              .map(m => (
                <option key={m.codigo} value={m.codigo}>
                  {m.codigo} - {m.nombre}
                </option>
              ))}
          </select>
        </div>

        <div style={styles.field}>
          <label style={styles.label}>Tasa de Conversión:</label>
          <input 
            type="number" 
            step="0.0001" 
            placeholder="0.0000" 
            value={fTC.tasa} 
            onChange={e => setFTC({...fTC, tasa: parseFloat(e.target.value)})} 
            style={styles.input} 
          />
        </div>

        <button 
          onClick={handleGuardarTasa} 
          disabled={loading || !fTC.monedaOrigen || !fTC.monedaDestino} 
          style={styles.btn}
        >
          Guardar Tasa
        </button>
      </div>

      {mensaje && (
        <div style={{ width: '100%', padding: '10px', borderRadius: '4px', backgroundColor: mensaje.tipo === 'success' ? '#d4edda' : '#f8d7da', color: mensaje.tipo === 'success' ? '#155724' : '#721c24' }}>
          {mensaje.texto}
        </div>
      )}
    </div>
  );
};

const styles = {
  card: { padding: '20px', border: '1px solid #ccc', borderRadius: '8px', minWidth: '350px', backgroundColor: '#fff', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' },
  cardTitle: { marginTop: 0, marginBottom: '20px', color: '#003366', borderBottom: '2px solid #ff6600', paddingBottom: '5px' },
  field: { marginBottom: '15px' },
  label: { display: 'block', fontWeight: 'bold', marginBottom: '5px', color: '#333' },
  input: { display: 'block', width: '100%', padding: '8px', boxSizing: 'border-box', border: '1px solid #ccc', borderRadius: '4px' },
  btn: { width: '100%', padding: '10px', backgroundColor: '#003366', color: '#fff', border: 'none', borderRadius: '4px', cursor: 'pointer', fontWeight: 'bold' }
};

export default Maestros;
