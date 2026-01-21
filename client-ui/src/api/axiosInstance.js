import axios from "axios";
import { getCookie } from "../utils/cookieUtils";
import { isTokenExpired, refreshAccessToken } from "../utils/tokenUtils";

const baseURL = import.meta.env.VITE_API_URL || "/api";

const ANONYMOUS_ENDPOINTS = [
  "/Auth/login",
  "/Auth/register",
  "/Auth/forgot-password",
  "/Auth/refresh-token",
];

const axiosInstance = axios.create({
  baseURL: baseURL,
  headers: {
    "Content-Type": "application/json",
    Accept: "application/json",
  },
  withCredentials: true,
  timeout: 10000,
});

// Refresh token logic
let isRefreshing = false;
let failedQueue = [];

const processQueue = (error, token = null) => {
  failedQueue.forEach((p) => (error ? p.reject(error) : p.resolve(token)));
  failedQueue = [];
};

// ==== Request interceptor to handle CORS preflight ====
axiosInstance.interceptors.request.use(
  (config) => {
    try {
      const isAnonymous = ANONYMOUS_ENDPOINTS.some((endpoint) =>
        config.url?.includes(endpoint),
      );

      if (!isAnonymous) {
        const token = getCookie("accessToken");
        if (token && !isTokenExpired(token)) {
          config.headers.Authorization = `Bearer ${token}`;
        }
      }

      return config;
    } catch (e) {
      console.error("Failed to access from cookies:", e.message);
    }

    return config;
  },
  (error) => Promise.reject(error),
);

axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;

    if (error.response?.status === 401 && !originalRequest._retry) {
      if (isRefreshing) {
        return new Promise((resolve, reject) => {
          failedQueue.push({ resolve, reject });
        }).then((token) => {
          originalRequest.headers.Authorization = `Bearer ${token}`;
          return axiosInstance(originalRequest);
        });
      }

      originalRequest._retry = true;
      isRefreshing = true;

      const newToken = await refreshAccessToken();
      if (newToken) {
        processQueue(null, newToken);
        originalRequest.headers["Authorization"] = `Bearer ${newToken}`;
        isRefreshing = false;
        return axiosInstance(originalRequest);
      }

      processQueue(new Error("Refresh token failed"), null);
      isRefreshing = false;
    }

    return Promise.reject(error);
  },
);

export default axiosInstance;
