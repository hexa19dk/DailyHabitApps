using AtomicHabits.Models;
using AtomicHabits.Models.DTO;
using AtomicHabits.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace AtomicHabits.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HabitController : ControllerBase
    {
        private readonly AppDbContext _db;
        private ApiResponse _response;
        private readonly IHabitService _habitService;
        private readonly IAuthService _authservice;

        public HabitController(AppDbContext db, IHabitService habitService, IAuthService authservice)
        {
            _db = db;
            _response = new ApiResponse();
            _habitService = habitService;
            _authservice = authservice;
        }

        [Authorize]
        [HttpGet("get-habits/{userId}")]
        public async Task<IActionResult> GetHabits(int userId, CancellationToken ct)
        {
            var res = await _habitService.GetHabits(userId, ct);
            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize]
        [HttpPost("post-habit")]
        public async Task<IActionResult> PostHabit(HabitDTO habitDto)
        {
            var res = await _habitService.PostHabit(habitDto);
            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize]
        [HttpPut("update-habit/{habitId}")]
        public async Task<IActionResult> UpdateHabit(int habitId, [FromBody] HabitDTO habitDto)
        {
            var res = await _habitService.UpdateHabit(habitId, habitDto);
            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize]
        [HttpDelete("delete-habit/{habitId}")]
        public async Task<IActionResult> DeleteHabit(int habitId)
        {
            var res = await _habitService.DeleteHabit(habitId);
            return StatusCode((int)res.StatusCode, res);
        }

        [Authorize]
        [HttpGet("habits-summary/{userId}")]
        public async Task<IActionResult> GetHabitsSummary(int userId)
        {
            var res = await _habitService.HabitSummary(userId);
            return StatusCode((int)res.StatusCode, res);
        }

    }
}
