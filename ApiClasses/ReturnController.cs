using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Rent_a_Car.Data;
using Rent_a_Car.Models;
using Rent_a_Car.ApiClasses;

namespace Rent_a_Car.ApiClasses
{
    public class ReturnController
    {
        public static bool ReturnACar(ApplicationDbContext context, string rentToken)
        {
            var rentedCars = context.RentCar.Select(c => c).Where(c => c.RentCarEventID == rentToken);

            if (rentedCars.Count() == 0) return false;

            var car = rentedCars.First();

            if (car.IsReturned == true)
            {
                return false;
            }

            try
            {
                car.IsReturned = true;
                int carDetailsID = car.CarDetailsID;
                context.CarDetails.FirstOrDefault(c => c.CarDetailsID == carDetailsID).IsAvailable = true;
                context.SaveChanges();
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
