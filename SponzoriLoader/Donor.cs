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

    public class PersonDonorEqualityComparer : IEqualityComparer<Donor>
    {
        public bool Equals(Donor x, Donor y)
        {
            return x.Name == y.Name
                && x.Surname == y.Surname
                && x.DateOfBirth == y.DateOfBirth;
        }

        public int GetHashCode(Donor obj)
        {
            return (obj.Name, obj.Surname, obj.DateOfBirth).GetHashCode();
        }
    }

    public class CompanyDonorEqualityComparer : IEqualityComparer<Donor>
    {
        public bool Equals(Donor x, Donor y)
        {
            return x.CompanyId == y.CompanyId;
        }

        public int GetHashCode(Donor obj)
        {
            return obj.CompanyId.GetHashCode();
        }
    }
}
