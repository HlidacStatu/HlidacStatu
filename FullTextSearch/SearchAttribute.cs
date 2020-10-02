using System;

namespace FullTextSearch
{
    public class SearchAttribute: Attribute
    {
        // Not implemented yet
        public double Weight { get; set; }

        // Not implemented yet
        // Boost score only if whole word is found (ICO)
        public bool WholeWord { get; set; }
    }
}
