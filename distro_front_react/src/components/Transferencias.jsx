import React, { useState, useEffect } from 'react';
import { API_URL } from '../config';

function Transferencias() {
  const [cuentas, setCuentas] = useState([]);
  const [form, setForm] = useState({ cuentaOrigen: '', cuentaDestino: '', monto: '' });

  useEffect(() => {
    fetch(`${API_URL}/Cuentas`)
      .then(r => r.json())
      .then(setCuentas)
      .catch(console.error);
  }, []);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (form.cuentaOrigen === form.cuentaDestino) return alert("Las cuentas origen y destino deben ser diferentes");

    fetch(`${API_URL}/Cuentas/transferencia`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        cuentaOrigen: form.cuentaOrigen,
        cuentaDestino: form.cuentaDestino,
        monto: parseFloat(form.monto)
      })
    }).then(async r => {
      if (!r.ok) {
        const text = await r.text();
        throw new Error(text || "Error al transferir");
      }
      setForm({ cuentaOrigen: '', cuentaDestino: '', monto: '' });
      alert("Transferencia realizada con éxito");
    }).catch(err => alert(err.message));
  };

  return (
    <div>
      <h3>3) Transferencias entre cuentas</h3>
      <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', width: '320px', gap: '10px', marginTop: '15px' }}>
        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Cuenta Origen:</label>
          <select value={form.cuentaOrigen} onChange={e => setForm({ ...form, cuentaOrigen: e.target.value })} style={{ width: '100%', padding: '6px' }} required>
            <option value="">-- Seleccionar --</option>
            {cuentas.map(c => <option key={c.nroCuenta} value={c.nroCuenta}>{c.nroCuenta} - {c.nombre}</option>)}
          </select>
        </div>

        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Cuenta Destino:</label>
          <select value={form.cuentaDestino} onChange={e => setForm({ ...form, cuentaDestino: e.target.value })} style={{ width: '100%', padding: '6px' }} required>
            <option value="">-- Seleccionar --</option>
            {cuentas.map(c => <option key={c.nroCuenta} value={c.nroCuenta}>{c.nroCuenta} - {c.nombre}</option>)}
          </select>
        </div>

        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Monto a Transferir:</label>
          <input type="number" step="0.01" min="0.01" value={form.monto} onChange={e => setForm({ ...form, monto: e.target.value })} style={{ width: '100%', padding: '6px' }} placeholder="0.00" required />
        </div>

        <button type="submit" style={{ marginTop: '10px', padding: '8px', cursor: 'pointer', backgroundColor: '#002D72', color: 'white', border: 'none', fontWeight: 'bold' }}>
          ACEPTAR
        </button>
      </form>
    </div>
  );
}

export default Transferencias;
