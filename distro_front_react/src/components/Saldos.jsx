import React, { useState, useEffect } from 'react';
import { API_URL } from '../config';

function Saldos() {
  const [cuentas, setCuentas] = useState([]);
  const [historial, setHistorial] = useState([]);
  const [ctaSel, setCtaSel] = useState(null);

  const cargarCuentas = () => {
    fetch(`${API_URL}/Cuentas`)
      .then(r => r.json())
      .then(setCuentas)
      .catch(console.error);
  };

  useEffect(() => {
    cargarCuentas();
  }, []);

  const verHistorial = (cuenta) => {
    setCtaSel(cuenta);
    fetch(`${API_URL}/Movimientos/cuenta/${cuenta.nroCuenta}`, { method: 'GET' })
      .then(r => r.status === 404 ? [] : r.json())
      .then(setHistorial)
      .catch(() => setHistorial([]));
  };

  return (
    <div>
      <h3>4) Consulta de Saldos</h3>
      <table border="1" cellPadding="6" style={{ width: '100%', borderCollapse: 'collapse', marginTop: '10px' }}>
        <thead style={{ backgroundColor: '#f2f2f2' }}>
          <tr>
            <th>Tipo</th>
            <th>Moneda</th>
            <th>Cuenta</th>
            <th>Titular</th>
            <th>Saldo</th>
            <th style={{ width: '50px', textAlign: 'center' }}>Mov.</th>
          </tr>
        </thead>
        <tbody>
          {cuentas.map(c => (
            <tr key={c.nroCuenta}>
              <td>{c.tipo}</td>
              <td>{c.moneda}</td>
              <td>{c.nroCuenta}</td>
              <td>{c.nombre}</td>
              <td style={{ textAlign: 'right' }}>{c.saldo.toFixed(2)}</td>
              <td style={{ textAlign: 'center' }}>
                <button onClick={() => verHistorial(c)} style={{ cursor: 'pointer' }}>👉</button>
              </td>
            </tr>
          ))}
          {cuentas.length === 0 && (
            <tr>
              <td colSpan="6" style={{ textAlign: 'center' }}>No existen cuentas registradas</td>
            </tr>
          )}
        </tbody>
      </table>

      {ctaSel && (
        <div style={{ marginTop: '30px', padding: '20px', border: '1px solid #ccc', backgroundColor: '#f9f9f9' }}>
          <h4>5) Movimientos: {ctaSel.nroCuenta}</h4>
          <p><strong>Titular:</strong> {ctaSel.nombre} | <strong>Saldo:</strong> {ctaSel.saldo.toFixed(2)} {ctaSel.moneda}</p>
          <table border="1" cellPadding="6" style={{ width: '100%', borderCollapse: 'collapse', marginTop: '10px' }}>
            <thead style={{ backgroundColor: '#eee' }}>
              <tr>
                <th>Fecha</th>
                <th>Tipo</th>
                <th style={{ textAlign: 'right' }}>Importe</th>
              </tr>
            </thead>
            <tbody>
              {historial.map((m, idx) => (
                <tr key={idx}>
                  <td>{new Date(m.fecha).toLocaleString()}</td>
                  <td style={{ color: m.tipo === 'A' ? 'green' : 'red', fontWeight: 'bold' }}>
                    {m.tipo === 'A' ? 'Abono' : 'Débito'}
                  </td>
                  <td style={{ textAlign: 'right', color: m.tipo === 'D' ? 'red' : 'green', fontWeight: 'bold' }}>
                    {m.tipo === 'D' ? '-' : ''}{m.import.toFixed(2)}
                  </td>
                </tr>
              ))}
              {historial.length === 0 && (
                <tr>
                  <td colSpan="3" style={{ textAlign: 'center' }}>Sin movimientos a la fecha</td>
                </tr>
              )}
            </tbody>
          </table>
          <button style={{ marginTop: '15px', padding: '5px 10px', cursor: 'pointer' }} onClick={() => setCtaSel(null)}>
            Cerrar Historial
          </button>
        </div>
      )}
    </div>
  );
}

export default Saldos;
