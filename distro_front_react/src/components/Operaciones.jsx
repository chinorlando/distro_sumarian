import React, { useState, useEffect } from 'react';
import { API_URL } from '../config';

function Operaciones() {
  const [cuentas, setCuentas] = useState([]);
  const [form, setForm] = useState({ nroCuenta: '', import: '' });

  useEffect(() => {
    fetch(`${API_URL}/Cuentas`)
      .then(r => r.json())
      .then(setCuentas)
      .catch(console.error);
  }, []);

  const handleOperacion = (tipoOp) => {
    if (!form.nroCuenta || !form.import) return alert("Complete los campos requeridos");
    
    fetch(`${API_URL}/Movimientos`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({
        nroCuenta: form.nroCuenta,
        import: parseFloat(form.import),
        tipo: tipoOp,
        fecha: new Date().toISOString()
      })
    }).then(async r => {
      const data = await r.json();
      if (!r.ok) throw new Error(data.message || data.detalle || "Error en operación");
      
      setForm({ nroCuenta: '', import: '' });
      alert("Operación realizada con éxito");
    }).catch(err => alert(err.message));
  };

  return (
    <div>
      <h3>2) Abonos / Retiros</h3>
      <div style={{ display: 'flex', flexDirection: 'column', width: '320px', gap: '10px', marginTop: '15px' }}>
        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Cuenta:</label>
          <select value={form.nroCuenta} onChange={e => setForm({ ...form, nroCuenta: e.target.value })} style={{ width: '100%', padding: '6px' }}>
            <option value="">-- Seleccione Cuenta --</option>
            {cuentas.map(c => <option key={c.nroCuenta} value={c.nroCuenta}>{c.nroCuenta} - {c.nombre}</option>)}
          </select>
        </div>

        <div>
          <label style={{ display: 'block', fontWeight: 'bold' }}>Monto:</label>
          <input type="number" step="0.01" min="0.01" value={form.import} onChange={e => setForm({ ...form, import: e.target.value })} style={{ width: '100%', padding: '6px' }} placeholder="0.00" />
        </div>

        <div style={{ display: 'flex', gap: '12px', marginTop: '10px' }}>
          <button type="button" onClick={() => handleOperacion('A')} style={{ flex: 1, padding: '8px', cursor: 'pointer', fontWeight: 'bold' }}>DEPOSITO</button>
          <button type="button" onClick={() => handleOperacion('D')} style={{ flex: 1, padding: '8px', cursor: 'pointer', fontWeight: 'bold' }}>RETIRO</button>
        </div>
      </div>
    </div>
  );
}

export default Operaciones;
