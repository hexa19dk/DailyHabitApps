import { useState } from 'react'
import {
  Box,
  AppBar,
  Toolbar,
  styled,
  Stack,
  IconButton,
  Badge,
} from '@mui/material'
import PropTypes from 'prop-types'

// components
import Profile from './Profile'
import Search from './Search'
import { IconBellRinging, IconMenu } from '@tabler/icons-react'

const Header = (props) => {
  const [anchorEl, setAnchorEl] = useState(null)
  const [menuPosition, setMenuPosition] = useState(null)

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget)
    setMenuPosition({
      top: event.clientY,
      left: event.clientX,
    })
  }

  const handleClose = () => {
    setAnchorEl(null)
    setMenuPosition(null)
  }

  const AppBarStyled = styled(AppBar)(({ theme }) => ({
    boxShadow: 'none',
    background: theme.palette.background.paper,
    justifyContent: 'center',
    backdropFilter: 'blur(4px)',
    borderBottom: '1px solid',
    borderColor: theme.palette.divider,
    [theme.breakpoints.up('lg')]: {
      minHeight: '70px',
    },
    position: 'fixed',
    top: 0,
    left: 0,
    right: 0,
    zIndex: theme.zIndex.appBar,
    width: '100%'
  }))

  const ToolbarStyled = styled(Toolbar)(({ theme }) => ({
    width: '100%',
    color: theme.palette.text.secondary,
    minHeight: '70px',
    padding: '0 24px !important',
    display: 'flex',
    alignItems: 'center',
    justifyContent: 'space-between'
  }))

  return (
    <AppBarStyled position="fixed" color="default">
      <ToolbarStyled>
        <Box sx={{ display: 'flex', alignItems: 'center' }}>
          <IconButton
            color='inherit'
            aria-label='menu'
            onClick={props.toggleMobileSidebar}
            sx={{
              display: {
                lg: 'none',
                xs: 'inline',
              },
              mr: 2
            }}>
            <IconMenu width='20' height='20' />
          </IconButton>

          <Search />
        </Box>
        
        <Stack spacing={1} direction='row' alignItems='center'>
          <IconButton
            size='large'
            aria-label='show notifications'
            color='inherit'
            sx={{
              color: 'text.secondary',
              '&:hover': {
                backgroundColor: 'primary.light',
              },
            }}>
            <Badge badgeContent={4} color='primary'>
              <IconBellRinging size='21' stroke='1.5' />
            </Badge>
          </IconButton>
          <Profile />
        </Stack>
      </ToolbarStyled>
    </AppBarStyled>
  )
}

Header.propTypes = {
  toggleMobileSidebar: PropTypes.func,
}

export default Header
