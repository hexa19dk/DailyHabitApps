using AtomicHabits.Models;
using AtomicHabits.Models.DTO;
using Microsoft.EntityFrameworkCore;

namespace AtomicHabits.Repositories
{
    public interface IHabitRepositories
    {
        Task<Habit?> GetHabitById(int habitId);
        Task<Habit?> GetHabitbyUserHabitId(int userId, int habitId);
        Task<List<Habit>> GetHabitByUserId(int userId);
        Task<bool> PostHabit(HabitDTO habitDto);
        Task<bool> UpdateHabit(int habitId, HabitDTO habitDto);
        Task<bool> DeleteHabit(int habitId);

        Task<List<Habit>> GetActiveHabits(int userId, CancellationToken ct);
        Task<List<HabitTracking>> GetTodayTrackings(List<int> habitIds, DateTime today, CancellationToken ct);
        Task<List<HabitTracking>> GetWeeklyTrackings(List<int> habitIds, DateTime startOfWeek, CancellationToken ct);
        Task<List<HabitTracking>> GetMonthlyTrackings(List<int> habitIds, DateTime startOfMonth, CancellationToken ct);
    }

    public class HabitRepositories : IHabitRepositories
    {
        private readonly AppDbContext _db;
        private readonly ILogger<HabitTrackingRepositories> _logger;
        public HabitRepositories(AppDbContext db, ILogger<HabitTrackingRepositories> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<Habit?> GetHabitById(int habitId)
        {
            try
            {
                return await _db.Habits.FindAsync(habitId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetHabitById] Error");
                throw;
            }
        }

        public async Task<Habit?> GetHabitbyUserHabitId(int userId, int habitId)
        {
            try
            {
                return await _db.Habits
                    .Where(h => h.Id == habitId && h.UserId == userId)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetHabitByUserId] Error");
                throw;
            }
        }

        public async Task<List<Habit>> GetHabitByUserId(int userId)
        {
            try
            {
                var habits = await _db.Habits
                   .Where(h => h.UserId == userId)
                   .ToListAsync();

                return habits;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[GetHabitByUserId] Error");
                throw;
            }
        }

        public async Task<bool> PostHabit(HabitDTO habitDto)
        {
            try
            {
                var habitRequest = new Habit
                {
                    UserId = habitDto.UserId,
                    Name = habitDto.Name,
                    Color = habitDto.Color,
                    Description = habitDto.Description,
                    Frequency = habitDto.Frequency,
                    GoalValue = habitDto.GoalValue,
                    GoalUnit = habitDto.GoalUnit,
                    GoalFrequency = habitDto.GoalFrequency,
                    CreatedAt = habitDto.CreatedAt,
                };

                await _db.Habits.AddAsync(habitRequest);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "[GetHabitByUserId] Error");
                throw;
            }
        }

        public async Task<bool> UpdateHabit(int habitId, HabitDTO habitDto)
        {
            try
            {
                var habit = await _db.Habits.FindAsync(habitId);
                if (habit == null)
                {
                    _logger.LogError($"[UpdateHabit] Error, habit with id: {habitId} not found");
                    return false;
                }

                habit.Name = habitDto.Name;
                habit.Color = habitDto.Color;
                habit.Description = habitDto.Description;
                habit.Frequency = habitDto.Frequency;
                habit.GoalValue = habitDto.GoalValue;
                habit.GoalUnit = habitDto.GoalUnit;
                habit.GoalFrequency = habitDto.GoalFrequency;
                habit.UpdatedAt = habitDto.UpdatedAt;

                _db.Habits.Update(habit);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "[UpdateHabit] Error");
                throw;
            }
        }

        public async Task<bool> DeleteHabit(int habitId)
        {
            try
            {
                var habit = await _db.Habits.FindAsync(habitId);
                if (habit == null)
                {
                    _logger.LogError($"Habit with id: {habitId} not found");
                    return false;
                }

                _db.Habits.Remove(habit);
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "[DeleteHabit] Error");
                throw;
            }
        }

        public async Task<List<Habit>> GetActiveHabits(int userId, CancellationToken ct)
        {
            try
            {
                return await _db.Habits
                      .Where(h => h.UserId == userId && !h.IsArchived)
                      .ToListAsync(ct);
            }
            catch (Exception ex)    
            {                 
                _logger.LogError(ex.Message, "[GetActiveHabits] Error");
                throw;
            }          
        }

        public async Task<List<HabitTracking>> GetTodayTrackings(List<int> habitIds, DateTime today, CancellationToken ct)
        {
            try
            {
                return await _db.HabitTrackings
                    .Where(t => habitIds.Contains(t.HabitId) && t.TrackingDate == today)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "[GetTodayTrackings] Error");
                throw;
            }           
        }

        public async Task<List<HabitTracking>> GetWeeklyTrackings(List<int> habitIds, DateTime startOfWeek, CancellationToken ct)
        {
            try
            {
                return await _db.HabitTrackings
                    .Where(t => habitIds.Contains(t.HabitId) && t.TrackingDate >= startOfWeek)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "[GetWeeklyTrackings] Error");
                throw;
            }          
        }

        public async Task<List<HabitTracking>> GetMonthlyTrackings(List<int> habitIds, DateTime startOfMonth, CancellationToken ct)
        {
            try
            {
                return await _db.HabitTrackings
                    .Where(t => habitIds.Contains(t.HabitId) && t.TrackingDate >= startOfMonth)
                    .ToListAsync(ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "[GetMonthlyTrackings] Error");
                throw;
            }            
        }
    }
}
