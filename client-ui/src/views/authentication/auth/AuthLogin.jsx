import React, { useEffect, useState } from 'react';
import {
    Box,
    Typography,
    FormGroup,
    FormControlLabel,
    Button,
    Stack,
    Checkbox
} from '@mui/material';
import { Link, useNavigate, useLocation } from 'react-router-dom';

import CustomTextField from '../../../components/forms/theme-elements/CustomTextField.jsx';
import { useAuth } from '../../../context/AuthContext';
import { useSnackbar } from '../../../context/SnackbarContext';

const AuthLogin = ({ title, subtitle, subtext }) => {
    const navigate = useNavigate();
    const location = useLocation();
    const { login, user, initializing } = useAuth();
    const { showError, showSuccess } = useSnackbar();
    const [usernameOrEmail, setUsernameOrEmail] = useState('');
    const [password, setPassword] = useState('');
    const [submitting, setSubmitting] = useState(false);

    useEffect(() => {
        if (!initializing && user) {
            // If already logged in, redirect to dashboard
            const from = location.state?.from?.pathname || '/dashboard';
            navigate(from, { replace: true });
        }
    }, [user, initializing, navigate, location]);

    const handleSubmit = async (e) => {
        e.preventDefault();
        setSubmitting(true);
        try {
            await login(usernameOrEmail, password);
            showSuccess('Login successful!');

            navigate('/dashboard', { replace: true, state: {} });
        } catch (err) {
            const errorMessage = err?.response?.data?.message || 'Invalid credentials. Please try again.';
            showError(errorMessage);
        } finally {
            setSubmitting(false);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            {title ? (
                <Typography fontWeight="700" variant="h2" mb={1}>
                    {title}
                </Typography>
            ) : null}

            {subtext}

            <Stack>
                <Box>
                    <Typography variant="subtitle1"
                        fontWeight={600} component="label" htmlFor='username' mb="5px">Username or Email</Typography>
                    <CustomTextField id="username" value={usernameOrEmail} onChange={(e) => setUsernameOrEmail(e.target.value)} variant="outlined" fullWidth required />
                </Box>
                <Box mt="25px">
                    <Typography variant="subtitle1"
                        fontWeight={600} component="label" htmlFor='password' mb="5px" >Password</Typography>
                    <CustomTextField id="password" value={password} onChange={(e) => setPassword(e.target.value)} type="password" variant="outlined" fullWidth required />
                </Box>
                <Stack justifyContent="space-between" direction="row" alignItems="center" my={2}>
                    <FormGroup>
                        <FormControlLabel
                            control={<Checkbox defaultChecked />}
                            label="Remember this Device"
                        />
                    </FormGroup>
                    <Typography
                        component={Link}
                        to="/auth/forgot-password"
                        fontWeight="500"
                        sx={{
                            textDecoration: 'none',
                            color: 'primary.main',
                        }}
                    >
                        Forgot Password ?
                    </Typography>
                </Stack>
            </Stack>
            <Box>
                <Button
                    disabled={submitting}
                    color="primary"
                    variant="contained"
                    size="large"
                    fullWidth
                    type="submit"
                >
                    Sign In
                </Button>
            </Box>
            {subtitle}
        </form>
    )
};

export default AuthLogin;
