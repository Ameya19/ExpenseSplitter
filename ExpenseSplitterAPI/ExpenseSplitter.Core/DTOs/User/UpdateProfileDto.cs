using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseSplitter.Core.DTOs.User
{
    public class UpdateProfileDto
    {
            public string? DisplayName { get; set; } = string.Empty;
            public string? AvatarUrl { get; set; }
    }
}
