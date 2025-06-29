using Assignment1.Extensions;
using Assignment1.Validation;
using Assignment1.ViewModel;
using Donation.IdentityModels;
using Donation.IdentityService;
using Donation.IdentityServices;
using Donation.Models;
using Donation.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Assignment1.Controllers
{
    //[Authorize]
    public class AccountController : Controller
    {
        private readonly IIdentityServices _identityServices;
        private readonly IDonationServices _donationServices;

        public AccountController(IIdentityServices identityServices, IDonationServices donationServices)
        {
            _identityServices = identityServices;
            _donationServices = donationServices;
        }

        // GET: Account
        [AdminOnlyValidation]
        public ActionResult Index(string phoneNumberOrEmail = "", string sortColumn = nameof(AccountUsersVM.FullName)
            , string iconClass = ViewExtensions.FaSortDesc, int pageCount = 5, int pageNumber = 1)
        {
            //if (!User.IsInRole(Roles.ADMIN))
            //    throw new Exception("User is not an admin!");

            var users = _identityServices.SearchForUsers(phoneNumberOrEmail.Trim())
                .Select(user => new AccountUsersVM()
                {
                    UserId = user.Id,
                    FirstName = user.FirstName ?? "",
                    LastName = user.LastName ?? "",
                    FullName = $"{user.FirstName} {user.LastName}",
                    Email = user.Email ?? "",
                    PhoneNumber = user.PhoneNumber ?? "",
                    Address = user.Address ?? "",
                    UserName = user.UserName,
                    Role = _identityServices.FindRole(user.Roles.First().RoleId)?.Name ?? "No Role Assigned",
                    Active = user.Active,
                    Notes = user.Notes ?? ""
                });

            // Apply sorting
            users = ApplySortingUsers(sortColumn, iconClass, users);

            //users = users.ToList();

            //ViewBag.Users = users;
            ViewBag.IconClass = iconClass;
            ViewBag.SortColumn = sortColumn;
            ViewBag.Roles = new SelectList(_identityServices.GetRoles(), "Id", "Name");

            ViewBag.Search = phoneNumberOrEmail;

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageCount = pageCount;
            ViewBag.PageNumberSelections = new SelectList(new List<SelectListItem>()
            {
                new SelectListItem() { Text = "5", Value = "5" },
                new SelectListItem() { Text = "10", Value = "10" },
                new SelectListItem() { Text = "15", Value = "15" }
            }, "Value", "Text", pageCount.ToString());
            ViewBag.TotalPages = (int)Math.Ceiling((double)users.Count() / pageCount);

            ViewBag.Users = users.Skip((pageNumber - 1) * pageCount).Take(pageCount).ToList();

            return View();
        }

        private static IEnumerable<AccountUsersVM> ApplySortingUsers(string sortColumn, string iconClass, IEnumerable<AccountUsersVM> users)
        {
            Func<AccountUsersVM, object> sortFunc = a => a.FullName;
            if (sortColumn == nameof(AccountUsersVM.UserName))
            {
                sortFunc = a => a.UserName;
            }
            else if (sortColumn == nameof(AccountUsersVM.Email))
            {
                sortFunc = a => a.Email;
            }
            else if (sortColumn == nameof(AccountUsersVM.PhoneNumber))
            {
                sortFunc = a => a.PhoneNumber;
            }
            else if (sortColumn == nameof(AccountUsersVM.Role))
            {
                sortFunc = a => a.Role;
            }
            else if (sortColumn == nameof(AccountUsersVM.Active))
            {
                sortFunc = a => a.Active;
            }

            users = iconClass == ViewExtensions.FaSortDesc
                    ? users.OrderByDescending(sortFunc)
                    : users.OrderBy(sortFunc);

            return users;
        }

        public ActionResult Login()
        {
            ViewBag.Message = "Login Page";
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountLoginVM loginVM)
        {
            try
            {
                var user = _identityServices.Find(loginVM.UserName, loginVM.Password)
                    ?? throw new Exception("Wrong username or password");

                // Sign in the user
                var userIdentity = _identityServices.CreateIdentity(user);
                userIdentity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Sid, user.Id));
                userIdentity.AddClaim(new System.Security.Claims.Claim("IsActive", user.Active.ToString()));

                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                authenticationManager.SignIn(userIdentity);

                if (((ClaimsIdentity)userIdentity).Claims.Where(c => c.Type == ClaimTypes.Role).First().Value == Roles.ADMIN)
                {
                    // Redirect to admin page
                    return RedirectToAction("Index", "Account", new { area = "" });
                }
                else
                    return RedirectToAction("UserView", "Donation", new { area = "" });
            }
            catch (Exception ex)
            {
                // TODO
                ModelState.AddModelError("Error", ex);
                ViewBag.Error = ex.Message;
                return View();
            }

        }

        [Authorize]
        public ActionResult Logout()
        {
            var authenticationManager = HttpContext.GetOwinContext().Authentication;
            authenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return Redirect("Login");
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult Update(string firstName, string lastName, string phoneNumber, string address, string idUser, string idRole, string redirectTo = "Index")
        {
            if (((ClaimsIdentity)User.Identity).FindFirst(c => c.Type == ClaimTypes.Sid).Value != idUser
            && !User.IsInRole(Roles.ADMIN))
                throw new Exception("That's the wrong way to do!");

            var user = _identityServices.FindUserById(idUser)
                ?? throw new Exception("User not found");

            _identityServices.UpdateUserWithRole(new AppIdentityUser()
            {
                Id = idUser,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phoneNumber,
                Address = address,
                Active = user.Active
            }, idRole);

            // I intent to separate the user profile from the identity user
            // identity will be used for authentication and authorization
            _donationServices.UpdateUserProfile(new UserProfile()
            {
                UserId = idUser,
                FullName = $"{firstName} {lastName}",
                Email = user.Email,
                PhoneNumber = phoneNumber,
                Address = address
            });

            return Redirect(redirectTo);
        }


        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult Lock(string idUser)
        {
            // Only admin can lock and unlock account
            var user = _identityServices.FindUserById(idUser);

            if (user != null)
            {
                user.Active = false;
                _identityServices.UpdateUser(user);
            }

            return Redirect("Index");

        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult Unlock(string idUser)
        {
            // Only admin can lock and unlock account
            var user = _identityServices.FindUserById(idUser);

            if (user != null)
            {
                user.Active = true;
                _identityServices.UpdateUser(user);
            }

            return Redirect("Index");
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult AddUser(string firstName, string lastName, string email, string phoneNumber, string address, string userName, string password, string idRole)
        {
            // Only admin can create new users
            if (_identityServices.IsEmailExist(email))
                throw new Exception("This email is existed!");
            if (_identityServices.IsUserNameExist(userName))
                throw new Exception("Username has been used!");

            // create for identity server
            var newUser = new AppIdentityUser()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address,
                UserName = userName,
                PasswordHash = Crypto.HashPassword(password),
                Active = true
            };

            var result = _identityServices.CreateUserWithRole(newUser, idRole);

            // create user profile
            var userProfile = _donationServices.CreateUserProfile(new UserProfile()
            {
                UserId = newUser.Id,
                FullName = $"{firstName} {lastName}",
                Email = email,
                PhoneNumber = phoneNumber,
                Address = address
            });

            return Redirect("Index");
        }

        [Authorize]
        public ActionResult Details()
        {
            // This action can be used to show details of a specific user.
            var user = _identityServices.FindUserById(User.Identity.GetUserId());
            if (user == null)
            {
                // Handle the case where the user is not found
                return HttpNotFound("User not found");
            }

            var userRole = _identityServices.GetUserRole(user.Id).Name;
            //var userRole = 

            ViewBag.Roles = new SelectList(_identityServices.GetRoles(), "Id", "Name");
            ViewBag.UserRole = userRole;

            return View(user);
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult Delete(string idUser)
        {
            // Only admin can create new users
            _identityServices.DeleteUser(idUser);
            _donationServices.SoftDeleteUserProfile(idUser);

            if (idUser.Equals(User.Identity.GetUserId()))
                return RedirectToAction("Logout");

            return Redirect("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult SendMail(string note, string idUser)
        {
            var user = _identityServices.FindUserById(idUser)
                ?? throw new Exception();
            //if (user == null)
            //{
            //    // Handle the case where the user is not found
            //    throw new Exception();
            //}

            SendEmail(note, user);

            return Redirect("Index");
        }

        private static void SendEmail(string note, AppIdentityUser user)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(MailKitSettings.NAME, MailKitSettings.EMAILID));
            // TODO: test email for now
            email.To.Add(new MailboxAddress(user.UserName, user.Email));

            email.Subject = "Testing out email sending";
            // $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                //Text = $"<b>Hello all the way from the land of C# {callbackUrl}</b>"
                Text = note
            };

            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                smtp.Connect(MailKitSettings.HOST, 587, false);

                // Note: only needed if the SMTP server requires authentication
                smtp.Authenticate(MailKitSettings.EMAILID, MailKitSettings.PASSWORD);

                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}