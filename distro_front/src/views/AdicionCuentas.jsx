import React, { useState } from 'react';
import { useCuentas } from '../viewmodels/useCuentas';
import { useMaestros } from '../viewmodels/useMaestros';

const AdicionCuentas = () => {
  const { agregarCuenta, loading } = useCuentas();
  const { monedas } = useMaestros(); // Traemos las monedas de la base de datos
  
  const [form, setForm] = useState({ nroCuenta: '', moneda: '', tipo: 'AHO', nombre: '' });
  const [msg, setMsg] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!form.moneda) {
      setMsg({ t: 'error', txt: 'Por favor seleccione una moneda' });
      return;
    }
    
    const res = await agregarCuenta(form);
    if (res.success) {
      setMsg({ t: 'success', txt: 'Cuenta registrada con éxito' });
      setForm({ nroCuenta: '', moneda: '', tipo: 'AHO', nombre: '' });
    } else {
      setMsg({ t: 'error', txt: res.message });
    }
  };

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2 style={styles.title}>Adición de Cuentas</h2>
        <form onSubmit={handleSubmit} style={styles.form}>
          
          <div style={styles.field}>
            <label style={styles.label}>Número de Cuenta:</label>
            <input 
              style={styles.input} 
              placeholder="Ej: 3015002341220"
              type="text" 
              value={form.nroCuenta} 
              onChange={e => setForm({...form, nroCuenta: e.target.value})} 
              required 
            />
          </div>

          <div style={styles.field}>
            <label style={styles.label}>Moneda:</label>
            <select 
              style={styles.input} 
              value={form.moneda} 
              onChange={e => setForm({...form, moneda: e.target.value})}
              required
            >
              <option value="">Seleccione moneda...</option>
              {monedas.map(m => (
                <option key={m.codigo} value={m.codigo}>
                  {m.codigo} - {m.nombre}
                </option>
              ))}
            </select>
          </div>

          <div style={styles.field}>
            <label style={styles.label}>Tipo de Cuenta:</label>
            <select 
              style={styles.input} 
              value={form.tipo} 
              onChange={e => setForm({...form, tipo: e.target.value})}
            >
              <option value="AHO">CUENTA DE AHORROS (AHO)</option>
              <option value="CTE">CUENTA CORRIENTE (CTE)</option>
            </select>
          </div>

          <div style={styles.field}>
            <label style={styles.label}>Titular de la Cuenta:</label>
            <input 
              style={styles.input} 
              placeholder="Nombre completo"
              type="text" 
              value={form.nombre} 
              onChange={e => setForm({...form, nombre: e.target.value})} 
              required 
            />
          </div>

          <div style={styles.actions}>
            <button type="submit" disabled={loading} style={styles.btnOk}>
              {loading ? 'REGISTRANDO...' : 'ACEPTAR'}
            </button>
            <button 
              type="button" 
              onClick={() => { setForm({ nroCuenta: '', moneda: '', tipo: 'AHO', nombre: '' }); setMsg(null); }} 
              style={styles.btnCancel}
            >
              CANCELAR
            </button>
          </div>
        </form>
        
        {msg && (
          <div style={{
            ...styles.alert, 
            backgroundColor: msg.t === 'success' ? '#d4edda' : '#f8d7da',
            color: msg.t === 'success' ? '#155724' : '#721c24'
          }}>
            {msg.txt}
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
    maxWidth: '450px', 
    backgroundColor: '#fff', 
    borderRadius: '12px', 
    padding: '30px', 
    boxShadow: '0 4px 15px rgba(0,0,0,0.1)',
    borderTop: '5px solid #ff6600'
  },
  title: { color: '#003366', textAlign: 'center', marginBottom: '25px', fontSize: '1.5rem' },
  form: { display: 'flex', flexDirection: 'column', gap: '15px' },
  field: { display: 'flex', flexDirection: 'column', gap: '5px' },
  label: { fontWeight: 'bold', color: '#555', fontSize: '0.9rem' },
  input: { 
    padding: '10px', 
    borderRadius: '6px', 
    border: '1px solid #ccc', 
    fontSize: '1rem',
    outlineColor: '#ff6600'
  },
  actions: { display: 'flex', justifyContent: 'space-between', marginTop: '20px' },
  btnOk: { 
    flex: 1, 
    marginRight: '10px', 
    padding: '12px', 
    backgroundColor: '#003366', 
    color: '#fff', 
    border: 'none', 
    borderRadius: '6px', 
    cursor: 'pointer', 
    fontWeight: 'bold' 
  },
  btnCancel: { 
    flex: 1, 
    padding: '12px', 
    backgroundColor: '#e0e0e0', 
    color: '#333', 
    border: 'none', 
    borderRadius: '6px', 
    cursor: 'pointer', 
    fontWeight: 'bold' 
  },
  alert: { marginTop: '20px', padding: '10px', borderRadius: '6px', textAlign: 'center', fontWeight: 'bold' }
};

export default AdicionCuentas;
