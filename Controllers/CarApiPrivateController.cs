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
using Rent_a_Car.Messenge.FromCustomer;
using System.IO;
using Azure.Storage.Blobs;

namespace Rent_a_Car.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class CarApiPrivateController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CarApiPrivateController(ApplicationDbContext context,
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;

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
                return new JsonResult(_context.CarDetails.Select(c => c).Where(c => c.CarID == carModelID));
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
        public async Task<ActionResult> Rent([FromBody] ReturnData returnData)
        {
            int carDetailsID = returnData.carDetailsID;
            var expectedReturnDate = returnData.expectedReturnDate;
            var carToRent = _context.CarDetails.SingleOrDefault(c => c.CarDetailsID == carDetailsID);

            string clientStringID = _context.Users.Where(c => c.UserName == returnData.email).Select(c => c.Id).FirstOrDefault();
            int clientID = _context.Customer.Where(c => c.AspNetUserID == clientStringID).Select(c => c.CustomerID).FirstOrDefault();
            if (carToRent == null)
            {
                return NotFound();
            }
            if (!carToRent.IsAvailable)
            {
                return BadRequest();
            }
            var rentedCarData = RentController.RentACar(_context, carDetailsID, clientID, expectedReturnDate);
            if (rentedCarData == null) return NotFound();

            var result = new JsonResult(rentedCarData);
            result.StatusCode = 200;
            return result;
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
        [Route("GetRentedCars/{email}")]
        public ActionResult CheckRentedCar([FromRoute] string email)
        {
            string clientStringID = _context.Users.Where(c => c.UserName == email).Select(c => c.Id).FirstOrDefault();
            int clientID = _context.Customer.Where(c => c.AspNetUserID == clientStringID).Select(c => c.CustomerID).FirstOrDefault();
            return new JsonResult(_context.RentCar.Where(r => r.CustomerID == clientID && r.IsReturned == false).Select((c) => new {carBrand = c.CarDetails.Car.Brand, carModel = c.CarDetails.Car.Model, carDetailsID = c.CarDetailsID, rentToken = c.RentCarEventID , rentDate = c.SubmitDate.ToShortDateString(), expedtedReturnDate  = c.MaximumReturnDate.ToShortDateString()}));
            
        }

        [HttpGet]
        [Route("GetAccountType/{userName}")]
        public JsonResult GetAccountType([FromRoute] string userName)
        {
            var user = _context.Users.FirstOrDefault(c => c.UserName == userName);
            return new JsonResult(_context.Users.Where(c => c.UserName == userName).Select(c => new
            {
                accountType = c.AccountType
            }));
        }

        [HttpPost]
        [Route("AddReturnFile")]
        public JsonResult AddReturnFile([FromForm] ReturnFormMessege fille)
        {
            ReturnFile returnFile = new ReturnFile();
            returnFile.ReturnFileID = fille.ReturnFileID;
            returnFile.RentedCarID = fille.RentedCarID;
            returnFile.ReturnDate = fille.ReturnDate;
            returnFile.CarConditon = fille.CarConditon;
            returnFile.OdometerReading = fille.OdometerReading;
            using (var ms = new MemoryStream())
            {
                fille.Photo.CopyTo(ms);
                returnFile.Photo = ms.ToArray();
            }
            using (var ms = new MemoryStream())
            {
                fille.ReturnProocol.CopyTo(ms);
                returnFile.ReturnProocol = ms.ToArray();
            }

            _context.ReturnFile.Add(returnFile);
            _context.SaveChanges();

            if (ReturnController.ReturnACar(_context, fille.ReturnFileID))
            {
                FileUploadForm form1 = new FileUploadForm();
                form1.File = fille.Photo;
                form1.IsPhoto = true;
                form1.RentID = returnFile.ReturnFileID;

                var result1 = UploadFile(form1);

                FileUploadForm form2 = new FileUploadForm();
                form2.File = fille.ReturnProocol;
                form2.IsProtocol = true;
                form2.RentID = returnFile.ReturnFileID;
                var result2 = UploadFile(form2);

                return new JsonResult("Pomyślnie zwrócono samochód");
            }
            else
            {
                _context.ReturnFile.Remove(returnFile);
                _context.SaveChanges(true);
                return new JsonResult("Bład przy zwrocie");
            }

        }

        /// <summary>
        /// Zwraca listę wszystkich aut gotowych do zwrotu
        /// </summary>
        [HttpGet]
        [Route("ReadyToReturn")]
        public JsonResult ReadyToReturn()
        {
            var dbcontext = _context.RentCar.Where(a => a.IsReturned == false)
                .Select(a => new
                {
                    RentID = a.RentCarEventID,
                    Brand = a.CarDetails.Car.Brand,
                    Model = a.CarDetails.Car.Model,
                    carID = a.CarDetails.CarID,
                    CustomerEmail = a.Customer.Email
                });
            return new JsonResult(dbcontext);
        }

        /// <summary>
        /// Zwraca Historie wyporzyczeń
        /// </summary>
        [HttpGet]
        [Route("History")]
        public async Task<JsonResult> History()
        {

            var returnFiles = _context.ReturnFile.Select(a => new
            {
                returnFileID = a.ReturnFileID,
                rentedCarModel = _context.RentCar.Where(c => c.RentCarEventID == a.ReturnFileID).FirstOrDefault().CarDetails.Car.Model,
                rentedCarBrand = _context.RentCar.Where(c => c.RentCarEventID == a.ReturnFileID).FirstOrDefault().CarDetails.Car.Brand,
                rentedCarID = a.RentedCarID,
                clientMail = (from returnFile in _context.ReturnFile
                              join ren in _context.RentCar on returnFile.ReturnFileID equals ren.RentCarEventID
                              join customer in _context.Customer on ren.CustomerID equals customer.CustomerID
                              join asp in _context.Users on customer.AspNetUserID equals asp.Id
                              where a.ReturnFileID == returnFile.ReturnFileID
                              select asp.Email).FirstOrDefault(),
                returnDate = a.ReturnDate,
                employerName = a.Employer.Name,
                carConditon = a.CarConditon
            }); ;
            return new JsonResult(returnFiles);
        }
        /// <summary>
        /// Pobierz samochody które zwróciłeć
        /// </summary>
        /// <remarks>GetRentedCars
        /// Trzeba być zalogowanym
        /// </remarks>
        /// <returns>Listę wypożyczonych samochodów, oraz tokeny wymagane do ich zwrotu</returns>
        /// <response code="200">Jeżeli jest się autoryzowanym</response> 


        [HttpGet]
        [Route("GetReturnedCar/{email}")]
        public ActionResult CheckReturnedCar([FromRoute] string email)
        {
            string clientStringID = _context.Users.Where(c => c.UserName == email).Select(c => c.Id).FirstOrDefault();
            int clientID = _context.Customer.Where(c => c.AspNetUserID == clientStringID).Select(c => c.CustomerID).FirstOrDefault();
            return new JsonResult(_context.RentCar.Where(r => r.CustomerID == clientID && r.IsReturned == true).
                                    Select((c) => new { carDetailsID = c.CarDetailsID, rentToken = c.RentCarEventID, rentDate = c.SubmitDate.ToShortDateString(), carBrand = c.CarDetails.Car.Brand, carModel = c.CarDetails.Car.Model }));
        }

        [HttpGet]
        [Route("CheckUserData/{email}")]
        public ActionResult CheckIfUserProvidedData([FromRoute] string email)
        {
            Customer customer = null;
            customer = _context.Customer.Where(c => c.Email == email).FirstOrDefault();
            
            if (customer == null)
            {
                JsonResult result = new JsonResult(null);
                result.StatusCode = 404;
                return result;
            }
            else
            {  
                JsonResult result = new JsonResult(new CustomerData( customer));
                result.ContentType = "application/json";
                return result; 
            }
        }

        [HttpPost]
        [Route("FetchPriceAndCreateCustomerWithData")]
        public ActionResult FetchPriceAndCreateCustomerWithData([FromBody] CustomerData customerData)
        {
            if (customerData == null)return new StatusCodeResult(404);
            var aspNetUser = _context.Users.Where(c => c.Email == customerData.Email).FirstOrDefault();
            if(aspNetUser == null)
            {
                return new StatusCodeResult(400);
            }
            var alreadyCustomer = _context.Customer.Where(c=>c.Email == customerData.Email).FirstOrDefault();
            if(alreadyCustomer == null)
            {
                var customer = new Customer()
                {
                    Name = customerData.Name,
                    Surname = customerData.Surname,
                    Email = customerData.Email,
                    BecoamingDriverDate = customerData.BecoamingDriverDate,
                    BirtheDate = customerData.BirtheDate,
                    City = customerData.City,
                    Street = customerData.Street,
                    StreetNumber = customerData.StreetNumber,
                    Poste_Code = customerData.Poste_Code,
                    AspNetUserID = aspNetUser.Id
                };
                _context.Customer.Add(customer);
                _context.SaveChanges();

            }

            return GetPrice(customerData);
            
        }

        [HttpPost]
        [Route("GetPrice")]
        public JsonResult GetPrice([FromBody] CustomerData question)
        {
            if (question == null)
                return new JsonResult(null);
            var dbcontext = _context;
            var detail = dbcontext.CarDetails.Where(cd => cd.CarDetailsID == question.carDetalisID).ToList();

            if (detail.Count == 1)
                return new JsonResult(Math.Round(((double)detail[0].Price + (double)detail[0].CarDetailsID / 10), 2));
            else
                return new JsonResult(null);

        }

        [HttpPost]
        [Route("UploadFile")]
        public async Task<ActionResult> UploadFile([FromBody] FileUploadForm fileUploadForm)
        {
            if (!fileUploadForm.isModal())
            {
                return new StatusCodeResult(400);
            }
            
            if(!_context.RentCar.Where(c => c.RentCarEventID == fileUploadForm.RentID).Any())
            {
                return new StatusCodeResult(400);
            }
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
            
            if (connectionString == null) return new StatusCodeResult(500);

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = null; 
            try
            {
                containerClient = blobServiceClient.GetBlobContainerClient("rentacarblob1");
            }
            catch (Exception ex)
            {

            }

            string fileName = fileUploadForm.RentID + "_"+ fileUploadForm.File.Name;

            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            using (var ms = new MemoryStream())
            {
                try
                {
                    
                    fileUploadForm.File.CopyTo(ms);
                    ms.Position = 0;
                    var response = blobClient.Upload(ms);
                    if (fileUploadForm.IsProtocol)
                    {
                        _context.ReturnFile.Where(c => c.ReturnFileID == fileUploadForm.RentID).SingleOrDefault().ReturnProocolBlobName = fileName;
                    }
                    else
                    {
                        _context.ReturnFile.Where(c => c.ReturnFileID == fileUploadForm.RentID).SingleOrDefault().PhotoBlobName = fileName;
                    }
                    _context.SaveChanges(); 
                    return new JsonResult(response);
                }catch (Exception ex)
                {
                    return new StatusCodeResult(400);
                }
                
            }

        }

        [HttpGet]
        [Route("DownloadFile")]
        public async Task<IActionResult> DownloadFile([FromBody] FileDownloadForm form)
        {
            if (!form.isModal())
            {
                return new StatusCodeResult(400);
            }

            if (!_context.RentCar.Where(c => c.RentCarEventID == form.RentID).Any())
            {
                return new StatusCodeResult(400);
            }
            string connectionString = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

            if (connectionString == null) return new StatusCodeResult(500);

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = null;
            try
            {
                containerClient = blobServiceClient.GetBlobContainerClient("rentacarblob1");
            }
            catch (Exception ex)
            {

            }

            string fileName = form.IsPhoto ? _context.ReturnFile.Where(c => c.ReturnFileID == form.RentID).Select(c => c.PhotoBlobName).FirstOrDefault() :
                _context.ReturnFile.Where(c => c.ReturnFileID == form.RentID).Select(c => c.ReturnProocolBlobName).FirstOrDefault();
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            var memoryStream = new MemoryStream();

            blobClient.DownloadTo(memoryStream);
            var length = memoryStream.Length;

            return File(memoryStream, "application/octet-stream", fileName);
        }
    }
}
