import React, { useState } from 'react';
import { useCuentas } from '../viewmodels/useCuentas';
import { useOperaciones } from '../viewmodels/useOperaciones';
import { useMaestros } from '../viewmodels/useMaestros';

const Operaciones = () => {
  const { cuentas, refresh } = useCuentas();
  const { ejecutarOperacion, loading, mensaje } = useOperaciones();
  const { monedas } = useMaestros(); // Traemos las monedas para obtener sus nombres
  
  const [form, setForm] = useState({ nroCuenta: '', tipo: 'A', importe: 0 });
  const [showConfirm, setShowConfirm] = useState(false);

  // Helper para obtener el nombre de la moneda a partir del código
  const getMonedaNombre = (codigo) => {
    const m = monedas.find(m => m.codigo === codigo);
    return m ? m.nombre.toUpperCase() : codigo;
  };

  // Prepara la confirmación
  const openConfirm = (tipo) => {
    if (!form.nroCuenta || form.importe <= 0) {
      alert("Por favor seleccione una cuenta e ingrese un monto válido.");
      return;
    }
    setForm({ ...form, tipo });
    setShowConfirm(true);
  };

  // Ejecuta la operación final
  const handleConfirm = async () => {
    const success = await ejecutarOperacion(form);
    if (success) {
      setForm({ ...form, importe: 0 });
      refresh();
      setShowConfirm(false);
    }
  };

  const selectedAccount = cuentas.find(c => c.nroCuenta === form.nroCuenta);

  return (
    <div style={styles.container}>
      <div style={styles.card}>
        <h2 style={styles.title}>Operaciones de Cuenta</h2>
        
        {!showConfirm ? (
          <div style={styles.form}>
            <div style={styles.field}>
              <label style={styles.label}>Seleccione Cuenta:</label>
              <select 
                style={styles.input} 
                value={form.nroCuenta} 
                onChange={e => setForm({...form, nroCuenta: e.target.value})}
              >
                <option value="">Seleccione una cuenta...</option>
                {cuentas.map(c => (
                  <option key={c.nroCuenta} value={c.nroCuenta}>
                    {c.nroCuenta} - {c.nombre}
                  </option>
                ))}
              </select>
              {selectedAccount && (
                <small style={styles.saldoInfo}>
                  Saldo actual: {selectedAccount.saldo} {getMonedaNombre(selectedAccount.moneda)}
                </small>
              )}
            </div>

            <div style={styles.field}>
              <label style={styles.label}>Monto de la Operación:</label>
              <input 
                style={styles.input} 
                type="number" 
                step="0.01" 
                placeholder="0.00"
                value={form.importe} 
                onChange={e => setForm({...form, importe: parseFloat(e.target.value) || 0})} 
              />
            </div>

            <div style={styles.actions}>
              <button 
                onClick={() => openConfirm('A')} 
                disabled={loading} 
                style={{...styles.btn, backgroundColor: '#28a745', color: 'white'}}
              >
                DEPÓSITO (ABONO)
              </button>
              <button 
                onClick={() => openConfirm('D')} 
                disabled={loading} 
                style={{...styles.btn, backgroundColor: '#dc3545', color: 'white'}}
              >
                RETIRO (DÉBITO)
              </button>
            </div>
          </div>
        ) : (
          <div style={styles.confirmBox}>
            <h3 style={{color: '#003366'}}>¿Confirmar Operación?</h3>
            <p><strong>Cuenta:</strong> {form.nroCuenta}</p>
            <p><strong>Tipo:</strong> {form.tipo === 'A' ? 'DEPÓSITO' : 'RETIRO'}</p>
            <p><strong>Monto:</strong> <span style={{fontSize: '1.2em', color: form.tipo === 'A' ? 'green' : 'red'}}>
              {form.importe} {getMonedaNombre(selectedAccount?.moneda)}
            </span></p>
            
            <div style={styles.actions}>
              <button onClick={handleConfirm} disabled={loading} style={styles.btnOk}>CONFIRMAR</button>
              <button onClick={() => setShowConfirm(false)} style={styles.btnCancel}>CANCELAR</button>
            </div>
          </div>
        )}

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
    maxWidth: '450px', 
    backgroundColor: '#fff', 
    borderRadius: '12px', 
    padding: '30px', 
    boxShadow: '0 4px 15px rgba(0,0,0,0.1)',
    borderTop: '5px solid #003366'
  },
  title: { color: '#003366', textAlign: 'center', marginBottom: '25px' },
  form: { display: 'flex', flexDirection: 'column', gap: '20px' },
  field: { display: 'flex', flexDirection: 'column', gap: '5px' },
  label: { fontWeight: 'bold', color: '#555' },
  input: { padding: '10px', borderRadius: '6px', border: '1px solid #ccc', fontSize: '1rem' },
  saldoInfo: { color: '#ff6600', fontWeight: 'bold', marginTop: '5px' },
  actions: { display: 'flex', justifyContent: 'center', gap: '15px', marginTop: '10px' },
  btn: { flex: 1, padding: '12px', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' },
  confirmBox: { textAlign: 'center', padding: '10px', backgroundColor: '#f8f9fa', borderRadius: '8px', border: '1px dashed #003366' },
  btnOk: { padding: '10px 20px', backgroundColor: '#003366', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' },
  btnCancel: { padding: '10px 20px', backgroundColor: '#6c757d', color: 'white', border: 'none', borderRadius: '6px', cursor: 'pointer', fontWeight: 'bold' },
  alert: { marginTop: '20px', padding: '10px', borderRadius: '6px', textAlign: 'center', fontWeight: 'bold' }
};

export default Operaciones;
