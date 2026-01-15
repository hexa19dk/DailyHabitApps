import React, { Suspense, lazy } from 'react';
import { Grid, Box, Skeleton } from '@mui/material';
import PageContainer from '../../components/container/PageContainer';

// components
const SalesOverview = lazy(() => import('./components/SalesOverview'));
const ProductPerformance = lazy(() => import('./components/ProductPerformance'));
const RecentTransactions = lazy(() => import('./components/RecentTransactions'));
const YearlyBreakup = lazy(() => import('./components/YearlyBreakup'));
const MonthlyEarnings = lazy(() => import('./components/MonthlyEarnings'));
const TopCards = lazy(() => import('./components/TopCards'));
const HabitCompletionRate = lazy(() => import('./components/habitCompletionRates/HabitCompletionRate'));
const HabitHeatmapCalendar = lazy(() => import('./components/HabitHeatmapCalendar'));

const Dashboard = () => {
  const fallback = <Skeleton variant="rectangular" height={200} animation="wave" />;

  const heatmapData = {
    1: 1,
    2: 1,
    3: 0,
    4: 1,
    5: 2,
    6: 1,
    7: 1
  };

  return (
    <PageContainer title="E-commerce Dashboard" description="E-commerce Dashboard">
      <Box>
        <Grid container spacing={4}>
          <Grid xs={12}>
            <Suspense fallback={fallback}>
              <TopCards />
            </Suspense>
          </Grid>

          <Grid item xs={12} sx={{ width: '100%' }}>
            <Suspense fallback={fallback}>
              <HabitCompletionRate />
            </Suspense>
          </Grid>

          <Grid item xs={12} sx={{ width: '100%' }}>
            <Suspense fallback={fallback}>
              <HabitHeatmapCalendar data={heatmapData} />
            </Suspense>
          </Grid>

          <Grid xs={12} lg={4}>
            <Grid container spacing={3}>
              <Grid item xs={12}>
                <Suspense fallback={fallback}>
                  <YearlyBreakup />
                </Suspense>
              </Grid>

              <Grid xs={12}>
                <Suspense fallback={fallback}>
                  <MonthlyEarnings />
                </Suspense>
              </Grid>

              <Grid xs={12} lg={8}>
                <Suspense fallback={fallback}>
                  <SalesOverview />
                </Suspense>
              </Grid>
            </Grid>
          </Grid>



          {/* <Grid xs={12} lg={8}>
            <Suspense fallback={fallback}>
              <ProductPerformance />
            </Suspense>
          </Grid>

          <Grid xs={12} lg={4}>
            <Suspense fallback={fallback}>
              <RecentTransactions />
            </Suspense>
          </Grid> */}

        </Grid>
      </Box>
    </PageContainer>
  );
};

export default Dashboard;
