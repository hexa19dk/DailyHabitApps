import React, { createContext, useContext, useEffect, useMemo, useState } from 'react';
import api from '../api/axiosInstance';
import { getCookie, deleteCookie } from "../utils/cookieUtils";
import { isTokenExpired, refreshAccessToken, storeTokens, buildUserProfile } from "../utils/tokenUtils";

const AuthContext = createContext(undefined);

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};

export const AuthProvider = ({ children }) => {
  const [user, setUser] = useState(null);
  const [initializing, setInitializing] = useState(true);

  useEffect(() => {
    const loadUser = async () => {
      const accessToken = getCookie("accessToken");
      const refreshToken = getCookie("refreshToken");

      if (accessToken) {
        if (!isTokenExpired(accessToken)) {
          setUser(buildUserProfile(accessToken));
        } else if (refreshToken) {
          const newToken = await refreshAccessToken();
          if (!newToken) 
            setUser(null);
        } else {
          setUser(null);
        }
      } else {
        setUser(null);
      }

      setInitializing(false);
    };

    loadUser();
  }, []);

  const login = React.useCallback(async (username, password) => {
    try {
      const res = await api.post('/Auth/login', { username, password });
      const accessToken = res.data.result.accessToken;
      const refreshToken = res.data.result.refreshToken;

      if (!accessToken || !refreshToken) {
        throw new Error("Login response missing tokens");
      }

      storeTokens(accessToken, refreshToken);
      const profile = buildUserProfile(accessToken);
      setUser(profile);

      return profile;
    } catch (error) {
      console.error('Authentication failed: ' + error);
      throw error;
    }
  }, []);

  const register = async (payload) => {
    try {
      const res = await api.post('/Auth/register', payload);
      return res.data;
    } catch (error) {
      console.error('registration failed: ' + error);
      throw error;
    }
  };

  const requestPasswordReset = async (email) => {
    try {
      await api.post('/Auth/forgot-password', { email });
    } catch (error) {
      console.error('Password reset failed: ' + error);
      throw error;
    }
  };

  const logout = () => {
    deleteCookie('accessToken');
    deleteCookie('refreshToken');
    setUser(null);
  };

  const value = useMemo(() => ({ user, initializing, login, register, requestPasswordReset, logout }), [user, initializing, login]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};


