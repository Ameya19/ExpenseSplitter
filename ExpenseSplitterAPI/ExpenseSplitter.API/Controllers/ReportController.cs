using ExpenseSplitter.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpenseSplitter.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/reports")]
    public class ReportController : ControllerBase
    {
        private readonly IReportRepository reportRepository;

        public ReportController(IReportRepository reportRepository)
        {
            this.reportRepository = reportRepository;
        }

        [HttpGet("group/{groupId:Guid}")]
        public async Task<IActionResult> GetGroupReport([FromRoute] Guid groupId)
        {
            var report = await this.reportRepository.GetGroupReport(groupId);
            if (report == null)
            {
                return NotFound("Report not found!");
            }
            return Ok(report);
        }

        [HttpGet("group/{groupId:Guid}/range")]
        public async Task<IActionResult> GetGroupReportByDateRange([FromRoute] Guid groupId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            if (from > to)
            {
                return BadRequest("Start date must be before end date.");
            }
            var report = await this.reportRepository.GetGroupReportByDateRange(groupId, from, to);
            if (report == null)
            {
                return NotFound("Report not found!");
            }
            return Ok(report);
        }

        [HttpGet("user/{userId:Guid}")]
        public async Task<IActionResult> GetUserReport([FromRoute] Guid userId)
        {
            var report = await this.reportRepository.GetUserReport(userId);
            if (report == null)
            {
                return NotFound("Report not found!");
            }
            return Ok(report);
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetMyReport()
        {
            var userId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);

            var report = await this.reportRepository.GetUserReport(userId);
            if (report == null)
            {
                return NotFound("Report not found!");
            }
            return Ok(report);
        }
    }
}
