using System.Collections.Generic;
using System.Linq;

namespace HlidacStatu.Lib.Data.OrgStrukturyStatu
{

    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class JednotkaOrganizacni
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("JednotkaOrganizacni")]
        public JednotkaOrganizacni[] PodrizeneOrganizace { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string idExterni { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int mistoSluzebniPocet { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public int mistoPracovniPocet { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string oznaceni { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string predstaveny { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zkratka { get; set; }


        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string idNadrizene { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string idNadrizeneExterni { get; set; }

        public int CelkemZamestnava()
        {
            int soucetPodrizenych = ZamestnavajiPodrizeneOrganizace();
            return soucetPodrizenych + mistoPracovniPocet + mistoSluzebniPocet;
        }

        public int ZamestnavajiPodrizeneOrganizace()
        {
            return PodrizeneOrganizace?.Sum(p => p.CelkemZamestnava()) ?? 0;
        }


        public int RidiSluzebnich()
        {
            return PodrizeneOrganizace?.Sum(p => p.mistoSluzebniPocet + p.RidiSluzebnich()) ?? 0;
        }

        public int RidiPracovnich()
        {
            return PodrizeneOrganizace?.Sum(p => p.mistoPracovniPocet + p.RidiPracovnich()) ?? 0;
        }

        public D3GraphHierarchy GenerateD3DataHierarchy()
        {
            List<D3GraphHierarchy> children = new List<D3GraphHierarchy>();
            if (!(PodrizeneOrganizace is null) && PodrizeneOrganizace.Length != 0)
            {
                foreach (var po in PodrizeneOrganizace)
                {
                    children.Add(po.GenerateD3DataHierarchy());
                }

            }

            var current = new D3GraphHierarchy()
            {
                name = this.oznaceni,
                size = this.CelkemZamestnava(),
                children = children,
                employs = $"pracovní: {this.mistoPracovniPocet}; služební: {this.mistoSluzebniPocet}",
                manages = $"podřízených p: {RidiPracovnich()}; s: {RidiSluzebnich()}"

            };

            return current;
        }

    }

    public class D3GraphHierarchy
    {
        public string name { get; set; }
        public string employs { get; set; }
        public string manages { get; set; }
        public List<D3GraphHierarchy> children { get; set; }
        public int size { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "", IsNullable = false)]
    public partial class organizacni_struktura_sluzebnich_uradu
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("ExportInfo", typeof(ExportInfo), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public ExportInfo ExportInfo { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("UradSluzebniSeznam", typeof(UradSluzebniSeznam), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UradSluzebniSeznam UradSluzebniSeznam { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("UradSluzebniStrukturaOrganizacni", typeof(UradSluzebniStrukturaOrganizacni), Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UradSluzebniStrukturaOrganizacni[] OrganizacniStruktura { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class ExportInfo
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ExportId { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ZdrojDat { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ExportTyp { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ExportPokyn { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public string ExportDatumCas { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UradSluzebniSeznam
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("UradSluzebni", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public UradSluzebni[] SluzebniUrady { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UradSluzebni
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string idDS { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string oznaceni { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string zkratka { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string idNadrizene { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class UradSluzebniStrukturaOrganizacni
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StrukturaOrganizacni", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public StrukturaOrganizacni StrukturaOrganizacni { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("StrukturaOrganizacniPlocha", Form = System.Xml.Schema.XmlSchemaForm.Unqualified)]
        public StrukturaOrganizacniPlocha StrukturaOrganizacniPlocha { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string id { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StrukturaOrganizacni
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("JednotkaOrganizacni")]
        public JednotkaOrganizacni HlavniOrganizacniJednotka { get; set; }
    }

    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.8.3928.0")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class StrukturaOrganizacniPlocha
    {
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("JednotkaOrganizacni")]
        public JednotkaOrganizacni[] JednotkaOrganizacni { get; set; }
    }
}