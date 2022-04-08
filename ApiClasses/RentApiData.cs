using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rent_a_Car.Models;
namespace Rent_a_Car.ApiClasses
{
    public class RentApiData
    {
        public int RentedCarID { get; set; }
        public System.DateTime RentDate { get; set; }
        public int CustomerID { get; set; }
        public string RentalIdentifier { get; set; }

        public static RentApiData FromRentEvent(RentCarEvent rentEvent){
            RentApiData result = new();
            result.RentedCarID = rentEvent.CarDetailsID;
            result.RentDate = DateTime.Now;
            result.CustomerID = rentEvent.CustomerID;
            result.RentalIdentifier = rentEvent.RentCarEventID;
            
            return result;

        }
    }
}
