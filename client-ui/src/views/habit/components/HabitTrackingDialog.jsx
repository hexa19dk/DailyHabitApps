import React, { useEffect, useState } from "react";
import {
    Dialog,
    DialogTitle,
    DialogContent,
    DialogActions,
    TextField,
    Button,
    FormControlLabel,
    Checkbox,
    MenuItem,
    Stack,
} from "@mui/material";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import { LocalizationProvider, DatePicker } from "@mui/x-date-pickers";
import dayjs from 'dayjs';
import { useAuth } from "../../../context/AuthContext";

const HabitTrackingDialog = ({ open, onClose, onSave, habitId, habits }) => {
    const user = useAuth();
    const initialFormState = {
        trackingDate: dayjs(),
        isCompleted: false,
        notes: "",
        timeSpentMinutes: "",
    };

    const [formData, setFormData] = useState({
        habitId: "",
        habitName: "",
        ...initialFormState,
    });

    const resetFormFields = () => {
        setFormData((prev) => ({
            ...prev,
            ...initialFormState,
        }));
    };


    // Only reset form when dialog is opened or habitId changes (not on every habits change)
    useEffect(() => {
        if (open && habitId && habits && habits.length > 0) {
            const selectedHabit = habits.find((h) => String(h.id) === String(habitId));
            if (selectedHabit) {
                setFormData({
                    habitId: selectedHabit.id,
                    habitName: selectedHabit.name,
                    ...initialFormState,
                });
            } else {
                setFormData({
                    habitId: '',
                    habitName: '',
                    ...initialFormState,
                });
            }
        }
        // Do not reset form on every habits change
        // eslint-disable-next-line react-hooks/exhaustive-deps
    }, [open, habitId]);

    const handleChange = (field, value) => {
        setFormData((prev) => ({ ...prev, [field]: value }));
    };

    const handleSubmit = () => {
        const payload = {
            habitId: formData.habitId,
            userId: user.userId,
            trackingDate: formData.trackingDate?.toISOString(),
            isCompleted: formData.isCompleted,
            notes: formData.notes,
            timeSpentMinutes: parseInt(formData.timeSpentMinutes || 0, 10),
            completedAt: formData.isCompleted ? formData.trackingDate?.toISOString() : null,
            createdAt: new Date().toISOString()
        };

        onSave(payload);
        resetFormFields();
        onClose();
    };

    return (
        <LocalizationProvider dateAdapter={AdapterDayjs}>
            <Dialog open={open} onClose={onClose} fullWidth maxWidth="sm">
                <DialogTitle>Track Habit Progress</DialogTitle>
                <DialogContent>
                    <Stack spacing={2} sx={{ mt: 1 }}>
                        {/* Habit Selector */}
                        <TextField
                            label="Habit Name"
                            value={formData.habitName}
                            onChange={(e) => handleChange("habitName", e.target.value)}
                            fullWidth
                            disabled
                        />

                        {/* Tracking Date */}
                        <LocalizationProvider dateAdapter={AdapterDayjs}>
                            <DatePicker
                                label="Tracking Date"
                                value={formData.trackingDate}
                                onChange={(newDate) => handleChange("trackingDate", newDate)}
                                slotProps={{ textField: { fullWidth: true, required: true } }}
                            />
                        </LocalizationProvider>

                        {/* Is Completed */}
                        <FormControlLabel
                            control={
                                <Checkbox
                                    checked={formData.isCompleted}
                                    onChange={(e) => handleChange("isCompleted", e.target.checked)}
                                />
                            }
                            label="Mark as Completed"
                        />

                        {/* Notes */}
                        <TextField
                            label="Notes (optional)"
                            multiline
                            rows={3}
                            value={formData.notes}
                            onChange={(e) => handleChange("notes", e.target.value)}
                            fullWidth
                        />

                        {/* Time Spent */}
                        <TextField
                            label="Time Spent (minutes)"
                            type="number"
                            value={formData.timeSpentMinutes}
                            onChange={(e) => handleChange("timeSpentMinutes", e.target.value)}
                            fullWidth
                        />
                    </Stack>
                </DialogContent>

                <DialogActions>
                    <Button onClick={onClose} color="inherit" sx={{
                        outline: 'none',
                        border: 'none',
                        '&:focus': { outline: 'none' },
                        '&:focus-visible': { outline: 'none' }
                    }}>
                        Cancel
                    </Button>
                    <Button variant="contained" onClick={handleSubmit}>
                        Submit
                    </Button>
                </DialogActions>
            </Dialog>
        </LocalizationProvider>

    );
};

export default HabitTrackingDialog;
