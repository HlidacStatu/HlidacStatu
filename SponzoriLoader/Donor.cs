using System;
using System.Collections.Generic;

namespace SponzoriLoader
{
    public class Donor
    {
        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string TitleBefore { get; set; }
        public string TitleAfter { get; set; }
        public string City { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }

    public class DonorEqualityComparer : IEqualityComparer<Donor>
    {
        public bool Equals(Donor x, Donor y)
        {
            return x.CompanyId == y.CompanyId
                && x.Name == y.Name
                && x.Surname == y.Surname
                && x.DateOfBirth == y.DateOfBirth;
        }

        public int GetHashCode(Donor obj)
        {
            return (obj.CompanyId, obj.Name, obj.Surname, obj.DateOfBirth).GetHashCode();
        }
    }

}
