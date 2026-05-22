import { useState, useEffect } from 'react';
import * as cuentaService from '../models/cuentaService';

export const useCuentas = () => {
  const [cuentas, setCuentas] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const fetchCuentas = async () => {
    setLoading(true);
    try {
      const data = await cuentaService.getCuentas();
      setCuentas(data);
    } catch (err) {
      setError("Error al cargar cuentas");
    } finally {
      setLoading(false);
    }
  };

  const agregarCuenta = async (cuenta) => {
    setLoading(true);
    try {
      await cuentaService.registrarCuenta(cuenta);
      await fetchCuentas();
      return { success: true };
    } catch (err) {
      return { success: false, message: err.response?.data || "Error al registrar" };
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchCuentas();
  }, []);

  return { cuentas, loading, error, agregarCuenta, refresh: fetchCuentas };
};
