using ExpenseSplitter.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.Interfaces.Repositories
{
    public interface IGroupRepository
    {
        public Task<Group> CreateGroup(Group group);
        public Task<Group?> GetGroupById(Guid id);
        public Task<IEnumerable<Group>> GetGroupsByUserId(Guid userId);
        public Task<Group?> UpdateGroupDetails(Guid groupId, Group updatedGroupDetails);
        public Task<bool> DeleteGroup(Guid groupId);
        public Task<bool> AddMember(Guid groupId, Guid userId);
        public Task<bool> RemoveMember(Guid groupId, Guid userId);
    }
}
