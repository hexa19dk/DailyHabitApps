using AtomicHabits.Models;
using AtomicHabits.Models.DTO;
using AtomicHabits.Repositories;
using Azure;
using System.Net;

namespace AtomicHabits.Services
{
    public interface IDashboardService
    {
        Task<ApiResponse> GetCardOverviews(int userId, CancellationToken ct);
    }

    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepositories _repo;
        private ApiResponse _response;
        public DashboardService(IDashboardRepositories repo)
        {
            _repo = repo;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> GetCardOverviews(int userId, CancellationToken ct)
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var startDate = DateTime.UtcNow.AddMonths(-5).Date;
                var endDate = DateTime.UtcNow.Date;

                var habitsData = await _repo.GetActiveHabits(userId);
                var getTrackings = await _repo.GetTrackings(userId);

                if (habitsData == null || getTrackings == null)
                {
                    _response.IsSuccess = false;
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.ErrorMessages = new List<string> { "No habits or trackings found." };
                    return _response;
                }

                var todayCompleted = getTrackings.Count(t => t.TrackingDate!.Value.Date == today && t.IsCompleted);
                var todayCard = new TodayCardDto
                {
                    Completed = todayCompleted,
                    Total = habitsData.Count
                };

                #region Streak Calculation

                var dateStreaks = getTrackings.Where(t => t.IsCompleted).Select(t => t.TrackingDate!.Value.Date).Distinct().OrderByDescending(d => d).ToList();

                int currentStreak = 0, longestStreak = 0, temp = 1;
                for(int i = 1; i < dateStreaks.Count; i++)
                {
                    if ((dateStreaks[i] - dateStreaks[i - 1]).TotalDays == 1)
                    {
                        temp++;
                    }
                    else
                    {
                        longestStreak = Math.Max(longestStreak, temp);
                        temp = 1;
                    }
                }

                longestStreak = Math.Max(longestStreak, temp);
                if (dateStreaks.LastOrDefault() == today || dateStreaks.LastOrDefault() == today.AddDays(-1))
                {
                    currentStreak = temp;
                }

                var streakCard = new StreakCardDto
                {
                    CurrentStreak = currentStreak,
                    LongestStreak = longestStreak
                };

                #endregion

                #region Weekly Card Calculation

                var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 1);
                var weeklyCompleted = getTrackings.Count(t => t.IsCompleted && t.TrackingDate!.Value.Date >= startOfWeek);
                var weeklyTotal = habitsData.Count * ((today - startOfWeek).Days + 1);

                var weeklyCard = new WeeklyCardDto
                {
                    CompletionRate = weeklyTotal == 0 ? 0 : weeklyCompleted * 100 / weeklyTotal
                };

                #endregion

                #region Monthly Trend Calculation

                var getCompletedHabit = await _repo.GetCompletedTrackingByUser(userId, startDate, endDate, ct);

                var monthlyTrends = getCompletedHabit
                    .GroupBy(t => new { t.TrackingDate!.Value.Year, t.TrackingDate!.Value.Month })
                    .Select(g =>
                    {
                        int daysInMonth = DateTime.DaysInMonth(g.Key.Year, g.Key.Month);
                        int totalPossibleSessions = habitsData.Count() * daysInMonth;

                        int completionRate = totalPossibleSessions == 0 ? 0 : (int)Math.Round((double)g.Count() / totalPossibleSessions * 100);

                        return new MonthlyTrendDto
                        {
                            Month = $"{g.Key.Month:D2}/{g.Key.Year}",
                            CompletionRate = completionRate
                        };
                    })
                    .OrderBy(x => x.Month)
                    .ToList();

                #endregion

                _response.IsSuccess = true;
                _response.StatusCode = HttpStatusCode.OK;
                _response.Result = new
                {
                    TodayCard = todayCard,
                    StreakCard = streakCard,
                    WeeklyCard = weeklyCard,
                    MonthlyTrend = monthlyTrends
                };

                return _response;
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.StatusCode = HttpStatusCode.InternalServerError;
                _response.ErrorMessages = new List<string> { "Post habit progress error, message: " + ex.Message };
                return _response;
            }
        }



    }
}
