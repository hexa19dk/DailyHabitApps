import { useEffect, useState } from 'react';
import { Typography, Grid, Box, Card, CircularProgress, styled, CardActionArea } from "@mui/material";
import { IconProgressCheck, IconMapPinFilled, IconClockHour1 } from '@tabler/icons-react';
import PageContainer from "../../components/container/PageContainer";
import { useAuth } from '../../context/AuthContext';
import { useHabits } from '../../context/HabitContext';
import Calendar from '../habit/components/Calendar';
import dayjs from 'dayjs';
import { useHabitTracking } from '../../context/HabitTrackingContext';


const StyledMenu = styled((props) => (
  <Menu
    elevation={0}
    anchorOrigin={{
      vertical: 'bottom',
      horizontal: 'right',
    }}
    transformOrigin={{
      vertical: 'top',
      horizontal: 'right',
    }}
    {...props}
  />
))(({ theme }) => ({
  '& .MuiPaper-root': {
    borderRadius: 6,
    marginTop: theme.spacing(1),
    minWidth: 180,
    color: 'rgb(55, 65, 81)',
    boxShadow:
      'rgb(255, 255, 255) 0px 0px 0px 0px, rgba(0, 0, 0, 0.05) 0px 0px 0px 1px, rgba(0, 0, 0, 0.1) 0px 10px 15px -3px, rgba(0, 0, 0, 0.05) 0px 4px 6px -2px',
    '& .MuiMenu-list': {
      padding: '4px 0',
    },
    '& .MuiMenuItem-root': {
      '& .MuiSvgIcon-root': {
        fontSize: 18,
        color: theme.palette.text.secondary,
        marginRight: theme.spacing(1.5),
        ...theme.applyStyles('dark', {
          color: 'inherit',
        }),
      },
      '&:active': {
        backgroundColor: alpha(
          theme.palette.primary.main,
          theme.palette.action.selectedOpacity,
        ),
      },
    },
    ...theme.applyStyles('dark', {
      color: theme.palette.grey[300],
    }),
  },
}));

const StyledCardActionArea = styled(CardActionArea)(({ theme }) => ({
  height: '100%',
  outline: 'none',
  '&:focus': {
    outline: 'none',
    boxShadow: 'none',
  },
  '&:focus-visible': {
    outline: 'none',
    boxShadow: 'none',
  },
  '&[data-active]': {
    backgroundColor: theme.palette.action.selected,
    '&:hover': {
      backgroundColor: theme.palette.action.selectedHover,
    },
  },
}));

const Stats = () => {
  const { habits, loading, fetchHabits } = useHabits();
  const { getHabitTrackingDates, getHabitStats } = useHabitTracking();
  const [selectedCardHabit, setSelectedCardHabit] = useState(null);
  const [highlightedDays, setHighlightedDays] = useState([]);
  const [calendarLoading, setCalendarLoading] = useState(false);

  const { user } = useAuth();

  useEffect(() => {
    fetchHabits(user.sub);
  }, [fetchHabits, user.sub]);

  const handleHabitClick = async (habitId) => {
    setSelectedCardHabit(habitId);
    setHighlightedDays([]); // Always clear first
    setCalendarLoading(true);

    const requestHabitId = habitId;

    try {
      const trackingRes = await getHabitTrackingDates(habitId, user.sub);

      if (requestHabitId !== habitId) return;

      if (!trackingRes?.result || trackingRes.result.length === 0) {
        setHighlightedDays([]); // Explicitly set to empty if no dates
        return;
      }

      setHighlightedDays(
        trackingRes.result.map(d => dayjs(d).startOf('day'))
      );

    } finally {
      setCalendarLoading(false);
    }
  };

  // Compute metrics for each habit
  const computeHabitStats = (habit) => {
    const totalDays = habit.totalDays || 0;
    const completedDays = habit.completedDays || 0;

    const completionRate =
      totalDays > 0 ? Math.round((completedDays / totalDays) * 100) : 0;

    // Streak calculations
    const dates = (habit.trackingDates || []).map((d) => dayjs(d));
    const sorted = dates.sort((a, b) => a.valueOf() - b.valueOf());

    let currentStreak = 0;
    let longestStreak = 0;
    let streak = 1;

    for (let i = 1; i < sorted.length; i++) {
      const diff = sorted[i].diff(sorted[i - 1], "day");

      if (diff === 1) {
        streak++;
      } else {
        longestStreak = Math.max(longestStreak, streak);
        streak = 1;
      }
    }

    longestStreak = Math.max(longestStreak, streak);

    // Current streak: does last date connect to today?
    const today = dayjs().startOf("day");
    const lastDate = sorted[sorted.length - 1];

    if (lastDate && today.diff(lastDate, "day") === 0) {
      currentStreak = streak;
    } else if (lastDate && today.diff(lastDate, "day") === 1) {
      currentStreak = streak;
    } else {
      currentStreak = 0;
    }

    return { completionRate, currentStreak, longestStreak };
  };

  return (
    <PageContainer title="Stats" description="Statistics and analytics">
      <Typography variant="h4" gutterBottom>
        Habit Progress Overview
      </Typography>

      <Box>
        {/* Habit Card Section */}
        <Grid container spacing={2} columnSpacing={2} paddingTop={4}>
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
              const { completionRate } = computeHabitStats(habit);

              return (
                <Grid item xs={12} sm={6} md={4} key={index} sx={{ display: 'flex', justifyContent: 'center' }}>
                  <StyledCardActionArea
                    onClick={() => handleHabitClick(habit.id)}
                    data-active={selectedCardHabit === habit.id ? '' : undefined}
                  >
                    <Card
                      sx={{
                        padding: 3,
                        // bgcolor: habit.color,
                        width: '320px', // fixed width for all cards
                        maxWidth: '300px',
                        minWidth: '300px',
                        display: 'flex',
                        flexDirection: 'column',
                        justifyContent: 'center',
                        borderRadius: 3,
                      }}
                    >
                      {/* Habit Title */}
                      <Box display="flex" alignItems="center" height="100%" width="100%">
                        <Box width="100%">
                          <Typography variant="h6" color="#1E293B" sx={{ fontWeight: 550 }}>
                            {habit.name}
                          </Typography>
                        </Box>
                      </Box>

                      {/* Habit Stats */}
                      <Box display="flex" flexDirection="row" alignItems="center" gap={2} sx={{ mt: 2 }}>
                        <Box display="flex" alignItems="center" gap={1}>
                          <IconProgressCheck size={20} color="#6c4ed9" />
                          <Typography variant="body2" color="text.secondary">Goal: {habit.goalValue}</Typography>
                        </Box>
                        <Box display="flex" alignItems="center" gap={1}>
                          <IconClockHour1 size={20} color="#32CD32" />
                          <Typography variant="body2" color="text.secondary">Frequency: {habit.goalFrequency}</Typography>
                        </Box>
                      </Box>

                    </Card>
                  </StyledCardActionArea>
                </Grid>
              )
            })
          )}
        </Grid>

        {/* Calendar Section */}
        <Grid container spacing={3}>
          <Grid item xs={12} paddingTop={2} sx={{ width: '-webkit-fill-available' }}>
            <Card sx={{ padding: 3, height: '100%', width: '100%' }}>
              <Calendar key={selectedCardHabit} highlightedDays={highlightedDays} readOnly={true} />
            </Card>
          </Grid>
        </Grid>
      </Box>

    </PageContainer >
  )
}

export default Stats