import React, { useState, useEffect } from 'react';
import { API_URL } from '../config';

function Configuracion() {
  const [monedas, setMonedas] = useState([]);
  const [monedaForm, setMonedaForm] = useState({ codigo: '', nombre: '', simbolo: '' });
  const [tasaForm, setTasaForm] = useState({ monedaOrigen: '', monedaDestino: '', tasa: '' });

  const cargarMonedas = () => {
    fetch(`${API_URL}/Monedas`)
      .then(r => r.json())
      .then(setMonedas)
      .catch(console.error);
  };

  useEffect(() => {
    cargarMonedas();
  }, []);

  const handleMoneda = (e) => {
    e.preventDefault();
    fetch(`${API_URL}/Monedas`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(monedaForm)
    }).then(async r => {
      if (!r.ok) throw new Error("Error al guardar moneda");
      cargarMonedas();
      setMonedaForm({ codigo: '', nombre: '', simbolo: '' });
      alert("Moneda guardada exitosamente");
    }).catch(err => alert(err.message));
  };

  const handleTasa = (e) => {
    e.preventDefault();
    fetch(`${API_URL}/TipoCambios`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...tasaForm, tasa: parseFloat(tasaForm.tasa) })
    }).then(async r => {
      if (!r.ok) throw new Error("Error al guardar tipo de cambio");
      setTasaForm({ monedaOrigen: '', monedaDestino: '', tasa: '' });
      alert("Tipo de cambio guardado exitosamente");
    }).catch(err => alert(err.message));
  };

  return (
    <div style={{ display: 'flex', flexWrap: 'wrap', gap: '40px', marginTop: '15px' }}>
      {/* MONEDAS */}
      <div>
        <h4>Registrar Moneda</h4>
        <form onSubmit={handleMoneda} style={{ display: 'flex', flexDirection: 'column', width: '250px', gap: '8px' }}>
          <div>
            <label style={{ display: 'block', fontWeight: 'bold' }}>Código:</label>
            <input type="text" value={monedaForm.codigo} onChange={e => setMonedaForm({ ...monedaForm, codigo: e.target.value })} style={{ width: '100%', padding: '4px' }} placeholder="Ej: BOB, USD" required />
          </div>
          <div>
            <label style={{ display: 'block', fontWeight: 'bold' }}>Nombre:</label>
            <input type="text" value={monedaForm.nombre} onChange={e => setMonedaForm({ ...monedaForm, nombre: e.target.value })} style={{ width: '100%', padding: '4px' }} required />
          </div>
          <div>
            <label style={{ display: 'block', fontWeight: 'bold' }}>Símbolo:</label>
            <input type="text" value={monedaForm.simbolo} onChange={e => setMonedaForm({ ...monedaForm, simbolo: e.target.value })} style={{ width: '100%', padding: '4px' }} required />
          </div>
          <button type="submit" style={{ cursor: 'pointer', padding: '6px' }}>Guardar</button>
        </form>
      </div>

      {/* TASAS */}
      <div>
        <h4>Registrar Tipo de Cambio</h4>
        <form onSubmit={handleTasa} style={{ display: 'flex', flexDirection: 'column', width: '250px', gap: '8px' }}>
          <div>
            <label style={{ display: 'block', fontWeight: 'bold' }}>Moneda Origen:</label>
            <select value={tasaForm.monedaOrigen} onChange={e => setTasaForm({ ...tasaForm, monedaOrigen: e.target.value })} style={{ width: '100%', padding: '4px' }} required>
              <option value="">-- Seleccionar --</option>
              {monedas.map(m => <option key={m.codigo} value={m.codigo}>{m.nombre}</option>)}
            </select>
          </div>
          <div>
            <label style={{ display: 'block', fontWeight: 'bold' }}>Moneda Destino:</label>
            <select value={tasaForm.monedaDestino} onChange={e => setTasaForm({ ...tasaForm, monedaDestino: e.target.value })} style={{ width: '100%', padding: '4px' }} required>
              <option value="">-- Seleccionar --</option>
              {monedas.map(m => <option key={m.codigo} value={m.codigo}>{m.nombre}</option>)}
            </select>
          </div>
          <div>
            <label style={{ display: 'block', fontWeight: 'bold' }}>Tasa de Conversión:</label>
            <input type="number" step="0.0001" min="0.0001" value={tasaForm.tasa} onChange={e => setTasaForm({ ...tasaForm, tasa: e.target.value })} style={{ width: '100%', padding: '4px' }} placeholder="1.0000" required />
          </div>
          <button type="submit" style={{ cursor: 'pointer', padding: '6px' }}>Guardar</button>
        </form>
      </div>
    </div>
  );
}

export default Configuracion;
