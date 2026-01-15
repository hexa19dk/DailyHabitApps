import React from 'react';
import Dayjs from 'dayjs';
import { styled, Stack, Typography } from '@mui/material';
import IconButton from '@mui/material/IconButton';
import ChevronLeft from '@mui/icons-material/ChevronLeft';
import ChevronRight from '@mui/icons-material/ChevronRight';
import { LocalizationProvider } from '@mui/x-date-pickers/LocalizationProvider';
import { AdapterDayjs } from '@mui/x-date-pickers/AdapterDayjs';
import { DateCalendar } from '@mui/x-date-pickers/DateCalendar';
import { PickersDay } from '@mui/x-date-pickers/PickersDay';

// --- Calendar Header ---
const CalendarHeaderCustomRoot = styled('div')({
    display: 'flex',
    justifyContent: 'space-between',
    padding: '8px 16px',
    alignItems: 'center',
});

function CalendarHeaderCustom(props) {
    const { currentMonth, onMonthChange } = props;
    const selectNextMonth = () => onMonthChange(currentMonth.add(1, 'month'));
    const selectPreviousMonth = () => onMonthChange(currentMonth.subtract(1, 'month'));

    return (
        <CalendarHeaderCustomRoot sx={{ backgroundColor: '#1E90FF', borderRadius: '10px' }}>
            <Stack spacing={1} direction="row">
                <IconButton onClick={selectPreviousMonth} title="Previous month" sx={{
                    color: '#FFFFFF', outline: 'none',
                    border: 'none',
                    '&:focus': { outline: 'none' },
                    '&:focus-visible': { outline: 'none' }
                }}>
                    <ChevronLeft />
                </IconButton>
            </Stack>
            <Typography variant="h3" sx={{ color: '#FFFFFF' }}>{currentMonth.format('MMMM YYYY')}</Typography>
            <Stack spacing={1} direction="row">
                <IconButton onClick={selectNextMonth} title="Next month" sx={{
                    color: '#FFFFFF', outline: 'none',
                    border: 'none',
                    '&:focus': { outline: 'none' },
                    '&:focus-visible': { outline: 'none' }
                }}>
                    <ChevronRight />
                </IconButton>
            </Stack>
        </CalendarHeaderCustomRoot>
    );
}

// --- Day Cell Customization ---
function ServerDay(props) {
    const { highlightedDays = [], day, outsideCurrentMonth, ...other } = props;
    // If no highlightedDays, never highlight
    const hasHighlights = Array.isArray(highlightedDays) && highlightedDays.length > 0;
    const isSelected = hasHighlights && highlightedDays.some(d => 
        d.date() === day.date() &&
        d.month() === day.month() &&
        d.year() === day.year()
    );

    return (
        <PickersDay
            {...other}
            outsideCurrentMonth={outsideCurrentMonth}
            day={day}
            sx={{
                width: '100%',
                aspectRatio: '1 / 1',
                maxWidth: '80px',
                borderRadius: '60%',
                bgcolor: isSelected ? 'primary.main' : '#fff',
                color: isSelected ? 'white' : 'inherit',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                fontWeight: 'bold',
                '&:hover': {
                    bgcolor: isSelected ? 'primary.dark' : 'rgba(0,0,0,0.04)',
                },
                outline: 'none',
                border: 'none',
                '&:focus': { outline: 'none' },
                '&:focus-visible': { outline: 'none' }
            }}
        />
    );
}

export default function Calendar({ highlightedDays = [] }) {
    // const [highlightedDays, setHighlightedDays] = React.useState([19, 20, 21, 22]);
    const [value, setValue] = React.useState(Dayjs());

    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <DateCalendar
                // defaultValue={today}
                onChange={(newValue) => setValue(newValue)}
                views={["year", "month", "day"]}
                sx={{
                    // --- Ensure perfect grid alignment between header and days ---
                    '& .MuiDayCalendar-header, & .MuiDayCalendar-weekContainer': {
                        display: 'grid',
                        gridTemplateColumns: 'repeat(7, 1fr)',
                        gap: '6px',
                        justifyItems: 'center',
                        alignItems: 'center',
                    },

                    '& .MuiDayCalendar-weekDayLabel': {
                        textAlign: 'center',
                        fontSize: '1.1rem',
                        fontWeight: '700',
                        color: 'grey',
                    },

                    '& .MuiPickersDay-root': {
                        margin: 0,
                        padding: 0,
                    },

                    width: '100%',
                    maxWidth: '900px', // Match larger card width
                    height: 'auto',
                    mx: 'auto',
                    my: 2,
                }}
                slots={{ calendarHeader: CalendarHeaderCustom, day: ServerDay }}
                slotProps={{ day: { highlightedDays } }}
                readOnly
                disableHighlightToday
            />
        </LocalizationProvider>
    );
}