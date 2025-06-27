using Donation.IdentityModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Donation.IdentityServices
{
    internal class AppUserStore : UserStore<AppIdentityUser>
    {
        internal AppUserStore(DbContext context) : base(context)
        {
        }
    }
}
