using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Models
{
    [Index(nameof(RentCarEvent.CustomerID))]
    [Index(nameof(RentCarEvent.CarDetailsID))]
    public partial class RentCarEvent
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string RentCarEventID { get; set; }
        public System.DateTime SubmitDate { get; set; }
        public System.DateTime MaximumReturnDate { get; set; }
        public int CustomerID { get; set; }
        public int CarDetailsID { get; set; }
        public bool IsReturned { get; set; }

        public virtual CarDetails CarDetails { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual ReturnFile ReturnFile { get; set; }
    }
}
