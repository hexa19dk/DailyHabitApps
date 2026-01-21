import { jwtDecode } from "jwt-decode";
import { getCookie, setCookie, deleteCookie } from "./cookieUtils";
import api from "../api/axiosInstance";

// Check if token expired
export const isTokenExpired = (token) => {
  try {
    const { exp } = jwtDecode(token);
    return Date.now() >= exp * 1000;
  } catch {
    return true;
  }
};

// Store tokens in cookies with proper expiration
export const storeTokens = (accessToken, refreshToken) => {
  const store = (name, token, fallbackMaxAge) => {
    try {
      const { exp } = jwtDecode(token);
      setCookie(name, token, {
        maxAge: Math.floor(exp - Date.now() / 1000),
      });
    } catch {
      setCookie(name, token, { maxAge: fallbackMaxAge });
    }
  };

  if (accessToken) store("accessToken", accessToken, 900);
  if (refreshToken) store("refreshToken", refreshToken, 604800);
};

// Build user profile from accessToken
export const buildUserProfile = (accessToken) => {
  try {
    const decoded = jwtDecode(accessToken);
    return decoded || null;
  } catch {
    return null;
  }
};

// Refresh access token using refresh token
export const refreshAccessToken = async () => {
  try {
    const refreshToken = getCookie("refreshToken");
    if (!refreshToken) return null;

    const res = await api.post("/Auth/refresh-token", { refreshToken });

    const { accessToken, refreshToken: newRefreshToken } = res.data || {};
    if (!accessToken) return null;

    storeTokens(accessToken, newRefreshToken || refreshToken);
    return accessToken;
  } catch (err) {
    console.error("Failed to refresh token:", err);

    deleteCookie("accessToken");
    deleteCookie("refreshToken");
    return null;
  }
};
