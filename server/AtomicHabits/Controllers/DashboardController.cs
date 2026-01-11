using AtomicHabits.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AtomicHabits.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _service;
        public DashboardController(IDashboardService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpGet("habit-card-overviews")]
        public async Task<IActionResult> GetCardOverview(int userId, CancellationToken ct)
        {
            var res = await _service.GetCardOverviews(userId, ct);
            return StatusCode((int)res.StatusCode, res);
        }
    }
}
