using ExpenseSplitter.Core.DTOs.Settlement;
using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/settlements")]
    public class SettlementController : ControllerBase
    {
        private readonly ISettlementRepository settlementRepository;

        public SettlementController(ISettlementRepository settlementRepository)
        {
            this.settlementRepository = settlementRepository;
        }

        [HttpPost]
        public async Task<IActionResult> RecordSettlement([FromBody] CreateSettlementDto createSettlementDto)
        {
            if (createSettlementDto.Amount <= 0)
            {
                return BadRequest("Amount must be greater than zero.");
            }

            if (createSettlementDto.FromUserId == createSettlementDto.ToUserId)
            {
                return BadRequest("Cannot settle with yourself.");
            }

            var settlement = new Settlement
            {
                GroupId = createSettlementDto.GroupId,
                FromUserId = createSettlementDto.FromUserId,
                ToUserId = createSettlementDto.ToUserId,
                Amount = createSettlementDto.Amount,
                Note = createSettlementDto.Note
            };

            var settlementDetails = await this.settlementRepository.RecordSettlement(settlement);

            if (settlementDetails == null)
            {
                return BadRequest("Failed to record settlement.");
            }
            else
            {
                var response = new SettlementResponseDto
                {
                    Id = settlementDetails.Id,
                    GroupId = settlementDetails.GroupId,
                    Amount = settlementDetails.Amount,
                    Note = settlementDetails.Note,
                    Status = settlementDetails.Status.ToString(),
                    CreatedAt = settlementDetails.CreatedAt,
                };

                return Ok(response);
            }
        }

        [HttpGet("group/{groupId:Guid}")]
        public async Task<IActionResult> GetAllSettlementsByGroupId([FromRoute] Guid groupId)
        {
            var settlements = await this.settlementRepository.GetSettlementsByGroupId(groupId);

            if (settlements == null)
            {
                return BadRequest("No settlements found for the specified group.");
            }
            else
            {
                var response = new List<SettlementResponseDto>();
                foreach (var settlement in settlements)
                {
                    var settlementDto = new SettlementResponseDto
                    {
                        Id = settlement.Id,
                        GroupId = settlement.GroupId,
                        Amount = settlement.Amount,
                        Note = settlement.Note,
                        Status = settlement.Status.ToString(),
                        CreatedAt = settlement.CreatedAt,
                    };

                    response.Add(settlementDto);
                }
                return Ok(response);
            }
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetSettlementById([FromRoute] Guid id)
        {
            var settlementDetails = await this.settlementRepository.GetSettlementById(id);
            if (settlementDetails == null)
            {
                return NotFound("Settlement not found.");
            }
            else
            {
                var response = new SettlementResponseDto
                {
                    Id = settlementDetails.Id,
                    GroupId = settlementDetails.GroupId,
                    Amount = settlementDetails.Amount,
                    Note = settlementDetails.Note,
                    Status = settlementDetails.Status.ToString(),
                    CreatedAt = settlementDetails.CreatedAt,
                };
                return Ok(response);
            }
        }

        [HttpGet("user/{userId:Guid}")]
        public async Task<IActionResult> GetSettlementsByUserId([FromQuery] Guid userId)
        {
            var settlements = await this.settlementRepository.GetSettlementsByUserId(userId);
            if (settlements == null)
            {
                return BadRequest("No settlements found for the specified user.");
            }
            else
            {
                var response = new List<SettlementResponseDto>();
                foreach (var settlement in settlements)
                {
                    var settlementDto = new SettlementResponseDto
                    {
                        Id = settlement.Id,
                        GroupId = settlement.GroupId,
                        Amount = settlement.Amount,
                        Note = settlement.Note,
                        Status = settlement.Status.ToString(),
                        CreatedAt = settlement.CreatedAt,
                    };
                    response.Add(settlementDto);
                }
                return Ok(response);
            }
        }

        [HttpPut("{id:Guid}/complete")]
        public async Task<IActionResult> CompleteSettlement([FromRoute] Guid id)
        {
            var settlementDetails = await this.settlementRepository.CompleteSettlement(id);
            if (settlementDetails == null)
            {
                return NotFound("Settlement not found.");
            }
            else
            {
                var response = new SettlementResponseDto
                {
                    Id = settlementDetails.Id,
                    GroupId = settlementDetails.GroupId,
                    Amount = settlementDetails.Amount,
                    Note = settlementDetails.Note,
                    Status = settlementDetails.Status.ToString(),
                    CreatedAt = settlementDetails.CreatedAt,
                };
                return Ok(response);
            }
        }

        [HttpDelete("{id:Guid}")]
        public async Task<IActionResult> CancelSettlement([FromRoute] Guid id)
        {
            var settlementDetails = await this.settlementRepository.CancelSettlement(id);

            if (settlementDetails == false)
            {
                return NotFound("Settlement not found.");
            }
            else
            {
                return Ok("Settlement cancelled successfully.");
            }
        }
    }
}
