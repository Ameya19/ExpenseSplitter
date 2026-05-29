using ExpenseSplitter.Core.DTOs.Group;
using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Enums;
using ExpenseSplitter.Core.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ExpenseSplitter.API.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/groups")]
    public class GroupController : ControllerBase
    {
        private readonly IGroupRepository groupRepository;

        public GroupController(IGroupRepository groupRepository)
        {
            this.groupRepository = groupRepository;
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroup([FromBody]CreateGroupDto groupDto)
        {
            var groupDetails = new Group
            {
                Name = groupDto.Name,
                Description = groupDto.Description,
                CreatedByUserId = groupDto.CreatedByUserId,
            };

            var result = await this.groupRepository.CreateGroup(groupDetails);

            var response = new Group
            {
                Name = result.Name,
                Description = result.Description,
                CreatedByUserId = result.CreatedByUserId,
            };

            return Ok(response);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetGroupById([FromRoute] Guid id)
        {
            var result = await this.groupRepository.GetGroupById(id);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                var response = new Group
                {
                    Name = result.Name,
                    Description = result.Description,
                    CreatedByUserId = result.CreatedByUserId,
                    Id = result.Id,
                    Members = result.Members.Select(m => new GroupMember
                    {
                        UserId = m.UserId,
                        GroupId = m.GroupId,
                        Role = m.Role,
                        User = new User
                        {
                            Id = m.User.Id,
                            DisplayName = m.User.DisplayName,
                            Email = m.User.Email
                        }
                    }).ToList(),
                    Expenses = result.Expenses.Select(e => new Expense
                    {
                        Id = e.Id,
                        Amount = e.Amount,
                        PaidBy = e.PaidBy,
                        GroupId = e.GroupId,
                        CreatedAt = e.CreatedAt
                    }).ToList()
                };

                return Ok(response);
            }
        }

        [HttpGet("user/{userId:Guid}")]
        public async Task<IActionResult> GetGroupsByUserId([FromRoute] Guid userId)
        {
            var result = await this.groupRepository.GetGroupsByUserId(userId);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                var response = new List<Group>();
                foreach(Group group in result)
                {
                    var groupDetails = new Group
                    {
                        Name = group.Name,
                        Description = group.Description,
                        CreatedByUserId = group.CreatedByUserId,
                        Id = group.Id
                    };

                    response.Add(groupDetails);
                }

                return Ok(response);
            }
        }

        [HttpPut]
        [Route("{groupId:Guid}")]
        public async Task<IActionResult> UpdateGroupDetails([FromRoute]Guid groupId, [FromBody]UpdateGroupDto groupDetails)
        {
            var updatedGroupDetails = new Group
            {
                Name = groupDetails.Name,
                Description = groupDetails.Description
            };

            var result = await this.groupRepository.UpdateGroupDetails(groupId, updatedGroupDetails);

            if (result == null)
            {
                return NotFound();
            }
            else
            {
                var response = new Group
                {
                    Id = groupId,
                    Name = result.Name,
                    Description = result.Description,
                };

                return Ok(response);
            }
        }

        [HttpDelete]
        [Route("{groupId:Guid}")]
        public async Task<IActionResult> DeleteGroup([FromRoute]Guid groupId)
        {
            var result = await this.groupRepository.DeleteGroup(groupId);

            if(result == true)
            {
                return Ok("Group has been deleted successfully!");
            }
            else
            {
                return NotFound($"Group with {groupId} not found.");
            }
        }

        [HttpPost("{groupId:Guid}/members/{userId:Guid}")]
        public async Task<IActionResult> AddMember([FromRoute]Guid groupId, [FromRoute]Guid userId)
        {
            var result = await this.groupRepository.AddMember(groupId, userId);

            if (!result)
                return BadRequest("User is already a member of this group.");

            return Ok(new
            {
                message = "Member Added successfully!"
            });
        }

        [HttpDelete("{groupId:Guid}/members/{userId:Guid}")]
        public async Task<IActionResult> RemoveMember([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var result = await this.groupRepository.RemoveMember(groupId, userId);

            if (!result)
                return BadRequest("Member not found in this group.");

            return Ok(new
            {
                message = "Member removed successfully."
            });
        }

        [HttpGet("{groupId:Guid}/members/{userId:Guid}/role")]
        public async Task<IActionResult> GetMemberRole([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var member = await this.groupRepository.GetMemberRole(groupId, userId);
            if (member == null)
                return NotFound("Member not found in this group.");
            return Ok(new
            {
                member.UserId,
                member.GroupId,
                Role = member.Role.ToString(),
                IsAdmin = member.Role == GroupRole.Admin
            });
        }
    }
}
