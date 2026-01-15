using AtomicHabits.Models;
using AtomicHabits.Models.DTO;
using AtomicHabits.Repositories;
using Azure;
using Paket;
using System.Net;

namespace AtomicHabits.Services
{
    public interface IHabitService
    {
        Task<ApiResponse> GetHabits(int userId, string token, CancellationToken cancellationToken);
        Task<ApiResponse> PostHabit(HabitDTO habitDto);
        Task<ApiResponse> UpdateHabit(int habitId, HabitDTO habitDto);
        Task<ApiResponse> DeleteHabit(int habitId);
        Task<ApiResponse> HabitSummary(int userId);
    }

    public class HabitService : IHabitService
    {
        private readonly IHabitRepositories _repo;
        private readonly AppDbContext _db;
        private ApiResponse _response;
        private readonly ILogger<HabitService> _log;
        private readonly IAuthService _authService;
        public HabitService(IHabitRepositories repo, AppDbContext db, ILogger<HabitService> log, IAuthService authService) 
        { 
            _repo = repo;
            _db = db;
            _response = new ApiResponse();
            _log = log;
            _authService = authService;
        }

        public async Task<ApiResponse> GetHabits(int userId, string token, CancellationToken ct)
        {
            try
            {
                var user = await _authService.GetCurrentUserFromJwt(token);
                if (user == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.Unauthorized;
                    _response.ErrorMessages = new List<string> { $"user is unahtorized " };
                    return _response;
                }

                var habits = await _repo.GetHabitByUserId(userId);

                if (habits == null || !habits.Any())
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "No habits found for this user." };
                    return _response;
                }

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = habits;
                return _response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"GetHabits failed for userId {userId}", userId);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { "GetAn unexpected error occurred while retrieving habits. " + ex.Message };
                return _response;
            }
        }

        public async Task<ApiResponse> PostHabit(HabitDTO habitDto)
        {
            try
            {
                if (habitDto == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.ErrorMessages = new List<string> { "Habit data is required." };
                    return _response;
                }

               var createdHabit = await _repo.PostHabit(habitDto);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = new { HabitDTO = habitDto };
                return _response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Post habit failed, payload: {habitDto}");
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { "Post habit error, message: " + ex.Message };
                return _response;
            }
        }

        public async Task<ApiResponse> UpdateHabit(int habitId, HabitDTO habitDto)
        {
            try
            {
                var habit = await _repo.GetHabitById(habitId);
                if (habit == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "Habit not found." };
                    return _response;
                }

                var updHabit = await _repo.UpdateHabit(habitId, habitDto);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = habit;
                return _response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Update habit failed, payload: {habitDto}");
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { "Update habit error, message: " + ex.Message };
                return _response;
            }
        }

        public async Task<ApiResponse> DeleteHabit(int habitId)
        {
            try
            {
                var habit = await _repo.DeleteHabit(habitId);

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = $"Habit with ID {habitId} deleted successfully.";
                return _response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Delete habit failed, habit id: {habitId}");
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { "Delete habit error, message: " + ex.Message };
                return _response;
            }
        }

        public async Task<ApiResponse> HabitSummary(int userId)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
                var startOfMonth = new DateTime(today.Year, today.Month, 1);

                var habits = await _repo.GetActiveHabits(userId, CancellationToken.None);
                var habitIds = habits.Select(h => h.Id).ToList();

                var todayTrackings = await _repo.GetTodayTrackings(habitIds, today, CancellationToken.None);
                var weekTrackings = await _repo.GetWeeklyTrackings(habitIds, startOfWeek, CancellationToken.None);
                var monthTrackings = await _repo.GetMonthlyTrackings(habitIds, startOfMonth, CancellationToken.None);

                int completedToday = todayTrackings.Count(t => t.IsCompleted);
                int habitsToday = habits.Count;
                int todayRate = habitsToday == 0 ? 0 : (completedToday * 100 / habitsToday);


                int completedThisWeek = weekTrackings.Count(t => t.IsCompleted);
                int totalSessionsThisWeek = ((today - startOfWeek).Days + 1) * habits.Count;
                int weeklyRate = totalSessionsThisWeek == 0 ? 0 : (completedThisWeek * 100 / totalSessionsThisWeek);


                int completedThisMonth = monthTrackings.Count(t => t.IsCompleted);
                int totalSessionsThisMonth = today.Day * habits.Count;
                int monthlyRate = totalSessionsThisMonth == 0 ? 0 : (completedThisMonth * 100 / totalSessionsThisMonth);


                int healthScore = (todayRate + weeklyRate + monthlyRate) / 3;

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = new HabitSummaryDto
                {
                    TodaySummary = new TodaySummaryDto
                    {
                        HabitsToday = habitsToday,
                        CompletedToday = completedToday,
                        TodayCompletionRate = todayRate
                    },
                    WeeklySummary = new WeeklySummaryDto
                    {
                        WeeklyCompletionRate = weeklyRate,
                        TotalCompletedThisWeek = completedThisWeek
                    },
                    MonthlySummary = new MonthlySummaryDto
                    {
                        MonthlyCompletionRate = monthlyRate,
                        TotalMonthlySessions = completedThisMonth
                    },
                    HabitHealthScore = healthScore
                };


                return _response;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"Get habit summary failed, message: " + ex.Message);
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string>
                {
                    "Get habits summary error, message: " + ex.Message
                };
                return _response;
            }
        }
    }
}
