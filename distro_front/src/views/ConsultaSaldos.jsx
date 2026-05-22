import React, { useState, useEffect } from 'react';
import { useCuentas } from '../viewmodels/useCuentas';
import { useMaestros } from '../viewmodels/useMaestros';
import * as movimientoService from '../models/movimientoService';

const ConsultaSaldos = () => {
  const { cuentas, loading, error } = useCuentas();
  const { monedas } = useMaestros(); // Traemos las monedas para los nombres
  const [historial, setHistorial] = useState(null);
  const [cuentaSel, setCuentaSel] = useState(null);

  // Helper para obtener el símbolo de la moneda
  const getMonedaSimbolo = (codigo) => {
    const m = monedas.find(m => m.codigo === codigo);
    return m ? m.simbolo : codigo;
  };

  const verMovimientos = async (cuenta) => {
    try {
      const data = await movimientoService.getHistorialByCuenta(cuenta.nroCuenta);
      setHistorial(data);
      setCuentaSel(cuenta);
    } catch (err) {
      alert("No hay movimientos para esta cuenta");
    }
  };

  if (loading) return <p>Cargando saldos...</p>;
  if (error) return <p style={{color:'red'}}>{error}</p>;

  return (
    <div style={{padding: 20}}>
      <h2>Consulta de Saldos</h2>
      <table style={styles.table}>
        <thead>
          <tr style={styles.header}>
            <th>Tipo</th>
            <th>Moneda</th>
            <th>Cuenta</th>
            <th>Titular</th>
            <th>Saldo</th>
            <th>Mov.</th>
          </tr>
        </thead>
        <tbody>
          {cuentas.map(c => (
            <tr key={c.nroCuenta} style={styles.row}>
              <td>{c.tipo}</td>
              <td>{getMonedaSimbolo(c.moneda)}</td>
              <td>{c.nroCuenta}</td>
              <td>{c.nombre}</td>
              <td style={{textAlign:'right'}}>{c.saldo} {getMonedaSimbolo(c.moneda)}</td>
              <td style={{textAlign:'center'}}>
                <button onClick={() => verMovimientos(c)} style={styles.btnMov}>📄</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {historial && (
        <div style={styles.historialBox}>
          <h3>Movimientos: {cuentaSel?.nroCuenta}</h3>
          <p>Titular: {cuentaSel?.nombre} | Saldo: {cuentaSel?.saldo} {getMonedaSimbolo(cuentaSel?.moneda)}</p>
          <table style={styles.table}>
            <thead>
              <tr style={{backgroundColor:'#eee'}}>
                <th>Fecha</th>
                <th>Tipo</th>
                <th>Importe</th>
              </tr>
            </thead>
            <tbody>
              {historial.map((m, i) => (
                <tr key={i}>
                  <td>{new Date(m.fecha).toLocaleString()}</td>
                  <td style={{color: m.tipo === 'A' ? 'green' : 'red'}}>{m.tipo === 'A' ? 'Abono' : 'Débito'}</td>
                  <td style={{textAlign:'right'}}>{m.importe} {getMonedaSimbolo(cuentaSel?.moneda)}</td>
                </tr>
              ))}
            </tbody>
          </table>
          <button onClick={() => setHistorial(null)} style={{marginTop:10}}>Cerrar Historial</button>
        </div>
      )}
    </div>
  );
};

const styles = {
  table: { width: '100%', borderCollapse: 'collapse', marginTop: 10 },
  header: { backgroundColor: '#003366', color: 'white' },
  row: { borderBottom: '1px solid #ccc' },
  btnMov: { cursor: 'pointer', background: 'none', border: 'none', fontSize: '1.2em' },
  historialBox: { marginTop: 30, padding: 20, border: '2px solid #003366', borderRadius: 8, backgroundColor: '#f0f4f8' }
};

export default ConsultaSaldos;
