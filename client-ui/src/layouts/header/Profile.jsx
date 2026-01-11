import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import {
  Avatar,
  Box,
  Menu,
  Button,
  IconButton,
  MenuItem,
  ListItemIcon,
  ListItemText,
  Typography
} from '@mui/material';

import { IconDashboard, IconMail, IconUser, IconLogout } from '@tabler/icons-react';
import { useAuth } from '../../context/AuthContext';
import { useSnackbar } from '../../context/SnackbarContext';

const Profile = () => {
  const [anchorEl2, setAnchorEl2] = useState(null);
  const navigate = useNavigate();
  const { logout } = useAuth();
  const { showSuccess } = useSnackbar();
  
  const handleClick2 = (event) => {
    setAnchorEl2(event.currentTarget);
  };
  const handleClose2 = () => {
    setAnchorEl2(null);
  };

  const handleLogout = () => {
    logout();
    showSuccess('You have been logged out successfully.');
    navigate('/auth/login');
    handleClose2();
  };

  return (
    <Box>
      <IconButton
        size="large"
        aria-label="show profile menu"
        color="inherit"
        aria-controls="profile-menu"
        aria-haspopup="true"
        sx={{
          ...(Boolean(anchorEl2) && {
            color: 'primary.main',
          }),
        }}
        onClick={handleClick2}
      >
        <Avatar
          sx={{
            width: 35,
            height: 35,
            backgroundColor: 'primary.main'
          }}
        >
          <IconUser size={22} />
        </Avatar>
      </IconButton>
      <Menu
        id="profile-menu"
        anchorEl={anchorEl2}
        keepMounted
        open={Boolean(anchorEl2)}
        onClose={handleClose2}
        anchorOrigin={{ horizontal: 'right', vertical: 'bottom' }}
        transformOrigin={{ horizontal: 'right', vertical: 'top' }}
        sx={{
          '& .MuiMenu-paper': {
            width: '200px',
          },
        }}
      >
        <MenuItem component={Link} to="/profile">
          <ListItemIcon>
            <IconUser size={20} />
          </ListItemIcon>
          <ListItemText>
            <Typography variant="subtitle1" color="textPrimary">My Profile</Typography>
          </ListItemText>
        </MenuItem>
        <MenuItem component={Link} to="/dashboard">
          <ListItemIcon>
            <IconDashboard size={20} />
          </ListItemIcon>
          <ListItemText>
            <Typography variant="subtitle1" color="textPrimary">Dashboard</Typography>
          </ListItemText>
        </MenuItem>
        <MenuItem component={Link} to="/settings">
          <ListItemIcon>
            <IconMail size={20} />
          </ListItemIcon>
          <ListItemText>
            <Typography variant="subtitle1" color="textPrimary">Settings</Typography>
          </ListItemText>
        </MenuItem>
        <Box mt={1} py={1} px={2}>
          <Button
            variant="outlined"
            color="primary"
            onClick={handleLogout}
            fullWidth
            startIcon={<IconLogout size={20} />}
          >
            Logout
          </Button>
        </Box>
      </Menu>
    </Box>
  );
};

export default Profile;
