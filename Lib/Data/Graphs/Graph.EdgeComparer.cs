﻿using System;
using System.Collections.Generic;


namespace HlidacStatu.Lib.Data
{
    public partial class Graph
    {
        public class EdgeComparer : IEqualityComparer<Edge>
        {
            public bool Equals(Edge x, Edge y)
            {
                //Check whether the compared objects reference the same data.
                if (Object.ReferenceEquals(x, y)) return true;

                //Check whether any of the compared objects is null.
                if (Object.ReferenceEquals(x, null) || Object.ReferenceEquals(y, null))
                    return false;

                return x.GetHashCode() == y.GetHashCode();
            }

            public int GetHashCode(Edge obj)
            {
                //Check whether the object is null
                if (Object.ReferenceEquals(obj, null)) return 0;

                //Get hash code for the Name field if it is not null.
                return obj.GetHashCode();

            }
        }

    }

}
