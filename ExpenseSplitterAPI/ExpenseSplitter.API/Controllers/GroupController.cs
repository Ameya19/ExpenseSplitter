using ExpenseSplitter.Core.DTOs.Group;
using ExpenseSplitter.Core.Entities;
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
                    Id = result.Id
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

            return Ok("Member added successfully.");
        }

        [HttpDelete("{groupId:Guid}/members/{userId:Guid}")]
        public async Task<IActionResult> RemoveMember([FromRoute] Guid groupId, [FromRoute] Guid userId)
        {
            var result = await this.groupRepository.RemoveMember(groupId, userId);

            if (!result)
                return BadRequest("Member not found in this group.");

            return Ok("Member removed successfully.");
        }
    }
}
