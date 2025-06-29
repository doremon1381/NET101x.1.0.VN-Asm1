using Donation.IdentityModels;
using Donation.IdentityServices;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace Donation.IdentityService
{
    public class IdentityServices : IIdentityServices
    {
        // This class can be used to implement identity-related services.
        // For example, you can add methods for user management, role management, etc.
        // Currently, it is empty and serves as a placeholder for future implementations.
        private AppIdentityDbContext _identityDbContext;
        private AppUserStore _userStore;
        private AppUserManager _userManager;
        private Lazy<RoleManager<IdentityRole>> _roleManager;

        public IdentityServices()
        {
            _identityDbContext = new AppIdentityDbContext();
            _userStore = new AppUserStore(_identityDbContext);
            _userManager = new AppUserManager(_userStore);
            _roleManager = new Lazy<RoleManager<IdentityRole>>(() => new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(_identityDbContext)));
        }

        /// <summary>
        /// TODO; use this for now
        /// </summary>
        public void AddTenUsers()
        {
            // create user role
            //if (_roleManager.IsValueCreated)
            //{
            var role = _roleManager.Value.FindByName(Roles.USER);
            if (role == null)
            {
                var userRole = new IdentityRole()
                {
                    Name = Roles.USER
                };
                _roleManager.Value.Create(userRole);
            }
            //}

            for (int i = 0; i < 10; i++)
            {
                var newUser = new AppIdentityUser()
                {
                    FirstName = $"User {i}",
                    LastName = "Nguyen",
                    UserName = $"user{i}",
                    Email = $"doremon131{i}@gmail.com",
                    PhoneNumber = $"012345678{i}",
                    PasswordHash = Crypto.HashPassword("user123"),
                    Active = false
                };

                var result = _userManager.Create(newUser);

                if (result.Succeeded)
                    _userManager.AddToRole(newUser.Id, _roleManager.Value.FindByName(Roles.USER).Name);
            }
        }

        public ClaimsIdentity CreateIdentity(AppIdentityUser user, string authenticationType = DefaultAuthenticationTypes.ApplicationCookie)
        {
            return _userManager.CreateIdentity(user, authenticationType);
        }

        public bool CreateUserWithRole(AppIdentityUser newUser, string idRole)
        {
            var result = _userManager.Create(newUser);
            if (result.Succeeded)
            {
                var role = _roleManager.Value.FindById(idRole);
                if (role != null)
                {
                    var addToRoleResult = _userManager.AddToRole(newUser.Id, role.Name);
                    return addToRoleResult.Succeeded;
                }
            }

            return false;
        }

        public bool DeleteUser(string id)
        {
            var user = _userManager.FindById(id);

            _userManager.Delete(user);

            return true;
        }

        public AppIdentityUser Find(string username, string password)
        {
            var user = _userManager.Find(username, password);
            //var user = _identityDbContext.Users.Where(u => u.UserName == username && u.PasswordHash == passwordHash && !u.IsDeleted)
            //    .FirstOrDefault();
            if (user == null)
                //|| user.IsDeleted)
                return null;
            return user;
        }

        public IdentityRole FindRole(string id)
        {
            return _identityDbContext.Roles.Find(id);
        }

        public AppIdentityUser FindUserById(string userId)
        {
            //return _identityDbContext.Users.Where(u => !u.IsDeleted).First(u => u.Id == userId);
            return _userManager.FindById(userId);
        }

        public List<AppIdentityUser> GetAllUsers()
        {
            //return _identityDbContext.Users.Where(u => !u.IsDeleted).ToList();
            return _userManager.Users.ToList();
        }

        public List<IdentityRole> GetRoles()
        {
            return _roleManager.Value.Roles.ToList();
        }

        public IdentityRole GetUserRole(string userId)
        {
            var user = _identityDbContext.Users.Find(userId);

            if (user == null)
                //|| user.IsDeleted)
                // TODO
                throw new Exception("User not found!");

            var userRole = _roleManager.Value.FindById(user.Roles.First().RoleId);

            if (userRole == null)
                // TODO
                throw new Exception("User role not found");

            return userRole;
        }

        public bool IsEmailExist(string email)
        {
            //return _identityDbContext.Users.Where(u => !u.IsDeleted).Any(u => u.Email == email);
            return _identityDbContext.Users.Any(u => u.Email == email);
        }

        public bool IsUserNameExist(string userName)
        {
            //return _identityDbContext.Users.Where(u => !u.IsDeleted).Any(u => u.UserName == userName);
            return _userManager.Users.Any(u => u.UserName == userName);
        }

        public List<AppIdentityUser> SearchForUsers(string phoneNumberOrEmail)
        {
            // TODO: will think about it later
            //string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            phoneNumberOrEmail = phoneNumberOrEmail.ToLower();

            if (string.IsNullOrEmpty(phoneNumberOrEmail))
                return _identityDbContext.Users.ToList();
            else if (phoneNumberOrEmail.IsEmail())
            {
                return new List<AppIdentityUser>() { _userManager.FindByEmailAsync(phoneNumberOrEmail).Result };
            }
            else
                return _identityDbContext.Users.Where(u => u.PhoneNumber.Contains(phoneNumberOrEmail) || u.Email.ToLower().Contains(phoneNumberOrEmail)).ToList();
        }

        public bool UpdateUser(AppIdentityUser user)
        {
            try
            {
                _userManager.Update(user);

                return true;
            }
            catch (Exception)
            {
                // TODO
                return false;
            }
        }

        public bool UpdateUserWithRole(AppIdentityUser appIdentityUser, string idRole)
        {
            try
            {
                var user = _identityDbContext.Users.Find(appIdentityUser.Id);

                if (user == null )
                    //|| user.IsDeleted)
                    return false;
                user.FirstName = appIdentityUser.FirstName;
                user.LastName = appIdentityUser.LastName;
                user.PhoneNumber = appIdentityUser.PhoneNumber;
                user.Address = appIdentityUser.Address;

                _identityDbContext.Users.AddOrUpdate(user);

                var role = _roleManager.Value.FindById(idRole);
                if (user.Roles.First().RoleId != role.Id)
                {
                    // remove old role
                    var oldRoles = _userManager.GetRoles(user.Id);
                    foreach (var oldRole in oldRoles)
                        if (oldRole != null)
                        {
                            var result = _userManager.RemoveFromRole(user.Id, oldRole);
                            if (!result.Succeeded)
                                throw new Exception();
                        }

                    _userManager.AddToRole(user.Id, role.Name);
                }

                return true;
            }
            catch (Exception)
            {
                // TODO:
                return false;
            }
        }
    }

    public interface IIdentityServices
    {
        ClaimsIdentity CreateIdentity(AppIdentityUser user, string authenticationType = DefaultAuthenticationTypes.ApplicationCookie);
        AppIdentityUser Find(string username, string password);
        AppIdentityUser FindUserById(string userId);
        List<AppIdentityUser> SearchForUsers(string usernameOrEmail);
        IdentityRole FindRole(string id);
        /// <summary>
        /// TODO: for seed data
        /// </summary>
        void AddTenUsers();
        List<IdentityRole> GetRoles();
        bool UpdateUserWithRole(AppIdentityUser appIdentityUser, string idRole);
        bool UpdateUser(AppIdentityUser user);
        bool CreateUserWithRole(AppIdentityUser newUser, string idRole);
        bool IsEmailExist(string email);
        IdentityRole GetUserRole(string userId);
        bool DeleteUser(string id);
        List<AppIdentityUser> GetAllUsers();
        bool IsUserNameExist(string userName);
    }
}
