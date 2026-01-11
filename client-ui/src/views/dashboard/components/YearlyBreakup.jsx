import React from 'react';
import Chart from 'react-apexcharts';
import { Card, CardContent, Typography, Stack } from '@mui/material';
import { IconArrowUpLeft } from '@tabler/icons-react';

const YearlyBreakup = () => {
  const series = [38, 40, 22];

  const options = {
    chart: {
      type: 'donut',
      fontFamily: "'Plus Jakarta Sans', sans-serif",
    },
    labels: ['2022', '2023', '2024'],
    colors: ['#5D87FF', '#49BEFF', '#FA896B'],
    plotOptions: {
      pie: {
        donut: {
          size: '85%',
          background: 'transparent',
          labels: {
            show: true,
            name: {
              show: true,
              fontSize: '14px',
              color: '#adb0bb',
            },
            value: {
              show: true,
              fontSize: '24px',
              color: '#adb0bb',
            },
            total: {
              show: true,
              label: 'Growth',
              fontSize: '14px',
              color: '#adb0bb',
            },
          },
        },
      },
    },
    dataLabels: {
      enabled: false,
    },
    legend: {
      show: false,
    },
    tooltip: {
      theme: 'dark',
      fillSeriesColor: false,
    },
  };

  return (
    <Card>
      <CardContent>
        <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between">
          <Stack spacing={1}>
            <Typography variant="h5">Yearly Breakup</Typography>
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
        <div style={{ height: '280px', marginTop: '20px' }}>
          <Chart options={options} series={series} type="donut" height="280px" />
        </div>
      </CardContent>
    </Card>
  );
};

export default YearlyBreakup;
