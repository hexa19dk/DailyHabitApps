import React from 'react';
import Chart from 'react-apexcharts';
import { Card, CardContent, Typography, Stack } from '@mui/material';
import { IconArrowUpLeft } from '@tabler/icons-react';

const SalesOverview = () => {
  const series = [
    {
      name: 'Sales',
      data: [355, 390, 300, 350, 390, 180, 355, 390, 300, 350, 390, 180],
    },
    {
      name: 'Earnings',
      data: [280, 250, 325, 215, 250, 310, 280, 250, 325, 215, 250, 310],
    },
  ];

  const options = {
    chart: {
      type: 'line',
      fontFamily: "'Plus Jakarta Sans', sans-serif",
      foreColor: '#adb0bb',
      toolbar: {
        show: false,
      },
      height: 350,
    },
    colors: ['#5D87FF', '#49BEFF'],
    plotOptions: {
      bar: {
        horizontal: false,
        columnWidth: '35%',
        borderRadius: [6],
      },
    },
    stroke: {
      show: true,
      width: 3,
      curve: 'smooth',
      colors: ['#5D87FF', '#49BEFF'],
    },
    grid: {
      borderColor: 'rgba(0,0,0,0.1)',
      strokeDashArray: 3,
      xaxis: {
        lines: {
          show: false,
        },
      },
    },
    dataLabels: {
      enabled: false,
    },
    xaxis: {
      categories: [
        'Jan',
        'Feb',
        'Mar',
        'Apr',
        'May',
        'Jun',
        'Jul',
        'Aug',
        'Sep',
        'Oct',
        'Nov',
        'Dec',
      ],
    },
    yaxis: {
      tickAmount: 4,
    },
    tooltip: {
      theme: 'dark',
    },
  };

  return (
    <Card>
      <CardContent>
        <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between" mb={3}>
          <Stack spacing={1}>
            <Typography variant="h5">Sales Overview</Typography>
            <Stack direction="row" spacing={1} alignItems="center">
              <Typography variant="subtitle1" fontWeight={600}>$36,358</Typography>
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
                  +9%
                </Typography>
              </Stack>
            </Stack>
          </Stack>
        </Stack>
        <Chart options={options} series={series} type="line" height="350px" />
      </CardContent>
    </Card>
  );
};

export default SalesOverview;
