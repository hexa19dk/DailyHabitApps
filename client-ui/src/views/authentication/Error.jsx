import { Box, Container, Typography, Button, styled } from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import ErrorImg from '../../assets/images/backgrounds/404-error-idea.gif';

const StyledBox = styled('div')({
  position: 'fixed',
  top: 0,
  left: 0,
  right: 0,
  bottom: 0,
  display: 'flex',
  alignItems: 'center',
  justifyContent: 'center',
  width: '100vw',
  height: '100vh',
  overflow: 'auto'
});

const ContentWrapper = styled('div')({
  display: 'flex',
  flexDirection: 'column',
  alignItems: 'center',
  justifyContent: 'center',
  width: '100%',
  maxWidth: '600px',
  margin: '0 auto',
  padding: '20px'
});

const StyledImage = styled('img')({
  width: '100%',
  maxWidth: '400px',
  height: 'auto',
  display: 'block',
  margin: '0 auto 32px'
});

const Error = () => (
  <StyledBox>
    <ContentWrapper>
      <StyledImage src={ErrorImg} alt="404" />
      <Typography 
        variant="h1" 
        sx={{ 
          fontSize: { xs: '2rem', sm: '3rem', md: '4rem' },
          fontWeight: 600,
          textAlign: 'center',
          mb: 2
        }}
      >
        Oops!!!
      </Typography>
      <Typography 
        variant="h4" 
        sx={{ 
          fontSize: { xs: '1.2rem', sm: '1.5rem', md: '2rem' },
          color: 'text.secondary',
          textAlign: 'center',
          mb: 4,
          maxWidth: '80%'
        }}
      >
        This page you are looking for could not be found.
      </Typography>
      <Button 
        color="primary" 
        variant="contained" 
        component={RouterLink} 
        to="/" 
        disableElevation
        size="large"
        sx={{ 
          px: { xs: 4, sm: 6 }, 
          py: { xs: 1, sm: 1.5 },
          fontSize: { xs: '1rem', sm: '1.1rem' },
          minWidth: '200px'
        }}
      >
        Go Back to Home
      </Button>
    </ContentWrapper>
  </StyledBox>
);

export default Error;
