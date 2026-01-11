import React, { useState } from 'react';
import { Box, Button, Stack, Typography } from '@mui/material';
import CustomTextField from '../../../components/forms/theme-elements/CustomTextField.jsx';
import { useAuth } from '../../../context/AuthContext';

const ForgotPassword = () => {
  const { requestPasswordReset } = useAuth();
  const [email, setEmail] = useState('');
  const [submitting, setSubmitting] = useState(false);
  const [sent, setSent] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await requestPasswordReset(email);
      setSent(true);
    } catch (err) {
      alert(err?.response?.data?.message || 'Failed to send reset email');
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <Stack>
        <Box>
          <Typography variant="subtitle1" fontWeight={600} component="label" htmlFor='email' mb="5px">Email</Typography>
          <CustomTextField id="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} variant="outlined" fullWidth required/>
        </Box>
      </Stack>
      <Box mt={2}>
        <Button disabled={submitting} color="primary" variant="contained" size="large" fullWidth type="submit">
          {sent ? 'Email Sent' : 'Send Reset Link'}
        </Button>
      </Box>
    </form>
  );
};

export default ForgotPassword;


