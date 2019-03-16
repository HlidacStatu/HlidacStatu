using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Enhancers
{
    public class Enhancement : IEquatable<Enhancement>
    {

        public Enhancement() { }

        public Enhancement(string title, string description, string changedParameter,
            string changedOldValue, string changedNewValue, IEnhancer enhancer
            )
        {
            this.Title = title;
            this.Description = description;
            this.Changed = new Enhancers.Enhancement.Change()
            {
                ParameterName = changedParameter,
                PreviousValue = changedOldValue,
                NewValue = changedNewValue
            };
            this.EnhancerType = enhancer.GetType().FullName;

        }

        public class Change
        {
            [Nest.Keyword]
            public string ParameterName { get; set; }

            [Nest.Keyword]
            public string PreviousValue { get; set; }
            [Nest.Keyword]
            public string NewValue { get; set; }

        }
        [Nest.Date]
        public DateTime Created { get; set; } = DateTime.Now;

        [Nest.Keyword]
        public string Title { get; set; }

        [Nest.Keyword]
        public string Description { get; set; }

        public Change Changed { get; set; } = new Change();

        public bool Public { get; set; } = true;


        [Nest.Keyword]
        public string EnhancerType { get; set; }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Enhancement);
        }
        public bool Equals(Enhancement other)
        {
            if (other == null)
                return false;
            if (this.EnhancerType != other.EnhancerType)
                return false;
            if (this.Title != other.Title)
                return false;
            if (this.Changed != null && other.Changed != null)
            {
                if (this.Changed.ParameterName != other.Changed.ParameterName)
                    return false;
            }
            if (
                (this.Changed != null && other.Changed == null)
                ||
                (this.Changed == null && other.Changed != null)
                )
            {
                return false;
            }

            return true;
        }
        public static bool operator ==(Enhancement enh1, Enhancement enh2)
        {
            if (((object)enh1) == null || ((object)enh2) == null)
                return Object.Equals(enh1, enh2);

            return enh1.Equals(enh2);
        }

        public static bool operator !=(Enhancement enh1, Enhancement enh2)
        {
            if (((object)enh1) == null || ((object)enh2) == null)
                return !Object.Equals(enh1, enh2);

            return !(enh1.Equals(enh2));
        }
    }
}
