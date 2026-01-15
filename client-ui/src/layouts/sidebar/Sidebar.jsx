import { useMediaQuery, Box, Drawer } from '@mui/material';
import SidebarItems from './SidebarItems';
// import Scrollbar from "../../../components/custom-scroll/Scrollbar";
import Scrollbar from "../../components/custom-scroll/Scrollbar";

const Sidebar = (props) => {
  const lgUp = useMediaQuery((theme) => theme.breakpoints.up("lg"));
  const sidebarWidth = '270px';

  if (lgUp) {
    return (
      <Box
        sx={{
          width: sidebarWidth,
          flexShrink: 0,
        }}
      >
        <Drawer
          anchor="left"
          open={props.isSidebarOpen}
          variant="permanent"
          slotProps={{
            paper: {
              sx: {
                width: sidebarWidth,
                boxSizing: 'border-box',
                border: '0',
                backgroundColor: (theme) => theme.palette.background.default,
                boxShadow: (theme) => theme.shadows[1],
              },
            }
          }}
        >
          <Scrollbar sx={{ height: "100vh" }}>
            <Box sx={{ px: 3 }}>
              <SidebarItems />
            </Box>
          </Scrollbar>
        </Drawer>
      </Box>
    );
  }

  return (
    <Drawer
      anchor="left"
      open={props.isMobileSidebarOpen}
      onClose={props.onSidebarClose}
      variant="temporary"
      slotProps={{
        paper: {
          sx: {
            width: sidebarWidth,
            boxShadow: (theme) => theme.shadows[8],
            backgroundColor: (theme) => theme.palette.background.default,
          },
        }
      }}
    >
      <Scrollbar sx={{ height: "100vh" }}>
        {/* ------------------------------------------- */}
        {/* Sidebar For Mobile */}
        {/* ------------------------------------------- */}
        <Box sx={{ px: 3 }}>
          <SidebarItems />
        </Box>
      </Scrollbar>
    </Drawer>
  );
};

export default Sidebar;
