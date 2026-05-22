import { useState } from 'react';
import * as movimientoService from '../models/movimientoService';
import * as cuentaService from '../models/cuentaService';

export const useOperaciones = () => {
  const [loading, setLoading] = useState(false);
  const [mensaje, setMensaje] = useState(null);

  const ejecutarOperacion = async (movimiento) => {
    setLoading(true);
    setMensaje(null);
    try {
      const res = await movimientoService.realizarOperacion(movimiento);
      setMensaje({ tipo: 'success', texto: `Éxito: ${res.message}` });
      return true;
    } catch (err) {
      // Capturamos el mensaje del backend (puede ser un string o un objeto)
      const errorMsg = err.response?.data?.error || err.response?.data?.message || err.response?.data || "Error en la operación";
      setMensaje({ tipo: 'error', texto: typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg) });
      return false;
    } finally {
      setLoading(false);
    }
  };

  const ejecutarTransferencia = async (transferencia) => {
    setLoading(true);
    setMensaje(null);
    try {
      const res = await cuentaService.realizarTransferencia(transferencia);
      setMensaje({ tipo: 'success', texto: `Transferencia exitosa: ${res.message}` });
      return true;
    } catch (err) {
      // Capturamos el mensaje del backend (puede ser un string o un objeto)
      const errorMsg = err.response?.data?.error || err.response?.data?.message || err.response?.data || "Error en transferencia";
      setMensaje({ tipo: 'error', texto: typeof errorMsg === 'string' ? errorMsg : JSON.stringify(errorMsg) });
      return false;
    } finally {
      setLoading(false);
    }
  };

  return { ejecutarOperacion, ejecutarTransferencia, loading, mensaje };
};
