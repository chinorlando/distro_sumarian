import { useState, useEffect } from 'react';
import * as monedaService from '../models/monedaService';
import * as tipoCambioService from '../models/tipoCambioService';

export const useMaestros = () => {
  const [monedas, setMonedas] = useState([]);
  const [loading, setLoading] = useState(false);
  const [mensaje, setMensaje] = useState(null);

  const fetchMonedas = async () => {
    try {
      const data = await monedaService.getMonedas();
      setMonedas(data);
    } catch (e) { console.error(e); }
  };

  const agregarMoneda = async (moneda) => {
    setLoading(true);
    try {
      await monedaService.postMoneda(moneda);
      setMensaje({ tipo: 'success', texto: "Moneda registrada" });
      await fetchMonedas();
    } catch (e) { setMensaje({ tipo: 'error', texto: "Error al registrar moneda" }); }
    finally { setLoading(false); }
  };

  const agregarTipoCambio = async (tc) => {
    setLoading(true);
    try {
      await tipoCambioService.registrarTipoCambio(tc);
      setMensaje({ tipo: 'success', texto: "Tipo de cambio registrado" });
    } catch (e) { setMensaje({ tipo: 'error', texto: "Error al registrar tipo de cambio" }); }
    finally { setLoading(false); }
  };

  useEffect(() => { fetchMonedas(); }, []);

  return { monedas, loading, mensaje, agregarMoneda, agregarTipoCambio };
};
