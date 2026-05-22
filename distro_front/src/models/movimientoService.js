import apiClient from '../api/apiClient';

export const getMovimientos = async () => {
  const response = await apiClient.get('/Movimientos');
  return response.data;
};

export const getHistorialByCuenta = async (nroCuenta) => {
  const response = await apiClient.get(`/Movimientos/cuenta/${nroCuenta}`);
  return response.data;
};

export const realizarOperacion = async (movimiento) => {
  const response = await apiClient.post('/Movimientos', movimiento);
  return response.data;
};
