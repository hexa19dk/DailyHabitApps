import React from 'react';
import {
  Card,
  CardContent,
  Typography,
  Box,
  Stack,
  Chip,
  Avatar,
  Divider
} from '@mui/material';
import {
  IconArrowUpLeft,
  IconArrowDownRight,
  IconCreditCard,
  IconCurrencyDollar,
  IconBasket,
  IconCash
} from '@tabler/icons-react';

const transactions = [
  {
    title: 'PayPal Transfer',
    subtitle: 'Money Added',
    amount: '+$350',
    icon: IconCreditCard,
    increase: true
  },
  {
    title: 'Wallet',
    subtitle: 'Bill Payment',
    amount: '-$560',
    icon: IconCash,
    increase: false
  },
  {
    title: 'Credit Card',
    subtitle: 'Money Reversed',
    amount: '+$350',
    icon: IconCurrencyDollar,
    increase: true
  },
  {
    title: 'Bank Transfer',
    subtitle: 'Money Added',
    amount: '+$950',
    icon: IconBasket,
    increase: true
  }
];

const RecentTransactions = () => {
  return (
    <Card>
      <CardContent>
        <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between" mb={3}>
          <Typography variant="h5">Recent Transactions</Typography>
          <Chip
            label="View All"
            size="small"
            sx={{
              backgroundColor: (theme) => theme.palette.primary.light,
              color: 'primary.main',
              borderRadius: '6px',
              cursor: 'pointer'
            }}
          />
        </Stack>
        <Stack spacing={3}>
          {transactions.map((transaction, index) => (
            <Box key={index}>
              <Stack direction="row" spacing={2} alignItems="center" justifyContent="space-between">
                <Stack direction="row" spacing={2} alignItems="center">
                  <Avatar
                    sx={{
                      width: 45,
                      height: 45,
                      backgroundColor: transaction.increase ? 'success.light' : 'error.light',
                    }}
                  >
                    {React.createElement(transaction.icon, {
                      size: 24,
                      color: transaction.increase ? '#13DEB9' : '#FA896B',
                      stroke: 1.5
                    })}
                  </Avatar>
                  <Box>
                    <Typography variant="subtitle1" fontWeight={600}>
                      {transaction.title}
                    </Typography>
                    <Typography variant="subtitle2" color="textSecondary">
                      {transaction.subtitle}
                    </Typography>
                  </Box>
                </Stack>
                <Stack direction="row" alignItems="center" spacing={0.5}>
                  {transaction.increase ? (
                    <IconArrowUpLeft size={16} color="#13DEB9" />
                  ) : (
                    <IconArrowDownRight size={16} color="#FA896B" />
                  )}
                  <Typography
                    variant="subtitle1"
                    fontWeight={600}
                    color={transaction.increase ? 'success.main' : 'error.main'}
                  >
                    {transaction.amount}
                  </Typography>
                </Stack>
              </Stack>
              {index !== transactions.length - 1 && (
                <Divider sx={{ my: 2 }} />
              )}
            </Box>
          ))}
        </Stack>
      </CardContent>
    </Card>
  );
};

export default RecentTransactions;
