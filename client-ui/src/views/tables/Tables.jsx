import React, { useState } from 'react';
import {
    Box,
    Card,
    CardContent,
    Typography,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Paper,
    Chip,
    TextField,
    InputAdornment,
    Select,
    MenuItem,
    FormControl,
    InputLabel,
    Stack
} from '@mui/material';
import { IconSearch } from '@tabler/icons-react';
import PageContainer from "../../components/container/PageContainer";

const createData = (name, position, office, age, startDate, salary) => {
    return { name, position, office, age, startDate, salary };
};

const rows = [
    createData('John Doe', 'Senior Developer', 'New York', 35, '2020/01/25', '$320,800'),
    createData('Jane Smith', 'UI Designer', 'London', 28, '2021/03/12', '$290,000'),
    createData('Mike Johnson', 'Marketing Manager', 'San Francisco', 42, '2019/08/15', '$350,000'),
    createData('Sarah Williams', 'Data Analyst', 'Chicago', 31, '2021/05/20', '$270,000'),
    createData('David Brown', 'Product Manager', 'Los Angeles', 38, '2020/11/03', '$380,000'),
];

const Tables = () => {
    const [searchTerm, setSearchTerm] = useState('');
    const [filterOffice, setFilterOffice] = useState('all');
    const [filterPosition, setFilterPosition] = useState('all');

    const offices = [...new Set(rows.map(row => row.office))];
    const positions = [...new Set(rows.map(row => row.position))];

    const filteredRows = rows.filter(row => {
        const matchesSearch = Object.values(row)
            .join(' ')
            .toLowerCase()
            .includes(searchTerm.toLowerCase());

        const matchesOffice = filterOffice === 'all' || row.office === filterOffice;
        const matchesPosition = filterPosition === 'all' || row.position === filterPosition;

        return matchesSearch && matchesOffice && matchesPosition;
    });

    return (
        <PageContainer title="Tables" description="Data Tables">
            <Box>
                <Card>
                    <CardContent>
                        <Typography variant="h3" mb={3}>
                            Employee Data
                        </Typography>

                        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} mb={3}>
                            <TextField
                                fullWidth
                                placeholder="Search employees..."
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                                InputProps={{
                                    startAdornment: (
                                        <InputAdornment position="start">
                                            <IconSearch size={20} />
                                        </InputAdornment>
                                    ),
                                }}
                            />

                            <FormControl sx={{ minWidth: 120 }}>
                                <InputLabel>Office</InputLabel>
                                <Select
                                    value={filterOffice}
                                    label="Office"
                                    onChange={(e) => setFilterOffice(e.target.value)}
                                >
                                    <MenuItem value="all">All Offices</MenuItem>
                                    {offices.map(office => (
                                        <MenuItem key={office} value={office}>{office}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>

                            <FormControl sx={{ minWidth: 120 }}>
                                <InputLabel>Position</InputLabel>
                                <Select
                                    value={filterPosition}
                                    label="Position"
                                    onChange={(e) => setFilterPosition(e.target.value)}
                                >
                                    <MenuItem value="all">All Positions</MenuItem>
                                    {positions.map(position => (
                                        <MenuItem key={position} value={position}>{position}</MenuItem>
                                    ))}
                                </Select>
                            </FormControl>
                        </Stack>

                        <TableContainer component={Paper}>
                            <Table sx={{ minWidth: 650 }} aria-label="employee table">
                                <TableHead>
                                    <TableRow>
                                        <TableCell>Name</TableCell>
                                        <TableCell>Position</TableCell>
                                        <TableCell>Office</TableCell>
                                        <TableCell>Age</TableCell>
                                        <TableCell>Start Date</TableCell>
                                        <TableCell>Salary</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {filteredRows.map((row) => (
                                        <TableRow
                                            key={row.name}
                                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                                        >
                                            <TableCell component="th" scope="row">
                                                {row.name}
                                            </TableCell>
                                            <TableCell>
                                                <Chip
                                                    label={row.position}
                                                    size="small"
                                                    sx={{
                                                        backgroundColor: (theme) => theme.palette.primary.light,
                                                        color: (theme) => theme.palette.primary.main,
                                                    }}
                                                />
                                            </TableCell>
                                            <TableCell>{row.office}</TableCell>
                                            <TableCell>{row.age}</TableCell>
                                            <TableCell>{row.startDate}</TableCell>
                                            <TableCell>{row.salary}</TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </CardContent>
                </Card>
            </Box>
        </PageContainer>
    );
};

export default Tables; 