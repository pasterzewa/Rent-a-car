using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Models
{
    [Index(nameof(Car.Brand))]
    [Index(nameof(Car.Model))]
    public partial class Car
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Car()
        {
            this.CarDetails = new HashSet<CarDetails>();
        }

        public int CarID { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int HorsePower { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CarDetails> CarDetails { get; set; }
    }
}
