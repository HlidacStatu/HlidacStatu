using System.Collections.Generic;

namespace SponzoriLoader
{
    class Donations
    {
        private Dictionary<Donor,List<Gift>> AllDonations { get; set; }

        public Donations(IEqualityComparer<Donor> equalityComparer)
        {
            AllDonations = new Dictionary<Donor, List<Gift>>(equalityComparer);
        }

        public void AddDonation(Donor donor, Gift gift)
        {
            if (!AllDonations.ContainsKey(donor))
            {
                AllDonations.Add(donor, new List<Gift>());
            }
            AllDonations[donor].Add(gift);
        }

        public Dictionary<Donor, List<Gift>> GetDonations()
        {
            return AllDonations;
        }

    }
}
