import React from 'react';
import Chart from 'react-apexcharts';
import { Card, CardContent, Typography, Stack } from '@mui/material';
import { IconArrowUpLeft } from '@tabler/icons-react';

const MonthlyEarnings = () => {
  const series = [{
    name: 'Monthly Earnings',
    data: [6, 10, 9, 11, 9, 10, 12, 10, 9, 11, 9, 10]
  }];

  const options = {
    chart: {
      type: 'bar',
      fontFamily: "'Plus Jakarta Sans', sans-serif",
      foreColor: '#adb0bb',
      toolbar: {
        show: false,
      },
      height: 200,
      stacked: true,
      sparkline: {
        enabled: true
      }
    },
    colors: ['#49BEFF'],
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: '35%',
        borderRadius: [6],
        distributed: true,
      },
    },
    dataLabels: {
      enabled: false,
    },
    stroke: {
      show: true,
      width: 3,
      colors: ['transparent']
    },
    xaxis: {
      categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'],
    },
    yaxis: {
      show: false
    },
    fill: {
      opacity: 1
    },
    tooltip: {
      theme: 'dark',
      fixed: {
        enabled: true,
        position: 'topRight',
        offsetX: 0,
        offsetY: 0,
      },
    },
    grid: {
      padding: {
        left: 0,
        right: 0,
        top: 0,
        bottom: 0
      },
    }
  };

  return (
    <Card>
      <CardContent>
        <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between">
          <Stack spacing={1}>
            <Typography variant="h5">Monthly Earnings</Typography>
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="subtitle1" fontWeight={600}>$6,820</Typography>
              <Stack
                direction="row"
                alignItems="center"
                spacing={0.5}
                sx={{
                  padding: '1px 8px',
                  backgroundColor: 'success.light',
                  borderRadius: '4px',
                }}
              >
                <IconArrowUpLeft size={16} color="#13DEB9" />
                <Typography
                  variant="subtitle2"
                  color="success.main"
                  fontSize="12px"
                  fontWeight="600"
                >
                  +12%
                </Typography>
              </Stack>
            </Stack>
          </Stack>
        </Stack>
        <div style={{ height: '200px', marginTop: '20px' }}>
          <Chart options={options} series={series} type="bar" height="200px" />
        </div>
      </CardContent>
    </Card>
  );
};

export default MonthlyEarnings;
