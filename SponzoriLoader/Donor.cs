using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponzoriLoader
{
    public class Donor : IEquatable<Donor>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TitleBefore { get; set; }
        public string TitleAfter { get; set; }
        public string City { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public bool Equals(Donor other)
        {
            return this.Name == other.Name
                && this.Surname == other.Surname
                && this.DateOfBirth == other.DateOfBirth;
        }
    }

    public class DonorEqualityComparer : IEqualityComparer<Donor>
    {
        public bool Equals(Donor x, Donor y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Donor obj)
        {
            return (obj.Name, obj.Surname, obj.DateOfBirth).GetHashCode();
        }
    }
}
