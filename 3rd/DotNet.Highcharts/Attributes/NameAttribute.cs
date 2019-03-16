using System;

namespace DotNet.Highcharts.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class NameAttribute : Attribute
    {
        public NameAttribute(string name) { Name = name; }
        public string Name { get; private set; }
    }
}