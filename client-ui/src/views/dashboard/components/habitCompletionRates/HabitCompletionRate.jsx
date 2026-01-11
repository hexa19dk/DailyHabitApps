import { useEffect, useState } from 'react';
import {
    Stack,
    FormControl,
    InputLabel,
    Select,
    MenuItem,
    Tabs,
    Tab,
    Box,
    CircularProgress,
    Typography,
    Card,
    CardContent
} from '@mui/material';

import { useHabitTracking } from '../../../../context/HabitTrackingContext';
import { useAuth } from '../../../../context/AuthContext';
import { useHabits } from '../../../../context/HabitContext';
import HabitDistributedChart from './HabitDistributedChart';

const HabitCompletionRate = () => {
    const { getWeeklyDistribution, getMonthlyDistribution, getYearlyDistribution } = useHabitTracking();
    const { user } = useAuth();
    const { habits, loading: habitsLoading, fetchHabits } = useHabits();
    const [habitId, setHabitId] = useState('');

    const [range, setRange] = useState('weekly');
    const [chartData, setChartData] = useState({ labels: [], values: [] });

    const currYear = new Date().getFullYear();
    const currMonth = new Date().getMonth() + 1;

    // Fetch habits once
    useEffect(() => {
        fetchHabits();
    }, [fetchHabits]);

    // Auto-select first habit
    useEffect(() => {
        if (!habitId && habits.result?.length > 0) {
            setHabitId(habits.result[0].id);
        }
    }, [habits, habitId]);

    // Fetch Habit Distribution Stats
    useEffect(() => {
        if (!habitId || !user?.userId) return;

        const fetchDistributedStats = async () => {
            let response;

            try {
                if (range === 'weekly') {
                    response = await getWeeklyDistribution({
                        HabitId: habitId,
                        UserId: user.userId,
                        Year: currYear,
                        Month: currMonth,
                    });
                } else if (range === 'monthly') {
                    response = await getMonthlyDistribution({
                        HabitId: habitId,
                        UserId: user.userId,
                        Year: currYear,
                    });
                } else if (range === 'yearly') {
                    response = await getYearlyDistribution({
                        HabitId: habitId,
                        UserId: user.userId,
                        StartYear: currYear - 4,
                        EndYear: currYear,
                    });
                }

                if (response?.data.result) {
                    setChartData({
                        labels: response.data.result.labels,
                        values: response.data.result.values,
                    });
                } else {
                    setChartData({ labels: [], values: [] });
                }
            } catch (error) {
                console.error('Error fetching distributed stats:', error);
            }
        }

        fetchDistributedStats();

    }, [habitId, range, user?.userId]);


    /* ---------------------------
       Inline HabitSelector
    ---------------------------- */
    const renderHabitSelector = () => (
        <FormControl fullWidth size="small">
            <InputLabel id="habit-select-label">Habit</InputLabel>
            <Select
                labelId="habit-select-label"
                value={habitId}
                label="Habit"
                onChange={(e) => setHabitId(e.target.value)}
                disabled={habitsLoading}
            >
                {habitsLoading && (
                    <MenuItem value="">
                        <CircularProgress size={20} />
                    </MenuItem>
                )}

                {!habitsLoading && habits.length === 0 && (
                    <MenuItem value="" disabled>
                        No habits found
                    </MenuItem>
                )}

                {!habitsLoading &&
                    habits.result.map((habit) => (
                        <MenuItem key={habit.id} value={habit.id}>
                            {habit.name}
                        </MenuItem>
                    ))
                }
            </Select>
        </FormControl>
    );

    /* ---------------------------
       Inline TimeRangeTabs
    ---------------------------- */
    const renderTimeRangeTabs = () => (
        <Box>
            <Tabs
                value={range}
                onChange={(_, newValue) => setRange(newValue)}
                indicatorColor="primary"
                textColor="primary"
                variant="fullWidth"
            >
                <Tab label="Weekly" value="weekly" />
                <Tab label="Monthly" value="monthly" />
                <Tab label="Yearly" value="yearly" />
            </Tabs>
        </Box>
    );

    return (
        <Card sx={{ width: '100%' }}>
            <CardContent>
                <Typography variant="h5" mb={2}>
                    Habit Completion Rate
                </Typography>
                <Stack spacing={3} sx={{ width: '100%' }}>
                    {renderHabitSelector()}
                    {renderTimeRangeTabs()}

                    <HabitDistributedChart
                        labels={chartData.labels}
                        values={chartData.values}
                    />
                </Stack>
            </CardContent>
        </Card>
    );
};

export default HabitCompletionRate;
