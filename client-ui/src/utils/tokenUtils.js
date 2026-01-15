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
  if (accessToken) {
    try {
      const decoded = jwtDecode(accessToken);
      const expiresIn = Math.floor(decoded.exp - Date.now() / 1000);
      setCookie("accessToken", accessToken, {
        path: "/",
        secure: true,
        sameSite: "strict",
        maxAge: expiresIn,
      });
    } catch {
      setCookie("accessToken", accessToken, { maxAge: 900 }); // 15 min fallback
    }
  }

  if (refreshToken) {
    try {
      const decoded = jwtDecode(refreshToken);
      const expiresIn = Math.floor(decoded.exp - Date.now() / 1000);
      setCookie("refreshToken", refreshToken, {
        path: "/",
        secure: true,
        sameSite: "strict",
        maxAge: expiresIn,
      });
    } catch {
      setCookie("refreshToken", refreshToken, { maxAge: 604800 }); // 7 days fallback
    }
  }
};

// Build user profile from accessToken
export const buildUserProfile = (accessToken) => {
  try {
    const decoded = jwtDecode(accessToken);
    const { ...claims } = decoded;
    return claims;
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
    const newAccessToken = res.data?.accessToken;
    const newRefreshToken = res.data.refreshToken || refreshToken;

    if (!newAccessToken) return null;

    storeTokens(newAccessToken, newRefreshToken);
    return newAccessToken;
  } catch (err) {
    console.error("Failed to refresh token:", err);
    deleteCookie("accessToken");
    deleteCookie("refreshToken");

    return null;
  }
};
