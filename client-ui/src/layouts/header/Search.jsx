import React, { useState } from 'react';
import { IconSearch } from '@tabler/icons-react';
import {
  Box,
  TextField,
  InputAdornment,
  Autocomplete,
  styled,
} from '@mui/material';

const Search = () => {
  const [search, setSearch] = useState('');
  
  const StyledSearchBox = styled(Box)(() => ({
    display: 'flex',
    alignItems: 'center',
    width: '200px',
    marginLeft: 'auto',
    marginRight: '10px',
  }));

  return (
    <StyledSearchBox>
      <Autocomplete
        id="search-box"
        freeSolo
        fullWidth
        options={[]}
        value={search}
        onChange={(event, newValue) => {
          setSearch(newValue);
        }}
        renderInput={(params) => (
          <TextField
            {...params}
            placeholder="Search..."
            variant="outlined"
            size="small"
            sx={{
              '& .MuiOutlinedInput-root': {
                backgroundColor: (theme) =>
                  theme.palette.mode === 'dark'
                    ? 'rgba(255, 255, 255, 0.05)'
                    : 'rgba(0, 0, 0, 0.02)',
                '&:hover': {
                  backgroundColor: (theme) =>
                    theme.palette.mode === 'dark'
                      ? 'rgba(255, 255, 255, 0.08)'
                      : 'rgba(0, 0, 0, 0.04)',
                },
                '& fieldset': {
                  borderColor: (theme) => theme.palette.divider,
                },
              },
            }}
            InputProps={{
              ...params.InputProps,
              startAdornment: (
                <InputAdornment position="start">
                  <IconSearch size={20} stroke={1.5} />
                </InputAdornment>
              ),
            }}
          />
        )}
      />
    </StyledSearchBox>
  );
};

export default Search; 