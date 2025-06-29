using Assignment1.Extensions;
using Assignment1.Validation;
using Assignment1.ViewModel;
using Donation.IdentityService;
using Donation.IdentityServices;
using Donation.Models;
using Donation.Services;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Assignment1.Controllers
{
    [Authorize]
    public class DonationController : Controller
    {
        private readonly IDonationServices _donationServices;
        private readonly IIdentityServices _identityServices;

        public DonationController(IIdentityServices identityServices, IDonationServices donationServices)
        {
            // Initialize any services or repositories here if needed
            _donationServices = donationServices;
            _identityServices = identityServices;
        }

        // GET: Donation
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult Index(string searching = "", string sortColumn = nameof(Donation.Models.Donation.Code)
                , string iconClass = ViewExtensions.FaSortDesc, int pageCount = 5, int pageNumber = 1)
        {
            try
            {
                // TODO
                //_donationServices.CreateSampleDonation();

                // add donation plans
                var donationPlans = ApplyDonationSorting(sortColumn, iconClass, _donationServices.GetDonationPlans());

                ViewBag.Search = searching;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageCount = pageCount;
                ViewBag.IconClass = iconClass;
                ViewBag.PageNumberSelections = new SelectList(new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "5", Value = "5" },
                    new SelectListItem() { Text = "10", Value = "10" },
                    new SelectListItem() { Text = "15", Value = "15" }
                }, "Value", "Text", pageCount.ToString());


                donationPlans = donationPlans.Where(d => d.Status.ToString().ToLower().Contains(searching.ToLower())
                || d.OrganizationPhone.ToLower().Contains(searching.ToLower())
                || d.OrganizationName.ToLower().Contains(searching.ToLower())
                || d.Code.ToLower().Contains(searching.ToLower())
                || d.Title.ToLower().Contains(searching.ToLower()));

                pageNumber = pageNumber == 0 ? 0 : pageNumber - 1;
                ViewBag.TotalPages = (int)Math.Ceiling((double)donationPlans.Count() / pageCount);

                ViewBag.DonationPlans = donationPlans
                    .Skip(pageNumber * pageCount)
                    .Take(pageCount).ToList();

                return View();
            }
            catch (Exception)
            {
                return View("Error");
            }
        }

        private static IEnumerable<Donation.Models.Donation> ApplyDonationSorting(string sortColumn, string iconClass, IEnumerable<Donation.Models.Donation> donationPlans)
        {
            Func<Donation.Models.Donation, object> sortFunc = d => d.Code;
            // Sorting logic
            if (sortColumn.Equals(nameof(Donation.Models.Donation.Title)))
                sortFunc = d => d.Title;
            else if (sortColumn.Equals(nameof(Donation.Models.Donation.StartDate)))
                sortFunc = d => d.StartDate;
            else if (sortColumn.Equals(nameof(Donation.Models.Donation.EndDate)))
                sortFunc = d => d.EndDate;
            else if (sortColumn.Equals(nameof(Donation.Models.Donation.TotalMoney)))
                sortFunc = d => d.TotalMoney;
            else if (sortColumn.Equals(nameof(Donation.Models.Donation.OrganizationName)))
                sortFunc = d => d.OrganizationName;
            else if (sortColumn.Equals(nameof(Donation.Models.Donation.OrganizationPhone)))
                sortFunc = d => d.OrganizationPhone;
            else if (sortColumn.Equals(nameof(Donation.Models.Donation.Status)))
                sortFunc = d => d.Status;

            return iconClass.Equals(ViewExtensions.FaSortAsc)
                ? donationPlans.OrderBy(sortFunc)
                : donationPlans.OrderByDescending(sortFunc);
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult CreateDonationPlan(string code, string title, DateTime startDate, DateTime endDate, string organization, string organizationPhone, string desciption)
        {
            var donation = new Donation.Models.Donation()
            {
                Code = code,
                Title = title,
                StartDate = startDate,
                EndDate = endDate,
                OrganizationName = organization,
                OrganizationPhone = organizationPhone,
                Description = desciption,
                CreatedAt = DateTime.Now
            };

            _donationServices.CreateDonation(donation);

            return Redirect("Index");
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult UpdateDonationPlan(string donationId, string code, string title, DateTime startDate, DateTime endDate, string organizationName, string phone, string content)
        {
            var donation = new Donation.Models.Donation
            {
                Id = donationId,
                Code = code,
                Title = title,
                StartDate = startDate,
                EndDate = endDate,
                OrganizationName = organizationName,
                OrganizationPhone = phone,
                Description = content
                //Content = content
            };

            _donationServices.UpdateDonation(donation);

            return Redirect("Index");
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult UpdateDonationStatus(string donationId, string donationStatus)
        {
            var result = _donationServices.UpdateDonationStatus(donationId, donationStatus);

            return Redirect("Index");
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult DeleteDonation(string donationId)
        {
            var donation = _donationServices.SoftDelete(donationId);

            return RedirectToAction("Index");
        }

        [Authorize]
        [AdminOnlyValidation]
        public ActionResult DonationDetails(string id, string searching = "", string sortColumn = nameof(UserDonationVM.UserFullName)
            , string iconClass = ViewExtensions.FaSortDesc, int pageCount = 5, int pageNumber = 1)
        {
            // TODO: seedData
            //var users = _identityServices.GetAllUsers()
            //    .Select(u => new UserProfile()
            //    {
            //        UserId = u.Id,
            //        Address = u.Address,
            //        FullName = u.FirstName + " " + u.LastName,
            //        PhoneNumber = u.PhoneNumber,
            //        Email = u.Email,
            //        CreatedAt = u.CreateAt
            //    });
            //_donationServices.CreateSampleUserDonations(id);

            var donation = _donationServices.FindById(id, userDonationsRequired: true);

            ViewBag.Search = searching;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageCount = pageCount;
            ViewBag.IconClass = iconClass;
            ViewBag.SortColumn = sortColumn;
            ViewBag.PageNumberSelections = new SelectList(new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "5", Value = "5" },
                    new SelectListItem() { Text = "10", Value = "10" },
                    new SelectListItem() { Text = "15", Value = "15" }
                }, "Value", "Text", pageCount.ToString());

            var userDonations = donation.UserDonations.Select(u => new UserDonationVM()
            {
                Id = u.Id,
                UserFullName = u.UserId != null ? _identityServices.FindUserById(u.UserId)?.FullName : "Unknown User",
                Money = u.Money,
                CreatedAt = u.CreatedAt,
                Note = u.Note,
                Status = u.Status
            });

            userDonations = ApplyUserDonationSorting(sortColumn, iconClass, userDonations);

            pageNumber = pageNumber == 0 ? 0 : pageNumber - 1;
            ViewBag.TotalPages = (int)Math.Ceiling((double)userDonations.Count() / pageCount);

            userDonations = userDonations
                .Where(u => u.UserFullName.ToLower().Contains(searching.ToLower())
                || u.Status.ToString().ToLower().Contains(searching.ToLower())
                // TODO
                //|| u.Money.Equals(Convert.ToDecimal(searching))
                || u.Note.Contains(searching))
                .Skip(pageNumber * pageCount)
                .Take(pageCount).ToList();

            ViewBag.UserDonations = userDonations;

            return View(donation);
        }

        private IEnumerable<UserDonationVM> ApplyUserDonationSorting(string sortColumn, string iconClass, IEnumerable<UserDonationVM> userDonationVMs)
        {
            Func<UserDonationVM, object> sortFunc = u => u.UserFullName;

            if (sortColumn == nameof(UserDonationVM.Money))
                sortFunc = u => u.Money;
            else if (sortColumn == nameof(UserDonationVM.CreatedAt))
                sortFunc = u => u.CreatedAt;
            else if (sortColumn == nameof(UserDonationVM.Status))
                sortFunc = u => u.Status;
            else if (sortColumn == nameof(UserDonationVM.Note))
                sortFunc = u => u.Note;

            if (iconClass.Equals(ViewExtensions.FaSortAsc))
                userDonationVMs = userDonationVMs.OrderBy(sortFunc);
            else
                userDonationVMs = userDonationVMs.OrderByDescending(sortFunc);

            return userDonationVMs;
        }

        [HttpPost]
        [Authorize]
        [AdminOnlyValidation]
        public ActionResult SetUserDonationStatus(string id, string donationId, string status)
        {
            var userDonation = _donationServices.UpdateUserDonation(id, status);

            return Redirect($"donationDetails?id={donationId}");

        }

        #region user
        [Authorize]
        public ActionResult UserView(string direction, int pageNumber = 0)
        {
            var donations = _donationServices.GetDonationPlans();

            int pageCount = 5;
            int lastPage = (int)Math.Ceiling((double)donations.Count() / pageCount);

            if (direction == "prev")
                pageNumber = pageNumber <= 0 ? 0 : pageNumber - 1;
            else if (direction == "next")
                pageNumber = pageNumber > lastPage - 1 ? lastPage - 1 : pageNumber + 1;

            donations = donations.Skip(pageNumber * pageCount).Take(pageCount);

            ViewBag.DonationPlans = donations.ToList();
            ViewBag.LastPage = lastPage;
            ViewBag.PageNumber = pageNumber;

            return View();
        }

        [Authorize]
        public ActionResult UserDonationDetails(string donationId)
        {
            var userDonations = _donationServices.FindUserDonationByDonationId(donationId)
                .Select(u => new UserDonationVM 
                {
                    Id = u.Id,
                    Money = u.Money,
                    Note = u.Note,
                    CreatedAt = u.CreatedAt,
                    Status = u.Status,
                    UserFullName = u.UserId != null ? _identityServices.FindUserById(u.UserId)?.FullName : "Unknown User"
                });
            var donationPlan = _donationServices.FindById(donationId);

            ViewBag.UserDonations = userDonations.ToList();
            ViewBag.Donation = donationPlan;

            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateNewUserDonation(string currentPath, string idDonation, string name, decimal money, string note)
        {
            var user = User;
            //var userProfile = _donationServices.FindUserProfleById()

            return Redirect(currentPath);
        }
        #endregion
    }
}