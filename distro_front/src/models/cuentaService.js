import apiClient from '../api/apiClient';

export const getCuentas = async () => {
  const response = await apiClient.get('/Cuentas');
  return response.data;
};

export const getCuentaById = async (id) => {
  const response = await apiClient.get(`/Cuentas/${id}`);
  return response.data;
};

export const registrarCuenta = async (cuenta) => {
  const response = await apiClient.post('/Cuentas', cuenta);
  return response.data;
};

export const realizarTransferencia = async (transferencia) => {
  const response = await apiClient.post('/Cuentas/transferencia', transferencia);
  return response.data;
};
