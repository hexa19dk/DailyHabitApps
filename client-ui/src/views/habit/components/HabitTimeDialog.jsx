import React, { useState } from 'react';
import { Dialog, DialogTitle, DialogContent, DialogActions, Button, TextField, Box } from '@mui/material';

const HabitTimeDialog = ({ open, onClose, onSave }) => {
  const [minutes, setMinutes] = useState('');

  const handleSave = () => {
    if (minutes && !isNaN(minutes) && Number(minutes) > 0) {
      onSave(Number(minutes));
      setMinutes('');
    }
  };

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle>Time Spent on Habit</DialogTitle>
      <DialogContent>
        <Box display="flex" flexDirection="column" alignItems="center" gap={2}>
          <TextField
            label="Minutes Spent"
            type="number"
            value={minutes}
            onChange={e => setMinutes(e.target.value)}
            inputProps={{ min: 1 }}
            fullWidth
            autoFocus
          />
        </Box>
      </DialogContent>
      <DialogActions>
        <Button onClick={onClose} color="secondary">Cancel</Button>
        <Button onClick={handleSave} color="primary" variant="contained">Save</Button>
      </DialogActions>
    </Dialog>
  );
};

export default HabitTimeDialog;
