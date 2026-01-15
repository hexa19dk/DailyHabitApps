namespace AtomicHabits.Models.DTO
{
    public class HabitDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public string Description { get; set; }
        public string Frequency { get; set; }
        public int GoalValue { get; set; }
        public string GoalUnit { get; set; }
        public string GoalFrequency { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
