import Chart from 'react-apexcharts';

const HabitDistributedChart = ({ labels, values }) => {
    const series = [
        {
            name: 'Completion',
            data: values,
        },
    ];

    const options = {
        chart: {
            type: 'bar',
            toolbar: { show: false },
            fontFamily: "'Plus Jakarta Sans', sans-serif",
        },
        plotOptions: {
            bar: {
                distributed: true,
                columnWidth: '45%',
                borderRadius: 6,
            },
        },
        colors: [
            '#5D87FF',
            '#49BEFF',
            '#13DEB9',
            '#FFAE1F',
            '#FA896B',
            '#ECF2FF',
            '#E8F7FF',
        ],
        dataLabels: {
            enabled: false,
        },
        legend: {
            show: false,
        },
        xaxis: {
            categories: labels,
        },
        yaxis: {
            labels: {
                style: {
                    fontSize: '12px',
                },
            },
        },
        tooltip: {
            theme: 'dark',
        },
        grid: {
            borderColor: '#f1f1f1',
        },
    };

    return (
        <Chart
            options={options}
            series={series}
            type="bar"
            height={280}
            width="100%"
        />
    );
};

export default HabitDistributedChart;
