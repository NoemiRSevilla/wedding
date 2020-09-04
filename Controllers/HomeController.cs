using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Wedding.Models;
using Google.Maps;
using Google.Maps.Geocoding;

namespace Wedding.Controllers
{
    public class HomeController : Controller
    {
        private MyContext dbContext;

        // here we can "inject" our context service into the constructor
        public HomeController(MyContext context)
        {
            dbContext = context;
        }
        [Route("")]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Route("create")]
        public IActionResult Create(User newUser)
        {
            if (ModelState.IsValid)
            {
                if (dbContext.users.Any(u => u.Email == newUser.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use!");
                    return View("Index");
                }
                PasswordHasher<User> Hasher = new PasswordHasher<User>();
                newUser.Password = Hasher.HashPassword(newUser, newUser.Password);
                dbContext.users.Add(newUser);
                dbContext.SaveChanges();
                HttpContext.Session.SetString("LoggedIn", "LoggedIn");
                HttpContext.Session.SetInt32("UserId", newUser.UserId);


                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == newUser.Email);

                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [Route("login")]
        [HttpPost]
        public IActionResult Login(LoginUser user)
        {
            if (ModelState.IsValid)
            {

                var userInDb = dbContext.users.FirstOrDefault(u => u.Email == user.LoginEmail);
                if (userInDb == null)
                {

                    ModelState.AddModelError("LoginEmail", "Invalid Email/Password");
                    return View("login");
                }
                var hasher = new PasswordHasher<LoginUser>();


                var result = hasher.VerifyHashedPassword(user, userInDb.Password, user.LoginPassword);


                if (result == 0)
                {

                    ModelState.AddModelError("LoginPassword", "Invalid Email/Password");
                    return View("login");

                }
                HttpContext.Session.SetString("LoggedIn", "LoggedIn");
                HttpContext.Session.SetInt32("UserId", userInDb.UserId);
                return RedirectToAction("Dashboard");
            }
            else
            {
                return View("Index");
            }
        }

        [Route("logout")]
        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        [Route("Dashboard")]
        [HttpGet]
        public IActionResult Dashboard(int id)
        {



            List<WeddPlan> AllWedds = dbContext.weddPlans
            .Include(r => r.GuestList)
            .ThenInclude(u => u.User)
            .ToList();

            ViewBag.weddings = AllWedds;

            ViewBag.User = (int)HttpContext.Session.GetInt32("UserId");
            // foreach (var x in dbContext.weddPlans)
            // {
            //     System.Console.WriteLine("//////////////////");
            //     System.Console.WriteLine(x.WeddingPlanner);
            // }
            return View();
        }

        [Route("newWedding")]
        [HttpGet]
        public IActionResult NewWedding()
        {
            return View();
        }

        [Route("addWedding")]
        [HttpPost]
        public IActionResult AddWedding(WeddPlan newWedding)
        {
            if (ModelState.IsValid)
            {
            newWedding.WeddingPlanner = (int)HttpContext.Session.GetInt32("UserId");
            dbContext.weddPlans.Add(newWedding);
            dbContext.SaveChanges();
            return RedirectToAction("DisplayWedd", new { id = newWedding.WeddPlanId });
            }
            return View("NewWedding");
        }

        [Route("DisplayWedd/{id}")]
        [HttpGet]
        public IActionResult DisplayWedd(int id)
        {
            // List<User> AllUsers = dbContext.users.ToList();
            // ViewBag.Users = AllUsers;
            WeddPlan theWedding = dbContext.weddPlans
            .Include(r => r.GuestList)
            .ThenInclude(u => u.User)
            .FirstOrDefault(w => w.WeddPlanId == id);

            //always need to use YOUR_API_KEY for requests.  Do this in App_Start.
            GoogleSigned.AssignAllServices(new GoogleSigned("AIzaSyAbZ0qvAraIoYDv8iwIXXTRTqgwhvv6eso"));

            var request = new GeocodingRequest();
            request.Address = theWedding.Address;
            var response = new GeocodingService().GetResponse(request);

            //The GeocodingService class submits the request to the API web service, and returns the
            //response strongly typed as a GeocodeResponse object which may contain zero, one or more results.

            //Assuming we received at least one result, let's get some of its properties:
            if (response.Status == ServiceResponseStatus.Ok && response.Results.Count() > 0)
            {
                var result = response.Results.First();

                Console.WriteLine("Full Address: " + result.FormattedAddress);         // "1600 Pennsylvania Ave NW, Washington, DC 20500, USA"
                Console.WriteLine("Latitude: " + result.Geometry.Location.Latitude);   // 38.8976633
                Console.WriteLine("Longitude: " + result.Geometry.Location.Longitude); // -77.0365739
                Console.WriteLine();
                ViewBag.Lati = result.Geometry.Location.Latitude;
                ViewBag.Long = result.Geometry.Location.Longitude;

            }
            else
            {
                Console.WriteLine("Unable to geocode.  Status={0} and ErrorMessage={1}", response.Status, response.ErrorMessage);
            }

            return View("DisplayWedd", theWedding);
        }

        [Route("addRsvp/{id}")]
        [HttpPost]
        public IActionResult AddRsvp(RSVP newRSVP, int id)
        {

            

            newRSVP.WeddPlanId = (int)id;
            newRSVP.UserId = (int)HttpContext.Session.GetInt32("UserId");
            dbContext.rsvps.Add(newRSVP);
            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }

        [Route("removeRsvp/{id}")]
        [HttpPost]
        public IActionResult RemoveRsvp(int id)
        {
            IEnumerable<RSVP> guests = dbContext.rsvps.Where(a => a.WeddPlanId == id);
            RSVP flaker = guests.SingleOrDefault(user => user.UserId == (int)HttpContext.Session.GetInt32("UserId"));

            dbContext.rsvps.Remove(flaker);


            dbContext.SaveChanges();


            return RedirectToAction("Dashboard");
        }

        [Route("delete/{id}")]
        [HttpPost]
        public IActionResult Delete(int id)
        {
            WeddPlan RetrievedWedding = dbContext.weddPlans.SingleOrDefault(w => w.WeddPlanId == id);
            dbContext.weddPlans.Remove(RetrievedWedding);

            dbContext.SaveChanges();
            return RedirectToAction("Dashboard");
        }
    }
}
