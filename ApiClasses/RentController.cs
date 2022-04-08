using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rent_a_Car.Data;
using Rent_a_Car.Models;
using Rent_a_Car.ApiClasses;
namespace Rent_a_Car.ApiClasses
{
    public class RentController
    {
        public static RentApiData RentACar(ApplicationDbContext context, int carDetailsID, int customerID, DateTime expectedTime)
        {
            RentCarEvent rentEvent = new();
            rentEvent.CustomerID = customerID;
            rentEvent.CarDetailsID = carDetailsID;
            rentEvent.RentCarEventID = Guid.NewGuid().ToString();
            rentEvent.MaximumReturnDate = expectedTime;
            rentEvent.SubmitDate = DateTime.Now;
            RentApiData result = RentApiData.FromRentEvent(rentEvent);
            //try
            //{
                context.RentCar.Add(rentEvent);
                context.CarDetails.SingleOrDefault(c => c.CarDetailsID == carDetailsID).IsAvailable = false;
                context.SaveChanges();
            //}
            //catch
            //{
            //    return null;
            //}
            return result;
        }
    }
}
