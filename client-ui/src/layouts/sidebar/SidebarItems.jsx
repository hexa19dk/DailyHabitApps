import React from 'react';
import { useLocation, Link } from 'react-router-dom';
import {
  List,
  ListItem,
  ListItemIcon,
  ListItemText,
  ListSubheader,
  Box,
  useTheme
} from '@mui/material';
import {
  IconLayoutDashboard,
  IconTable,
  IconCalendarWeek,
  IconSettings,
  IconChartHistogram
} from '@tabler/icons-react';

const menuItems = [
  {
    title: 'Dashboard',
    icon: IconLayoutDashboard,
    href: '/dashboard'
  },
  {
    title: 'Habit Tracker',
    icon: IconCalendarWeek,
    href: '/habits'
  },
  {
    title: 'Stats',
    icon: IconChartHistogram,
    href: '/stats'
  },
  // {
  //   title: 'Tables',
  //   icon: IconTable,
  //   href: '/tables'
  // },
  // {
  //   title: 'Profile',
  //   icon: IconUserCircle,
  //   href: '/profile'
  // },
  {
    title: 'Settings',
    icon: IconSettings,
    href: '/settings'
  }
];

const SidebarItems = () => {
  const theme = useTheme();
  const { pathname } = useLocation();

  return (
    <Box sx={{ pt: 2 }}>
      <List
        component="nav"
        aria-labelledby="nested-list-subheader"
        subheader={
          <ListSubheader
            component="div"
            id="nested-list-subheader"
            sx={{
              bgcolor: 'transparent',
              color: theme.palette.text.secondary,
              lineHeight: '24px',
              p: 0,
              mb: 1
            }}
          >
            MENU
          </ListSubheader>
        }
      >
        {menuItems.map((item, index) => {
          const Icon = item.icon;
          const isSelected = pathname === item.href;

          return (
            <ListItem
              key={index}
              component={Link}
              to={item.href}
              sx={{
                mb: 1,
                mx: 0,
                px: 2,
                borderRadius: 1,
                cursor: 'pointer',
                color: isSelected ? 'primary.main' : 'text.secondary',
                backgroundColor: isSelected ? 'primary.light' : 'transparent',
                '&:hover': {
                  backgroundColor: isSelected ? 'primary.light' : 'action.hover',
                },
                '& .MuiListItemIcon-root': {
                  color: isSelected ? 'primary.main' : 'text.secondary',
                },
              }}
            >
              <ListItemIcon sx={{ minWidth: 40 }}>
                <Icon size={20} stroke={1.5} />
              </ListItemIcon>
              <ListItemText 
                primary={item.title}
                primaryTypographyProps={{
                  fontSize: '14px',
                  fontWeight: isSelected ? 600 : 400
                }}
              />
            </ListItem>
          );
        })}
      </List>
    </Box>
  );
};

export default SidebarItems;

