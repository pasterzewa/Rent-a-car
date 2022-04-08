using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rent_a_Car.ApiClasses;
using Rent_a_Car.ApiClasses.NewFolder;
using Rent_a_Car.Messenge.ForAlien;
using Rent_a_Car.Messenge.FromAlien;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Rent_a_Car.Controllers
{
    [Route("[controller]")]
    [Authorize]
    public class AlienApiController : ControllerBase
    {

        [HttpGet]
        [AllowAnonymous]
        [Route("Get")]
        public JsonResult Get()
        {
            Vehicles vehicles = new Vehicles();
            string respond = ComunicateWithAlliens.CallToAllien("https://mini.rentcar.api.snet.com.pl/vehicles").Result;
            vehicles = JsonConvert.DeserializeObject<Vehicles>(respond);
            
            return new JsonResult(vehicles);
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("GetPrice/{id}")]
        public JsonResult GetPrice([FromRoute]string id, [FromBody] AskAlienAboutPrice ask)
        {
            ThierPrice Price = new ThierPrice();
            string respond = ComunicateWithAlliens.CallToAllien("https://mini.rentcar.api.snet.com.pl/vehicle/"+ id +"/GetPrice", ask).Result;
            Price = JsonConvert.DeserializeObject<ThierPrice>(respond);

            return new JsonResult(Price);
        }

        [HttpPost]
        [Route("Rent/{id}")]
        public JsonResult RentCar([FromRoute] string id)
        {
            AlienRentInfo RentInfo = new AlienRentInfo();
            string respond = ComunicateWithAlliens.CallToAllien("https://mini.rentcar.api.snet.com.pl/vehicles/Rent/"+id, new { startDate = DateTime.Today }).Result;
            RentInfo = JsonConvert.DeserializeObject<AlienRentInfo>(respond);
            if(RentInfo.startDate != null)
                return new JsonResult("Sukses");
            else
                return new JsonResult("Niepowodzenie");
        }


    }
}
