import React, { useState } from 'react';
import { useCuentas } from '../viewmodels/useCuentas';
import { useOperaciones } from '../viewmodels/useOperaciones';
import { useMaestros } from '../viewmodels/useMaestros';

const Transferencias = () => {
  const { cuentas, refresh } = useCuentas();
  const { ejecutarTransferencia, loading, mensaje } = useOperaciones();
  const { monedas } = useMaestros();
  
  const [form, setForm] = useState({ cuentaOrigen: '', cuentaDestino: '', monto: 0 });

  // Helper para obtener el nombre de la moneda
  const getMonedaNombre = (codigo) => {
    const m = monedas.find(m => m.codigo === codigo);
    return m ? m.nombre.toUpperCase() : codigo;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (form.cuentaOrigen === form.cuentaDestino) {
      alert("Las cuentas de origen y destino deben ser diferentes.");
      return;
    }
    const success = await ejecutarTransferencia(form);
    if (success) {
      setForm({ cuentaOrigen: '', cuentaDestino: '', monto: 0 });
      refresh();
    }
  };

  const cOrigen = cuentas.find(c => c.nroCuenta === form.cuentaOrigen);
  const cDestino = cuentas.find(c => c.nroCuenta === form.cuentaDestino);

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2 style={styles.title}>Transferencias entre Cuentas</h2>
        
        <form onSubmit={handleSubmit} style={styles.form}>
          {/* CUENTA ORIGEN */}
          <div style={styles.section}>
            <h3 style={styles.sectionTitle}>CUENTA ORIGEN</h3>
            <div style={styles.field}>
              <label style={styles.label}>Cuenta:</label>
              <select 
                style={styles.input} 
                value={form.cuentaOrigen} 
                onChange={e => setForm({...form, cuentaOrigen: e.target.value})}
                required
              >
                <option value="">Seleccione origen...</option>
                {cuentas.map(c => (
                  <option key={c.nroCuenta} value={c.nroCuenta}>
                    {c.nroCuenta} - {c.nombre}
                  </option>
                ))}
              </select>
            </div>
            <div style={styles.infoRow}>
              <span>Saldo Disponible:</span>
              <span style={styles.saldoTxt}>
                {cOrigen ? `${cOrigen.saldo} ${getMonedaNombre(cOrigen.moneda)}` : '0.00'}
              </span>
            </div>
          </div>

          <div style={styles.divider}></div>

          {/* CUENTA DESTINO */}
          <div style={styles.section}>
            <h3 style={styles.sectionTitle}>CUENTA DESTINO</h3>
            <div style={styles.field}>
              <label style={styles.label}>Cuenta:</label>
              <select 
                style={styles.input} 
                value={form.cuentaDestino} 
                onChange={e => setForm({...form, cuentaDestino: e.target.value})}
                required
              >
                <option value="">Seleccione destino...</option>
                {cuentas
                  .filter(c => c.nroCuenta !== form.cuentaOrigen)
                  .map(c => (
                    <option key={c.nroCuenta} value={c.nroCuenta}>
                      {c.nroCuenta} - {c.nombre}
                    </option>
                  ))}
              </select>
            </div>
            {cDestino && (
              <div style={styles.infoRow}>
                <span>Moneda de destino:</span>
                <span style={styles.monedaDestTxt}>{getMonedaNombre(cDestino.moneda)}</span>
              </div>
            )}
          </div>

          {/* MONTO */}
          <div style={styles.field}>
            <label style={styles.label}>Monto a Transferir:</label>
            <input 
              style={styles.input} 
              type="number" 
              step="0.01" 
              placeholder="0.00"
              value={form.monto} 
              onChange={e => setForm({...form, monto: parseFloat(e.target.value) || 0})} 
              required
            />
          </div>

          <div style={styles.actions}>
            <button type="submit" disabled={loading} style={styles.btnOk}>
              {loading ? 'PROCESANDO...' : 'ACEPTAR'}
            </button>
            <button 
              type="button" 
              onClick={() => setForm({cuentaOrigen:'', cuentaDestino:'', monto:0})} 
              style={styles.btnCancel}
            >
              CANCELAR
            </button>
          </div>
        </form>

        {mensaje && (
          <div style={{
            ...styles.alert, 
            backgroundColor: mensaje.tipo === 'success' ? '#d4edda' : '#f8d7da',
            color: mensaje.tipo === 'success' ? '#155724' : '#721c24'
          }}>
            {mensaje.texto}
          </div>
        )}
      </div>
    </div>
  );
};

const styles = {
  container: { display: 'flex', justifyContent: 'center', padding: '20px' },
  card: { 
    width: '100%', 
    maxWidth: '500px', 
    backgroundColor: '#fff', 
    borderRadius: '12px', 
    padding: '30px', 
    boxShadow: '0 4px 15px rgba(0,0,0,0.1)',
    borderTop: '5px solid #ff6600'
  },
  title: { color: '#003366', textAlign: 'center', marginBottom: '25px' },
  form: { display: 'flex', flexDirection: 'column', gap: '15px' },
  section: { backgroundColor: '#fdfdfd', padding: '15px', borderRadius: '8px', border: '1px solid #eee' },
  sectionTitle: { fontSize: '0.9rem', color: '#ff6600', marginTop: 0, marginBottom: '15px', borderBottom: '1px solid #ffe0cc' },
  field: { display: 'flex', flexDirection: 'column', gap: '5px', marginBottom: '10px' },
  label: { fontWeight: 'bold', color: '#555', fontSize: '0.85rem' },
  input: { padding: '10px', borderRadius: '6px', border: '1px solid #ccc', fontSize: '1rem' },
  infoRow: { display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem', marginTop: '5px' },
  saldoTxt: { color: '#ff6600', fontWeight: 'bold' },
  monedaDestTxt: { color: '#003366', fontWeight: 'bold' },
  divider: { height: '1px', backgroundColor: '#eee', margin: '5px 0' },
  actions: { display: 'flex', justifyContent: 'center', gap: '15px', marginTop: '15px' },
  btnOk: { flex: 1, padding: '12px', backgroundColor: '#003366', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' },
  btnCancel: { flex: 1, padding: '12px', backgroundColor: '#e0e0e0', color: '#333', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' },
  alert: { marginTop: '20px', padding: '10px', borderRadius: '6px', textAlign: 'center', fontWeight: 'bold' }
};

export default Transferencias;
