using Donation.IdentityModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donation.IdentityServices
{
    internal class AppUserManager : UserManager<AppIdentityUser>
    {
        internal AppUserManager(IUserStore<AppIdentityUser> store) : base(store)
        {
        }
    }
}
