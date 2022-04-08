using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent_a_Car.Models
{
    [Index(nameof(ReturnFile.EmployerID))]
    public partial class ReturnFile
    {
        [Key]
        public string ReturnFileID { get; set; }
        public int RentedCarID { get; set; }
        public System.DateTime ReturnDate { get; set; }
        public string CarConditon { get; set; }
        public int OdometerReading { get; set; }
        public byte[] Photo { get; set; }
        public string PhotoBlobName { get; set; }
        public byte[] ReturnProocol { get; set; }
        public string ReturnProocolBlobName { get; set; }
        public Nullable<int> EmployerID { get; set; }

        public virtual Employer Employer { get; set; }
        [ForeignKey("ReturnFileID")]
        public virtual RentCarEvent RentCarEvent { get; set; }
    }
}
