import React, { useState } from "react";
import { styled, Container, Box } from '@mui/material';

import Header from './header/Header';
import Sidebar from './sidebar/Sidebar';
import { Outlet } from "react-router";
import Footer from "./footer/Footer";

const MainWrapper = styled('div')(() => ({
  display: 'flex',
  width: '100%',
  minHeight: '100vh',
  position: 'relative',
}));

const PageWrapper = styled('div')(() => ({
  display: 'flex',
  flexGrow: 1,
  flexDirection: 'column',
  zIndex: 1,
  backgroundColor: 'transparent',
  position: 'relative',
  minHeight: '100vh',
}));

const ContentWrapper = styled(Box)(() => ({
  flexGrow: 1,
  paddingTop: '70px', // Height of the header
  minHeight: 'calc(100vh - 70px)', // Subtract header height
  display: 'flex',
  flexDirection: 'column',
}));

const FullLayout = () => {
  const [isSidebarOpen, setSidebarOpen] = useState(true);
  const [isMobileSidebarOpen, setMobileSidebarOpen] = useState(false);
  // const sidebarWidth = 270; // Match this with your Sidebar width

  return (
    <MainWrapper className='mainwrapper'>
      <Sidebar
        isSidebarOpen={isSidebarOpen}
        isMobileSidebarOpen={isMobileSidebarOpen}
        onSidebarClose={() => setMobileSidebarOpen(false)}
      /> 

      <PageWrapper className="page-wrapper">
        <Header
          toggleSidebar={() => setSidebarOpen(!isSidebarOpen)}
          toggleMobileSidebar={() => setMobileSidebarOpen(true)}
        />

        <Container
          sx={{
            paddingTop: "100px",
            paddingX: { xs: 2, sm: 3 },
            // marginLeft: { sm: `${sidebarWidth}px` },
            width: { sm: `calc(100%)` },
            // width: { sm: `calc(100% - ${sidebarWidth}px)` },
            boxSizing: 'border-box'
          }}
        >
          <Box sx={{ 
            minHeight: 'calc(100vh - 250px)',
            width: '100%'
          }}>
            <Outlet />
          </Box>
        </Container>
        <Footer /> 
      </PageWrapper>
    </MainWrapper>
  );
};

export default FullLayout;
