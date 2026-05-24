import React, { useState, useEffect } from 'react';
import { API_URL } from '../config';

function RegistroCuentas() {
  const [monedas, setMonedas] = useState([]);
  const [form, setForm] = useState({ nroCuenta: '', tipo: '', moneda: '', nombre: '' });

  useEffect(() => {
    fetch(`${API_URL}/Monedas`)
      .then(r => r.json())
      .then(setMonedas)
      .catch(console.error);
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();
    const len = form.nroCuenta.length;
    if (form.tipo === 'CTE' && len !== 13) return alert("La Cuenta Corriente (CTE) debe tener exactamente 13 dígitos");
    if (form.tipo === 'AHO' && len !== 14) return alert("La Cuenta de Ahorro (AHO) debe tener exactamente 14 dígitos");

    fetch(`${API_URL}/Cuentas`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ ...form, saldo: 0 })
    }).then(async r => {
      if (!r.ok) {
        const err = await r.json();
        throw new Error(err || "Error al crear cuenta");
      }
      setForm({ nroCuenta: '', tipo: '', moneda: '', nombre: '' });
      alert("Cuenta guardada exitosamente");
    }).catch(err => alert(err.message));
  };

  return (
    <div>
      <h3>1) Adición de Cuentas</h3>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', width: '320px', gap: '10px', marginTop: '15px' }}>
        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Número de Cuenta:</label>
          <input type="text" value={form.nroCuenta} onChange={e => setForm({ ...form, nroCuenta: e.target.value })} style={{ width: '100%', padding: '6px' }} placeholder="Corriente 13 dig. / Ahorro 14 dig." required />
        </div>
        
        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Tipo de Cuenta:</label>
          <select value={form.tipo} onChange={e => setForm({ ...form, tipo: e.target.value })} style={{ width: '100%', padding: '6px' }} required>
            <option value="">-- Seleccionar --</option>
            <option value="CTE">Corriente (13 dígitos)</option>
            <option value="AHO">Ahorro (14 dígitos)</option>
          </select>
        </div>

        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Moneda:</label>
          <select value={form.moneda} onChange={e => setForm({ ...form, moneda: e.target.value })} style={{ width: '100%', padding: '6px' }} required>
            <option value="">-- Seleccionar --</option>
            {monedas.map(m => <option key={m.codigo} value={m.codigo}>{m.nombre} ({m.simbolo})</option>)}
          </select>
        </div>

        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Titular / Nombre:</label>
          <input type="text" value={form.nombre} onChange={e => setForm({ ...form, nombre: e.target.value })} style={{ width: '100%', padding: '6px' }} required />
        </div>

        <button type="submit" style={{ marginTop: '10px', padding: '8px', cursor: 'pointer', backgroundColor: '#002D72', color: 'white', border: 'none', fontWeight: 'bold' }}>
          ACEPTAR
        </button>
      </form>
    </div>
  );
}

export default RegistroCuentas;
