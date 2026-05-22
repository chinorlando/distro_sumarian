import axios from 'axios';

const apiClient = axios.create({
  baseURL: 'http://localhost:5239/api', // URL actualizada según tu ejecución actual
  headers: {
    'Content-Type': 'application/json'
  }
});

export default apiClient;
