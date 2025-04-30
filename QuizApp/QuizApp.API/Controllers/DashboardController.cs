using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using QuizApp.QuizApp.Core.Entities;
using QuizApp.QuizApp.Core.Interfaces;
using QuizApp.QuizApp.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace QuizApp.QuizApp.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
      private readonly IDashboardService _dashboardService;
      private readonly ILogger<DashboardController> _logger;

      public DashboardController(IDashboardService dashboardService, ILogger<DashboardController> logger)
      {
        _dashboardService = dashboardService;
        _logger = logger;
      }

      [HttpGet("stats/{userId}")]
      [Authorize]
      public async Task<IActionResult> GetUserStats(int userId)
      {
        try
        {
          var stats = await _dashboardService.GetUserStatsAsync(userId);
          return Ok(stats);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error fetching user stats");
          return StatusCode(500, "Internal server error");
        }
      }

      [HttpGet("stats/quizzes/{userId}")]
      [Authorize]
      public async Task<IActionResult> GetQuizsStats(int userId)
      {
        try
        {
          var stats = await _dashboardService.GetQuizsStatsAsync(userId);
          return Ok(stats);
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "Error fetching quizs stats");
          return StatusCode(500, "Internal server error");
        }
      }
    }
}