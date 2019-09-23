using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.Serialization;


namespace HlidacStatu.Lib.Data.External.DataSets
{
    public partial class Serialization
    {

        public class PublicDatasetContractResolver : DefaultContractResolver
        {
            public static readonly PublicDatasetContractResolver Instance = new PublicDatasetContractResolver();

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
