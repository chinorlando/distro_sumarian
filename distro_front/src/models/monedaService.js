import apiClient from '../api/apiClient';

export const getMonedas = async () => {
  const response = await apiClient.get('/Monedas');
  return response.data;
};

export const postMoneda = async (moneda) => {
  const response = await apiClient.post('/Monedas', moneda);
  return response.data;
};
