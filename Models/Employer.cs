using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rent_a_Car.Models
{
    public partial class Employer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Employer()
        {
            this.ReturnFiles = new HashSet<ReturnFile>();
        }

        public int EmployerID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReturnFile> ReturnFiles { get; set; }
    }
}
