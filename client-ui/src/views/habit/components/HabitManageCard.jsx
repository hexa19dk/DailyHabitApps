import React from 'react';
import { Card, Box, Typography, Stack, Chip, Button } from '@mui/material';
import { LocationOn, AccessTime, MonetizationOn } from '@mui/icons-material';

const labelColors = {
    Label: '#E0E7FF',
    'Customer Experience': '#D1FAE5',
    Engineering: '#FEF3C7',
};
const labelTextColors = {
    Label: '#6366F1',
    'Customer Experience': '#10B981',
    Engineering: '#F59E42',
};

const HabitManageCard = ({
    label = 'Label',
    labelType = 'Label',
    title = 'Senior Product Designer',
    description = 'Maecenas accumsan lacus vel facilisis. Ullamcorper sit amet risus nullam eget.',
    type = 'Full Time',
    location = 'Remote',
    reward = '$100-$200K',
    onView = () => { },
}) => {
    return (
        <Card
            sx={{
                borderRadius: 4,
                boxShadow: '0 4px 24px 0 rgba(16,30,54,.08)',
                p: 3,
                width: '100%',
                maxWidth: 500,
                mx: 'auto',
                mb: 3,
                background: '#fff',
            }}
        >
            <Stack spacing={2}>
                <Box display="flex" alignItems="center" justifyContent="flex-start">
                    <Chip
                        label={label}
                        sx={{
                            backgroundColor: labelColors[labelType] || '#E0E7FF',
                            color: labelTextColors[labelType] || '#6366F1',
                            fontWeight: 600,
                            fontSize: 13,
                            borderRadius: 2,
                            px: 1.5,
                            py: 0.5,
                        }}
                        size="small"
                    />
                </Box>
                <Typography variant="h6" fontWeight={700} color="#1E293B">
                    {title}
                </Typography>
                <Typography variant="body2" color="text.secondary">
                    {description}
                </Typography>
                <Stack direction="row" spacing={3} alignItems="center" mt={1}>
                    <Stack direction="row" spacing={1} alignItems="center">
                        <AccessTime sx={{ fontSize: 20, color: '#64748B' }} />
                        <Typography variant="body2" color="#64748B">{type}</Typography>
                    </Stack>
                    <Stack direction="row" spacing={1} alignItems="center">
                        <LocationOn sx={{ fontSize: 20, color: '#64748B' }} />
                        <Typography variant="body2" color="#64748B">{location}</Typography>
                    </Stack>
                    <Stack direction="row" spacing={1} alignItems="center">
                        <MonetizationOn sx={{ fontSize: 20, color: '#64748B' }} />
                        <Typography variant="body2" color="#64748B">{reward}</Typography>
                    </Stack>
                </Stack>
                <Box display="flex" justifyContent="flex-end" alignItems="center">
                    <Button
                        variant="text"
                        sx={{
                            color: '#7C3AED',
                            fontWeight: 600,
                            textTransform: 'none',
                            fontSize: 16,
                            '&:hover': { color: '#5B21B6', background: 'transparent' },
                        }}
                        endIcon={<span style={{ fontSize: 18, marginLeft: 4 }}>→</span>}
                        onClick={onView}
                    >
                        View Job
                    </Button>
                </Box>
            </Stack>
        </Card>
    );
};

export default HabitManageCard;
