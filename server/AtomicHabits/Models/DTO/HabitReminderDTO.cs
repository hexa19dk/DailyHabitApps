namespace AtomicHabits.Models.DTO
{
    public class HabitReminderDTO
    {
        public int HabitId { get; set; }
        public TimeSpan ReminderTime { get; set; }
        public string DaysOfWeek { get; set; }
        public bool IsEnabled { get; set; }
    }
}
