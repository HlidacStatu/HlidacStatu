using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.XSD
{

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/", IsNullable = false)]
    public partial class zaznam
    {



        /// <remarks/>
        public dumpZaznam data { get; set; }

        /// <remarks/>
        public zaznamPotvrzeni potvrzeni { get; set; }

    }

    

    
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/")]
    public partial class zaznamPotvrzeni
    {

        private zaznamPotvrzeniHash hashField;

        private string elektronickaZnackaField;

        /// <remarks/>
        public zaznamPotvrzeniHash hash
        {
            get
            {
                return this.hashField;
            }
            set
            {
                this.hashField = value;
            }
        }

        /// <remarks/>
        public string elektronickaZnacka
        {
            get
            {
                return this.elektronickaZnackaField;
            }
            set
            {
                this.elektronickaZnackaField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://portal.gov.cz/rejstriky/ISRS/1.2/")]
    public partial class zaznamPotvrzeniHash
    {

        private string algoritmusField;

        private string valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string algoritmus
        {
            get
            {
                return this.algoritmusField;
            }
            set
            {
                this.algoritmusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public string Value
        {
            get
            {
                return this.valueField;
            }
            set
            {
                this.valueField = value;
            }
        }
    }



}
