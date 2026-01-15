import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { CircularProgress, Box } from '@mui/material';
import { useAuth } from '../context/AuthContext';

const LoadingFallback = () => (
  <Box
    sx={{
      display: 'flex',
      justifyContent: 'center',
      alignItems: 'center',
      height: '100vh',
      width: '100vw',
    }}
  >
    <CircularProgress />
  </Box>
);

const ProtectedRoute = ({ children, redirectTo = '/auth/login'}) => {
  const { user, initializing } = useAuth();
  const location = useLocation();

  if (initializing) {
    return <LoadingFallback />;
  }

  if (!user) {    
    // Save the attempted location for redirect after login
    return <Navigate to={redirectTo} state={{ from: location }} replace />;
  }

  // BYPASS AUTH: Always render children
  return children;
};

export default ProtectedRoute;
