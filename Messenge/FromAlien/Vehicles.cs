using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.ApiClasses.NewFolder
{
    public class vehicle
    {
        public string id;
        public string brandName;
        public string modelName;
        public int year;
        public int enginePower;
        public string enginePowerType;
        public int capacity;
        public string description;
    }
    public class Vehicles
    {
        public int vehiclesCount;
        public DateTime generateDate;
        public List<vehicle> vehicles;

        
    }
}
