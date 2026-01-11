import React, { useEffect } from 'react';
import { useState } from 'react';
import { Grid, Box, Card, Typography, Stack, Checkbox, Button, CircularProgress } from '@mui/material';
import { IconFlame, IconPlus, IconProgressCheck, IconMapPinFilled, IconClockHour1 } from '@tabler/icons-react';
import { useHabits } from '../../context/HabitContext';
import { useAuth } from '../../context/AuthContext';
import { useSnackbar } from '../../context/SnackbarContext';
import { useHabitTracking } from '../../context/HabitTrackingContext';
import CircleCheckedFilled from '@mui/icons-material/CheckCircle';
import CircleUnchecked from '@mui/icons-material/RadioButtonUnchecked';

import HabitMenuButton from './components/HabitMenuButton';
import HabitTrackingDialog from './components/HabitTrackingDialog';
import HabitSummaryCards from "./components/HabitSummaryCard";
import PageContainer from "../../components/container/PageContainer";
import HabitDialogForm from './components/HabitDialogForm';
import HabitTimeDialog from './components/HabitTimeDialog';


const Habit = () => {
    const [open, setOpen] = useState(false);
    const { habits, loading, createHabit, fetchHabits, updateHabit, deleteHabit } = useHabits();
    const { submitHabitTracking, getHabitStats, submitDailyHabit } = useHabitTracking();
    const { user } = useAuth();

    const handleOpen = () => setOpen(true);
    const [openTrackingDialog, setOpenTrackingDialog] = useState(false);
    const [openTimeDialog, setOpenTimeDialog] = useState(false);
    const [pendingHabitId, setPendingHabitId] = useState(null);
    const { showError, showSuccess } = useSnackbar();
    const [editMode, setEditMode] = useState(false);
    const [selectedHabit, setSelectedHabit] = useState(null);
    const [selectedCardHabit, setSelectedCardHabit] = useState(null);
    const [habitStats, setHabitStats] = useState({});


    useEffect(() => {
        if (!user?.userId) return;
        fetchHabits(user.userId);
    }, [fetchHabits, user?.userId]);

    useEffect(() => {
        if (!habits?.result || habits.result.length === 0) return;

        const fetchAllStats = async () => {
            const statsMap = {};
            for (const habit of habits.result) {
                try {
                    const res = await getHabitStats(habit.id, user.userId);
                    if (res?.result) {
                        statsMap[habit.id] = res.result;
                    }
                } catch (e) {
                    console.error("Error fetching stats for habit", habit.id, e);
                }
            }
            setHabitStats(statsMap);
        };

        fetchAllStats();
    }, [habits?.result, getHabitStats, user?.userId]);

    const habitSummaryStats = {
        todaySummary: { habitsToday: 5, completedToday: 2, todayCompletionRate: 40 },
        weeklySummary: { weeklyCompletionRate: 65, totalCompletedThisWeek: 13 },
        monthlySummary: { monthlyCompletionRate: 70, totalMonthlySessions: 35 },
        habitHealthScore: 82
    };

    const handleOpenTrackingDialog = (habitId) => {
        setSelectedCardHabit(habitId);
        setOpenTrackingDialog(true);
    };

    const handleCloseTrackingDialog = () => setOpenTrackingDialog(false);

    const handleDeleteHabit = async (habitId) => {
        try {
            await deleteHabit(habitId);
            showSuccess('Habit deleted successfully!');
        } catch (error) {
            console.error('Failed to delete habit:', error);
        }
    }

    const handleSubmitHabit = async (habit) => {
        try {
            let res;
            if (editMode) {
                res = await updateHabit(habit.id, habit);
                if (res.status === 200) {
                    showSuccess('Habit updated successfully!');
                    // Optionally update local state
                } else {
                    console.log('Update Habit Response Error: ', res);
                    showError('Failed to update habit.');
                }
            } else {
                res = await createHabit(habit);
                if (res.status === 200) {
                    showSuccess('Habit created successfully!');
                    // Optionally update local state
                } else {
                    console.log('Create Habit Response Error: ', res);
                    showError('Failed to create habit.');
                }
            }
            fetchHabits(user.userId);
            setOpen(false);
            setEditMode(false);
            setSelectedCardHabit(null);
        } catch (error) {
            console.log('Habit creation failed:', error);
            showError("An error occurred while saving the habit.");
        }
    }

    const handleSaveTracking = async (formData) => {
        try {
            const trackingPayload = {
                ...formData,
                userId: user.userId,
            }
            const response = await submitHabitTracking(trackingPayload);
            console.log('Habit tracking response:', response);
            if (response.status == 200) {
                showSuccess('Habit tracking saved successfully!');
            } else {
                showError(response.data.errorMessages || 'Failed to submit habit tracking.');
            }
        } catch (error) {
            console.error('Failed to save habit tracking:', error.message);
            showError(error.response?.data?.errorMessages || error.message || 'An error occurred while submitting habit tracking.');
        }
    }

    const handleEditHabit = (habit) => {
        setSelectedHabit(habit);
        setEditMode(true);
        setOpen(true);
    };

    // Show time dialog after marking complete
    const handleMarkCompletion = (habitId) => {
        setPendingHabitId(habitId);
        setOpenTimeDialog(true);
    };

    // Called after time entry
    const handleSaveTimeSpent = async (minutes) => {
        setOpenTimeDialog(false);
        if (!pendingHabitId) return;

        try {
            // Submit completion with time spent (use correct DB field: TimeSpentMinutes)
            const res = await submitDailyHabit(pendingHabitId, {}, minutes);
            if (res.status === 200) {
                showSuccess('Habit marked as complete for today!');
            } else {
                showError('Failed to mark habit as complete.');
            }
        } catch (error) {
            console.error('Error marking habit as complete:', error);
            showError('Failed to mark habit as complete.');
        }
        setPendingHabitId(null);
    };

    return (
        <PageContainer title="Habits" description="Habit">
            <Typography variant="h4" gutterBottom>
                Create and Manage Your Habits
            </Typography>
            <Box>               

                <HabitSummaryCards stats={habitSummaryStats} />

                {/* Button Create Habit Section */}
                <Grid container paddingTop={4} display={'block'}>
                    <Grid item xs={12}>
                        <Button onClick={handleOpen} variant="outlined" color="primary" startIcon={<IconPlus />} fullWidth sx={{
                            '&:focus': {
                                outline: 'none',
                                boxShadow: 'none'
                            },
                            '&:focus-visible': {
                                outline: 'none',
                                boxShadow: 'none'
                            }
                        }}>
                            Add New Habit
                        </Button>
                    </Grid>
                </Grid>

                {/* Habit Card Section*/}
                {loading ? (
                    <Grid item xs={12} display="flex" justifyContent="center" alignItems="center">
                        <CircularProgress color="primary" />
                    </Grid>
                ) : habits.length === 0 ? (
                    <Grid item xs={12}>
                        <Card sx={{ padding: 3, height: '100%', width: '100%', textAlign: 'center' }}>
                            <Typography variant="h6" color="textSecondary">
                                No habits found. Please add a new habit.
                            </Typography>
                        </Card>
                    </Grid>
                ) : (
                    habits.result.map((habit, index) => {
                        const progress = habitStats[habit.id]?.completionRate || 0;
                        const monthlyGoal = habitStats[habit.id]?.monthlyGoal || 0;
                        console.log("HabitStats: ", habitStats[habit.id], habit);

                        return (
                            <Grid container spacing={3} sx={{ pt: 2 }} key={index}>
                                <Grid item xs={12} sx={{ width: '100%' }}>
                                    <Card sx={{ 
                                            p: 3, height: '100%', width: '100%', boxShadow: 3, cursor: 'pointer', transition: "0.2s",
                                            '&:hover': { boxShadow: 6, transform: "translateY(-2px)" }
                                        }}
                                        onClick={() => handleOpenTrackingDialog(habit.id)}
                                    >
                                        <Box display="flex" flexDirection="row" alignItems="center" sx={{ width: '100%' }}>
                                            {/* Checkbox on the left, large, triggers HabitTrackingDialog */}
                                            <Checkbox
                                                size="large"
                                                icon={<CircleUnchecked fontSize="large" />}
                                                checkedIcon={<CircleCheckedFilled fontSize="large" />}
                                                sx={{ mr: 2 }}
                                                checked={habitStats[habit.id]?.completedToday === true}
                                                disabled={habitStats[habit.id]?.completedToday === true}
                                                onClick={(e) => {
                                                    e.stopPropagation();
                                                    handleMarkCompletion(habit.id);
                                                }}
                                            />
                                            {/* Title & Subtitle */}
                                            <Box display="flex" flexDirection="column" alignItems="flex-start" sx={{ flex: 2 }}>
                                                <Typography variant="h3" color="#1E293B" sx={{ fontWeight: 600 }}>
                                                    {habit.name}
                                                </Typography>
                                                <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>
                                                    {habit.description}
                                                </Typography>
                                            </Box>
                                            {/* Right: Menu Button */}
                                            <Box display="flex" alignItems="center" sx={{ flex: 0 }}>
                                                <HabitMenuButton
                                                    onEdit={() => handleEditHabit(habit)}
                                                    onDelete={() => handleDeleteHabit(habit.id)}
                                                />
                                            </Box>
                                        </Box>

                                        {/* Goal Value, Goal Unit, Goal Frequency with icons */}
                                        <Box display="flex" flexDirection="row" alignItems="center" gap={2} sx={{ mt: 2 }}>
                                            <Box display="flex" alignItems="center" gap={1}>
                                                <IconProgressCheck size={20} color="#6c4ed9" />
                                                <Typography variant="body2" color="text.secondary">Current Progress: {habit.goalValue}</Typography>
                                            </Box>
                                            <Box display="flex" alignItems="center" gap={1}>
                                                <IconMapPinFilled size={20} color="#d94e4e" />
                                                <Typography variant="body2" color="text.secondary">Monthly Goal: {monthlyGoal}</Typography>
                                            </Box>
                                            <Box display="flex" alignItems="center" gap={1}>
                                                <IconClockHour1 size={20} color="#32CD32" />
                                                <Typography variant="body2" color="text.secondary">Goal Frequency: {habit.goalFrequency}</Typography>
                                            </Box>
                                        </Box>

                                        {/* Progress Bar & Percentage */}
                                        <Box display="flex" alignItems="center" gap={2} sx={{ mt: 2 }}>
                                            <Box sx={{ flex: 1 }}>
                                                <Box sx={{ width: '100%', bgcolor: '#f0f0f0', borderRadius: 2, height: 8 }}>
                                                    {progress > 0 ? (
                                                        <Box sx={{ width: `${progress}%`, bgcolor: '#6c4ed9', height: 8, borderRadius: 2 }} />
                                                    ) : (
                                                        <Box sx={{ width: '100%', bgcolor: '#fff', height: 8, borderRadius: 2 }} />
                                                    )}
                                                </Box>
                                            </Box>
                                            <Typography variant="body2" color="text.secondary">{progress}%</Typography>
                                        </Box>
                                    </Card>
                                </Grid>
                            </Grid>
                        )
                    })
                )}

                {/* Habit Dialog Form Section */}
                <HabitDialogForm
                    open={open}
                    onClose={() => { setOpen(false); setEditMode(false); setSelectedHabit(null); }}
                    onSubmit={handleSubmitHabit}
                    habit={selectedHabit}
                    isEditMode={editMode}
                />

                {/* <pre>{JSON.stringify(habits, null, 2)}</pre> */}
                {/* <pre>{JSON.stringify(selectedCardHabit, null, 2)}</pre> */}

                <HabitTrackingDialog
                    open={openTrackingDialog}
                    onClose={handleCloseTrackingDialog}
                    habitId={selectedCardHabit}
                    habits={habits?.result || []}
                    onSave={handleSaveTracking}
                />

                {/* Time Entry Dialog for Habit Completion */}
                <HabitTimeDialog
                    open={openTimeDialog}
                    onClose={() => { setOpenTimeDialog(false); setPendingHabitId(null); }}
                    onSave={handleSaveTimeSpent}
                />

            </Box >
        </PageContainer >
    );
}

export default Habit;