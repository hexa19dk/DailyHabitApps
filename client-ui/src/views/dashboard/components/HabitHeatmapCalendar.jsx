import React from 'react';
import Chart from 'react-apexcharts';
import {
    Card,
    CardContent,
    Typography
} from '@mui/material';


const getDaysInMonth = (year, month) => new Date(year, month, 0).getDate();

const buildHeatmapSeries = (data, year, month) => {
    const daysInMonth = getDaysInMonth(year, month);
    const weeks = [[], [], [], [], [], []];


    for (let day = 1; day <= daysInMonth; day++) {
        const date = new Date(year, month - 1, day);
        const weekIndex = Math.floor((day + date.getDay()) / 7);

        weeks[weekIndex].push({
            x: day.toString(),
            y: data[day] || 0
        });
    }

    return weeks
        .filter(w => w.length)
        .map((week, i) => ({
            name: `Week ${i + 1}`,
            data: week
        }));
};

const HabitHeatmapCalendar = ({ data = {} }) => {
    const year = new Date().getFullYear();
    const month = new Date().getMonth() + 1;
    const series = buildHeatmapSeries(data, year, month);

    console.log('Heatmap Series:', series);

    const options = {
        chart: {
            type: 'heatmap',
            height: 250,
            toolbar: { show: false },
            fontFamily: "'Plus Jakarta Sans', sans-serif",
        },
        plotOptions: {
            heatmap: {
                shadeIntensity: 0.5,
                radius: 4,
                useFillColorAsStroke: false,
                colorScale: {
                    ranges: [
                        { from: 0, to: 0, color: '#eef2eeff', name: 'No activity' },
                        { from: 1, to: 1, color: '#FFB200', name: 'Done' },
                        { from: 2, to: 3, color: '#FF0000', name: 'High' },
                    ]
                }
            }
        },
        dataLabels: { enabled: false },
        xaxis: {
            type: 'category',
            labels: { show: true }
        },
        yaxis: {
            labels: { show: true }
        },
        tooltip: {
            theme: 'dark',
            y: {
                formatter: (val) => val === 0 ? 'No completion' : `${val} completion(s)`
            }
        }
    };


    return (
        <Card>
            <CardContent>
                <Typography variant="h5" mb={2}>
                    Habit Heatmap (This Month)
                </Typography>

                <Chart
                    options={options}
                    series={series}
                    type="heatmap"
                    height={250}
                />
            </CardContent>
        </Card>
    );
};


export default HabitHeatmapCalendar;