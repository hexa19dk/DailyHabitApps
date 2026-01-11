import './App.css'
import { CssBaseline, ThemeProvider } from '@mui/material';
import { baselightTheme } from './theme/DefaultColors';
import { RouterProvider } from 'react-router';
import router from './routes/Router'
import { HabitProvider } from './context/HabitContext';
import { AuthProvider } from './context/AuthContext';
import { SnackbarProvider } from './context/SnackbarContext';
import { HabitTrackingProvider } from './context/HabitTrackingContext';


function App() {
  const theme = baselightTheme;

  return (
    <ThemeProvider theme={theme}>
      <CssBaseline />
      <SnackbarProvider>
        <AuthProvider>
          <HabitProvider>
            <HabitTrackingProvider>
              <RouterProvider router={router} />
            </HabitTrackingProvider>
          </HabitProvider>
        </AuthProvider>
      </SnackbarProvider>
    </ThemeProvider>
  )
}

export default App
