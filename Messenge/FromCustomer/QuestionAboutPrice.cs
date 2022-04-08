using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Messenge.FromCustomer
{
    public class QuestionAboutPrice
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public System.DateTime DateofBecomingDriver { get; set; }
        public System.DateTime Birthday { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int StreetNumber { get; set; }
        public int carDetalisID { get; set; }
    }
}
