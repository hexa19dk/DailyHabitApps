namespace AtomicHabits.Models
{
    public class HabitTracking
    {
        public int Id { get; set; }

        public int HabitId { get; set; }
        public Habit Habit { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public DateTime? TrackingDate { get; set; }
        public bool IsCompleted { get; set; } = false;
        public string? Notes { get; set; }

        // Time Tracking
        public int? TimeSpentMinutes { get; set; }
        public DateTime? CompletedAt { get; set; }

        // Metadata
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }

}
