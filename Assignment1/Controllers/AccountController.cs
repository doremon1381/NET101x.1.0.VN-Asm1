using Assignment1.Extensions;
using Assignment1.ViewModel;
using Donation.IdentityModels;
using Donation.IdentityService;
using Donation.IdentityServices;
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
        private IIdentityServices _identityServices;

        public AccountController(IIdentityServices identityServices)
        {
            _identityServices = identityServices;
        }

        // GET: Account
        [Authorize(Users = Roles.ADMIN)]
        public ActionResult Index(string phoneNumberOrEmail = "", string sortColumn = "FullName"
            , string iconClass = "fa-sort-desc", int pageCount = 5, int pageNumber = 1)
        {
            if (!User.IsInRole(Roles.ADMIN))
                throw new Exception("User is not an admin!");

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

            if (sortColumn == nameof(AccountUsersVM.FullName))
            {
                users = iconClass == ViewExtensions.SortDesc
                    ? users.OrderByDescending(p => p.FullName)
                    : users.OrderBy(p => p.FullName);
            }
            else if (sortColumn == nameof(AccountUsersVM.UserName))
            {
                users = iconClass == ViewExtensions.SortDesc
                    ? users.OrderByDescending(p => p.UserName)
                    : users.OrderBy(p => p.UserName);
            }
            else if (sortColumn == nameof(AccountUsersVM.Email))
            {
                users = iconClass == ViewExtensions.SortDesc
                    ? users.OrderByDescending(p => p.Email)
                    : users.OrderBy(p => p.Email);
            }
            else if (sortColumn == nameof(AccountUsersVM.PhoneNumber))
            {
                users = iconClass == ViewExtensions.SortDesc
                    ? users.OrderByDescending(p => p.PhoneNumber)
                    : users.OrderBy(p => p.PhoneNumber);
            }
            else if (sortColumn == nameof(AccountUsersVM.Role))
            {
                users = iconClass == ViewExtensions.SortDesc
                    ? users.OrderByDescending(p => p.Role)
                    : users.OrderBy(p => p.Role);
            }
            else if (sortColumn == nameof(AccountUsersVM.Active))
            {
                users = iconClass == ViewExtensions.SortDesc
                    ? users.OrderByDescending(p => p.Active)
                    : users.OrderBy(p => p.Active);
            }

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
                var user = _identityServices.Find(loginVM.UserName, loginVM.Password);
                if (user == null)
                    throw new Exception("Wrong username or password");

                // Sign in the user
                var userIdentity = _identityServices.CreateIdentity(user);
                userIdentity.AddClaim(new System.Security.Claims.Claim(ClaimTypes.Sid, user.Id));

                var authenticationManager = HttpContext.GetOwinContext().Authentication;
                authenticationManager.SignIn(userIdentity);

                return RedirectToAction("Index", "Home", new { area = "" });
            }
            catch (Exception ex)
            {
                // TODO
                ModelState.AddModelError("Error", ex);
                ViewBag.Error = ex.Message;
                return View("Error");
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
        public ActionResult Update(string firstName, string lastName, string email, string phoneNumber, string userName, string address, string idUser, string idRole, string redirectTo = "Index")
        {
            try
            {
                if (((ClaimsIdentity)User.Identity).FindFirst(c => c.Type == ClaimTypes.Sid).Value != idUser
                && !User.IsInRole(Roles.ADMIN))
                    throw new Exception("That's the wrong way to do!");

                var user = _identityServices.FindUserById(idUser);
                if (user == null)
                    throw new Exception("User not found");

                _identityServices.UpdateUserWithRole(new AppIdentityUser()
                {
                    Id = idUser,
                    FirstName = firstName,
                    LastName = lastName,
                    PhoneNumber = phoneNumber,
                    Address = address,
                    Active = user.Active
                }, idRole);

                return Redirect(redirectTo);
            }
            catch (Exception)
            {
                // TODO:
                return View("Error");
            }
        }


        [HttpPost]
        [Authorize]
        public ActionResult Lock(string idUser)
        {
            // Only admin can lock and unlock account
            if (!User.IsInRole(Roles.ADMIN))
                return View("Error");

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
        public ActionResult Unlock(string idUser)
        {
            // Only admin can lock and unlock account
            if (!User.IsInRole(Roles.ADMIN))
                return View("Error");

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
        public ActionResult AddUser(string firstName, string lastName, string email, string phoneNumber, string address, string userName, string password, string idRole)
        {
            // Only admin can create new users
            if (!User.IsInRole(Roles.ADMIN))
                return Redirect("Index");

            if (!_identityServices.IsEmailExist(email))
            {
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
            }
            else
            {
                // TODO: show dialog into view

                return null;
            }

            return Redirect("Index");
        }

        [Authorize]
        public ActionResult Details()
        {
            try
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
            catch (Exception)
            {
                // TODO: 
                return View("Error");
            }
        }

        [HttpPost]
        public ActionResult Delete(string idUser)
        {
            try
            {
                // Only admin can create new users
                if (!User.IsInRole(Roles.ADMIN))
                    throw new Exception();

                _identityServices.DeleteUser(idUser);

                if (idUser.Equals(User.Identity.GetUserId()))
                    return RedirectToAction("Logout");

                return Redirect("Index");
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult SendMail(string note, string idUser)
        {
            try
            {
                var user = _identityServices.FindUserById(idUser);
                if (user == null)
                {
                    // Handle the case where the user is not found
                    throw new Exception();
                }

                SendEmail(note, user);

                return Redirect("Index");
            }
            catch (Exception)
            {
                return View("Error");
            }

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