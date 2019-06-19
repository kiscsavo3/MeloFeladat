using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Model
{
    public class Role : IdentityRole<Guid>
    {

        public Role()
        {

        }

        public Role(string roleName): base(roleName)
        {

        }
    }
}
