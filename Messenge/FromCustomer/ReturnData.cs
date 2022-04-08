using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Messenge.FromCustomer
{
    public class ReturnData
    {
        public int carDetailsID { get; set; }
        public string email { get; set; }
        public System.DateTime expectedReturnDate { get; set; }
    }
}
