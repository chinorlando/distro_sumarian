import apiClient from '../api/apiClient';

export const getTipoCambios = async () => {
  const response = await apiClient.get('/TipoCambios');
  return response.data;
};

export const registrarTipoCambio = async (tc) => {
  const response = await apiClient.post('/TipoCambios', tc);
  return response.data;
};
