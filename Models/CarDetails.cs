using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rent_a_Car.Models
{

    [Index(nameof(CarDetails.CarID), IsUnique = true)]
    [Index(nameof(CarDetails.CompanyID))]
    [Index(nameof(CarDetails.CompanyID), nameof(CarDetails.CarID), IsUnique = true)]
    public class CarDetails
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CarDetails()
        {
            this.RentCars = new HashSet<RentCarEvent>();
        }
        public int CarDetailsID { get; set; }
        public int CarID { get; set; }
        public int CompanyID { get; set; }
        [Column(TypeName = "decimal(18,4)")]
        public decimal Price { get; set; }
        public System.DateTime YearOfProduction { get; set; }
        public string Description { get; set; }
        public bool IsAvailable { get; set; }

        public virtual Car Car { get; set; }
        public virtual Company Company { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RentCarEvent> RentCars { get; set; }
    }
}