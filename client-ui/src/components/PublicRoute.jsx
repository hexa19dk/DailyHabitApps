import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';

const PublicRoute = ({ children, redirectTo = '/dashboard' }) => {
  const { user, initializing } = useAuth();
  const location = useLocation();

  if (initializing) {
    return null; // or a loading spinner
  }

  if (user) {
    // If user is already authenticated, redirect them to the intended destination
    const from = location.state?.from?.pathname || redirectTo;
    return <Navigate to={from} replace />;
  }

  return children;
};

export default PublicRoute;
