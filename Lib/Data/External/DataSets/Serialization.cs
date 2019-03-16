using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.DataSets
{
    public class Serialization
    {
        public class PublicDatasetContractResolver : DefaultContractResolver
        {
            public new static readonly PublicDatasetContractResolver Instance = new PublicDatasetContractResolver();

            protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
            {
                JsonProperty property = base.CreateProperty(member, memberSerialization);

                if (property.DeclaringType == typeof(Registration) && property.PropertyName == "hidden")
                {
                    property.ShouldSerialize = instance => false;
                }

                return property;
            }
        }
    }
}
