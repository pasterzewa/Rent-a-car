using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rent_a_Car.Data;
using Rent_a_Car.Models;
using Rent_a_Car.ApiClasses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using static IdentityServer4.IdentityServerConstants;

using Rent_a_Car.MessegeForCustomer;
using Rent_a_Car.Messenge.FromCustomer;

namespace Rent_a_Car.Controllers
{
    [ApiController]
    //[Authorize(LocalApi.PolicyName)]
    [Route("[controller]")]
    public class CarApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CarApiController(ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Loguje sie do api
        /// </summary>
        /// <remarks>
        /// Logowanie do API
        /// </remarks>
        /// <returns>Status zalogowania</returns>
        /// <response code="200">Udane logowanie</response>
        /// <response code="400">Użytkownik jest już zalogowany</response>
        /// <response code="401">Nieudane logowanie</response>  
        /// <response code="403">Konto bez uprawnień</response>
        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<IActionResult> Login([FromForm]string email, [FromForm] string password)
        {
            if (User.Identity.IsAuthenticated)
            {
                return StatusCode(400);
            }
            else
            {
                var result = await _signInManager.PasswordSignInAsync(email, password, false, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return StatusCode(200);
                }
            }
            return StatusCode(401);
        }
        

        /// <summary>
        /// Zwraca listę wszystkich marek samochodów
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("CarModels")]
        public IEnumerable<Car> Get()
        {
            return _context.Car.ToList();
        }

        /// <summary>
        /// Zwraca listę wszystkich dostępnych samochodów o danym modelu
        /// </summary>
        /// <remarks>
        /// Numer modelu należy pobrać z /Api/CarModels
        /// </remarks>
        /// <returns>Szczegóły dotyczące pojedynczych samochodów i dane wymagane do wypożyczenia</returns>
        /// <response code="200">Zwraca listę samochodów w danym modelu</response>
        /// <response code="404">Jeśli dany numer modelu jest nieprawidłowy</response>  
        [HttpGet]
        [AllowAnonymous]
        [Route("Car/{carModelID}")]
        public ActionResult Get([FromRoute] int carModelID)
        {
            try
            {
                return new JsonResult(_context.CarDetails.Select(c => c).Where(c=> c.CarID == carModelID));
            }
            catch
            {
                return NotFound();
            }
        }
        /// <summary>
        /// Wypożyczenie samochodu
        /// </summary>
        /// <remarks>
        /// Żeby wypożyczyć samochód wymagane jest bycie zalogowanym - osobą wypożyczającą jest osoba zalogowana
        /// Przykładowe zapytanie
        ///
        ///     POST
        ///     {
        ///        "carDetailsID": 1,
        ///        "expectedReturnDate": "2022-01-22",
        ///     }
        ///
        /// </remarks>
        /// <returns>Dane dotyczące wypożyczenia - w tym token wypożyczenia</returns>
        /// <response code="400">Jeśli samochód już jest wypożyczony</response>  
        /// <response code="404">Jeśli nie ma takiego ID samochodu</response>
        [HttpPost]
        [Route("Rent")]
        public ActionResult Rent([FromBody] ReturnData returnday)
        {
            var carToRent = _context.CarDetails.SingleOrDefault(c => c.CarDetailsID == returnday.carDetailsID);
            if (carToRent == null)
            {
                return NotFound();
            }
            if (!carToRent.IsAvailable)
            {
                return BadRequest();
            }
            var rentedCarData = RentController.RentACar(_context, returnday.carDetailsID, 1, returnday.expectedReturnDate);
            if (rentedCarData == null) return NotFound();

            //var result = new JsonResult(rentedCarData);
            //result.StatusCode = 200;
            //return result;

            // zwraca sukses
            return new JsonResult("Sukces");
        }

        /// <summary>
        /// Zwrot wypożyczonego samochodu
        /// </summary>
        /// <remarks>
        /// Należy być zalogowanym.
        /// </remarks>
        /// <response code="200">Zwrot został wykonany prawidłowo</response>
        /// <response code="400">Zwrot się nie powiódł</response>  
        [HttpPost]
        [Route("Return/{rentalID}")]
        public ActionResult ReturnCar([FromRoute] string rentalID)
        {
            if (ReturnController.ReturnACar(_context, rentalID))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Pobierz samochody które masz wypożyczone
        /// </summary>
        /// <remarks>
        /// Trzeba być zalogowanym
        /// </remarks>
        /// <returns>Listę wypożyczonych samochodów, oraz tokeny wymagane do ich zwrotu</returns>
        /// <response code="200">Jeżeli jest się autoryzowanym</response> 
        /// <response code="401">Jeżeli nei jest się autoryzowanym</response>
        [HttpGet]
        [Route("GetRentedCars")]
        public ActionResult CheckRentedCar()
        {
            int ClientID = 1;
            return new JsonResult(_context.RentCar.Where(r => r.CustomerID == ClientID && r.IsReturned == false).
                                    Select((c)=> new {carDetailsID = c.CarDetailsID , rentToken = c.RentCarEventID}));
        }

        

        /// <summary>
        /// Pobierz cenne za dzien za samochud danej firmy
        /// <remarks>
        /// Body: QuestionAboutPrice
        ///
        [HttpPost]
        [Route("GetPrice")]
        public JsonResult GetPrice([FromBody] CustomerData question)
        {
            if (question == null)
                return new JsonResult(null);
            var dbcontext = _context;
            var detail =  dbcontext.CarDetails.Where(cd => cd.CarDetailsID == question.carDetalisID).ToList();

            if (detail.Count == 1)
                return new JsonResult(Math.Round(((double)detail[0].Price +  (double)detail[0].CarDetailsID/10),2));
            else
                return new JsonResult(null);

        }

        /// <summary>
        /// Zwraca listę wszystkich aut gotowych do zwrotu
        /// </summary>
        [HttpGet]
        [Route("ReadyToReturn")]
        public JsonResult ReadyToReturn()
        {
            var dbcontext = _context.RentCar.Where(a => a.IsReturned == false)
                .Select( a=> new
                {
                    RentID = a.RentCarEventID,
                    Brand = a.CarDetails.Car.Brand,
                    Model = a.CarDetails.Car.Model,
                    carID = a.CarDetails.CarID,
                    CustomerID = a.CustomerID
                });
            return new JsonResult(dbcontext);
        }

        /// <summary>
        /// Zwraca Historie wyporzyczeń
        /// </summary>
        [HttpGet]
        [Route("History")]
        public  JsonResult History()
        {
            var dbcontext = _context.ReturnFile.Select(a=> new 
            { 
                ReturnFileID = a.ReturnFileID,
                Car = a.RentCarEvent.CarDetails.Car,
                employerName = a.Employer.Name,
                ReturnDate = a.ReturnDate,
                ClientMail = a.RentCarEvent.Customer.Email,
                CarCondition = a.CarConditon
            });

            return new JsonResult(dbcontext);
        }

        /// <summary>
        /// Pobiera protokół
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [Route("DowlandProtocol")]
        public FileResult DowlandProtocol(string id)
        {
            var rp = _context.ReturnFile.Where(a => a.ReturnFileID == id).First().ReturnProocol;
            var protocol = File(rp, "text/pdf") ;

            return protocol;
        }


    }
}
