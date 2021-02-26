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
        public Summary GetSummary()
        {
            var sum = new Summary();
            sum.Oddeleni = this.oznaceni.ToLower().Contains("oddělení") ? 1 : 0;
            sum.Odbory = this.oznaceni.ToLower().Contains("odbor") ? 1 : 0;
            sum.Sekce = this.oznaceni.ToLower().Contains("sekce") ? 1 : 0;
            sum.Jine = sum.Oddeleni + sum.Odbory + sum.Sekce == 0 ? 1 : 0;
            sum.PracovniPozice = this.mistoPracovniPocet;
            sum.SluzebniMista = this.mistoSluzebniPocet;


            if (!(PodrizeneOrganizace is null) && PodrizeneOrganizace.Length != 0)
            {
                foreach (var po in PodrizeneOrganizace)
                {
                    sum.Add(po.GetSummary());
                }
            }

            return sum;
        }

    }

    public class Summary
    {
        internal Summary() { }
        public Summary(IEnumerable<JednotkaOrganizacni> urady)
        : this()
        {
            if (urady == null)
                throw new System.ArgumentNullException();
            this.Urady = urady.Count();

            foreach (var os in urady)
            {
                this.Add(os.GetSummary());
            }

        }
        public Summary Add(Summary sum)
        {
            this.Jine += sum.Jine;
            this.Odbory += sum.Odbory;
            this.Oddeleni += sum.Oddeleni;
            this.Urady += sum.Urady;
            this.PracovniPozice += sum.PracovniPozice;
            this.Sekce += sum.Sekce;
            this.SluzebniMista += sum.SluzebniMista;
            return this;
        }
        public int Jine { get; set; }
        public int Sekce { get; set; }
        public int Urady { get; set; } 
        public int Odbory { get; set; }
        public int Oddeleni { get; set; }
        public int OrganizacniJednotky { get { return Jine + Sekce + Odbory + Oddeleni; } }
        public int PracovniPozice { get; set; }
        public int SluzebniMista { get; set; }

        public string Description(string ico)
        {
            var ret = $"Organizace je tvořena "
                + (this.Urady == 0 ? "" : $" {Devmasters.Lang.Plural.Get(this.Urady, "jedním úřadem", "{0} úřady", "{0} úřady")}, dále ")
                + $"{Devmasters.Lang.Plural.Get(this.OrganizacniJednotky, "jednou organizační části", "{0} organizačními částmi", "{0} organizačními částmi")}, "
                + $"{Devmasters.Lang.Plural.GetWithZero(this.SluzebniMista, "nezaměstnává žádné úředníky na služebních místech", "zaměstnává jednoho úředníka na služebních místech", "zaměstnává {0} úředníky na služebních místech", "zaměstnává {0} úředníků na služebních místech")} "
                + $"a {Devmasters.Lang.Plural.GetWithZero(this.PracovniPozice, "žádné další zaměstnance", "jednoho zaměstnance", "{0} další zaměstnance", "{0} dalších zaměstnanců")}."
                ;
                return ret;


        }
        public string HtmlDescription(string ico)
        {
            var ret = $"Organizace je tvořena<ul>"
                + (this.Urady == 0 ? "" : $"<li><a href='/subjekt/DalsiInformace/{ico}'>{Devmasters.Lang.Plural.Get(this.Urady, "jedním úřadem", "{0} úřady", "{0} úřady")}</a></li>")
                + $"<li><a href='/subjekt/OrganizacniStruktura/{ico}'>{Devmasters.Lang.Plural.Get(this.OrganizacniJednotky, "<b>jednou</b> organizační části", "<b>{0}</b> organizačními částmi", "<b>{0}</b> organizačními částmi")}</a></li>"
                + $"<li>{Devmasters.Lang.Plural.GetWithZero(this.SluzebniMista, "nezaměstnává žádné úředníky na služebních místech", "zaměstnává <b>jednoho úředníka</b> na služebních místech", "zaměstnává <b>{0} úředníky</b> na služebních místech", "zaměstnává <b>{0} úředníků</b> na služebních místech")}</li>"
                + $"<li>{Devmasters.Lang.Plural.GetWithZero(this.PracovniPozice, "žádné další zaměstnance", "<b>jednoho</b> zaměstnance", "<b>{0}</b> další zaměstnance", "<b>{0}</b> dalších zaměstnanců")}</li>"
                ;
                return ret;


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