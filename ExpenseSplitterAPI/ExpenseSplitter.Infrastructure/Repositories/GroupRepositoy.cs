using ExpenseSplitter.Core.Entities;
using ExpenseSplitter.Core.Interfaces.Repositories;
using ExpenseSplitter.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Infrastructure.Repositories
{
    public class GroupRepositoy : IGroupRepository
    {
        private readonly AppDbContext appDbContext;

        public GroupRepositoy(AppDbContext appDbContext) 
        {
            this.appDbContext = appDbContext;
        }

        public async Task<bool> AddMember(Guid groupId, Guid userId)
        {
            //Check if member exists.
            var checkIfAlreadyExists = await this.appDbContext.GroupMembers.AnyAsync(m => m.GroupId == groupId && m.UserId == userId);

            if (checkIfAlreadyExists)
            {
                return false;
            }

            var member = new GroupMember
            {
                GroupId = groupId,
                UserId = userId,
                Role = Core.Enums.GroupRole.Member,
                JoinedAt = DateTime.UtcNow
            };

            await appDbContext.GroupMembers.AddAsync(member);
            return true;
        }

        public async Task<Group> CreateGroup(Group group)
        {
            this.appDbContext.Groups.Add(group);
            await this.appDbContext.SaveChangesAsync();
            return group;
        }

        public async Task<bool> DeleteGroup(Guid groupId)
        {
            var groupDetails = await this.appDbContext.Groups.FirstOrDefaultAsync(x => x.Id ==  groupId);

            if (groupDetails != null)
            {
                groupDetails.IsDeleted = true;
                groupDetails.UpdatedAt = DateTime.UtcNow;

                await appDbContext.SaveChangesAsync();
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<Group?> GetGroupById(Guid id)
        {
            return await this.appDbContext.Groups.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<Group>> GetGroupsByUserId(Guid userId)
        {
            return await this.appDbContext.Groups.Where(x => x.CreatedByUserId == userId).ToListAsync();
        }

        public async Task<bool> RemoveMember(Guid groupId, Guid userId)
        {
            var member = await this.appDbContext.GroupMembers.FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

            if(member == null)
            {
                return false;
            }

            appDbContext.GroupMembers.Remove(member);
            await this.appDbContext.SaveChangesAsync();
            return true;
        }

        public async Task<Group?> UpdateGroupDetails(Guid groupId, Group updatedGroupDetails)
        {
            var groupDetails = await this.appDbContext.Groups.FirstOrDefaultAsync(x =>x.Id == groupId);

            if (groupDetails != null)
            {
                groupDetails.Name = updatedGroupDetails.Name;
                groupDetails.Description = updatedGroupDetails.Description;

                await this.appDbContext.SaveChangesAsync();
                return groupDetails;
            }
            else
            {
                return null;
            }
        }
    }
}
