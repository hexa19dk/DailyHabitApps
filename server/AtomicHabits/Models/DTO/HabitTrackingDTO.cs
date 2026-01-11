namespace AtomicHabits.Models.DTO
{
    public class HabitTrackingDTO
    {
        public int HabitId { get; set; }
        public int UserId { get; set; }
        public DateTime TrackingDate { get; set; }
        public bool IsCompleted { get; set; }
        public string Notes { get; set; }

        public int TimeSpentMinutes { get; set; }
        public DateTime CompletedAt { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
