using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HlidacStatu.Web.Framework
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SwaggerControllerTagAttribute : Attribute
    {
        public string ControllerName { get; private set; }

        public SwaggerControllerTagAttribute(string ctrlrName)
        {
            if (string.IsNullOrEmpty(ctrlrName))
            {
                throw new ArgumentNullException("ctrlrName");
            }
            ControllerName = ctrlrName;
        }
    }
}