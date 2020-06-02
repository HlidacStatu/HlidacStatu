using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SponzoriLoader
{
    class Donations
    {
        public Dictionary<Donor,List<Gift>> AllDonations { get; private set; }

        public Donations()
        {
            AllDonations = new Dictionary<Donor, List<Gift>>(new DonorEqualityComparer());
        }

        public void AddDonation(Donor donor, Gift gift)
        {
            if (!AllDonations.ContainsKey(donor))
            {
                AllDonations.Add(donor, new List<Gift>());
            }
            AllDonations[donor].Add(gift);
        }

    }
}
