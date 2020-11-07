using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External.eAgri.DeMinimis
{

    // NOTE: Generated code may require at least .NET Framework 4.5 or .NET Core/Standard 2.0.
    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace = "http://www.pds.eu/RDM_PUB01B", IsNullable = false)]
    public partial class Response
    {

        private byte pocetField;

        private ResponseSeznam_subjektu seznam_subjektuField;

        /// <remarks/>
        public byte pocet
        {
            get
            {
                return this.pocetField;
            }
            set
            {
                this.pocetField = value;
            }
        }

        /// <remarks/>
        public ResponseSeznam_subjektu seznam_subjektu
        {
            get
            {
                return this.seznam_subjektuField;
            }
            set
            {
                this.seznam_subjektuField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektu
    {

        private ResponseSeznam_subjektuSubjekt subjektField;

        /// <remarks/>
        public ResponseSeznam_subjektuSubjekt subjekt
        {
            get
            {
                return this.subjektField;
            }
            set
            {
                this.subjektField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjekt
    {

        private uint subjektidField;

        private string obchodni_jmenoField;

        private ResponseSeznam_subjektuSubjektSeznam_identifikatoru seznam_identifikatoruField;

        private ResponseSeznam_subjektuSubjektAdresa adresaField;

        private ResponseSeznam_subjektuSubjektOblast_stav[] limity_stavField;

        private ResponseSeznam_subjektuSubjektNarizeni_stav[] limity_stav_narizeniField;

        private ResponseSeznam_subjektuSubjektPodpora[] seznam_podporField;

        private System.DateTime datum_zahajeni_cinnostiField;

        private ResponseSeznam_subjektuSubjektUcetni_obdobi[] seznam_uoField;

        /// <remarks/>
        public uint subjektid
        {
            get
            {
                return this.subjektidField;
            }
            set
            {
                this.subjektidField = value;
            }
        }

        /// <remarks/>
        public string obchodni_jmeno
        {
            get
            {
                return this.obchodni_jmenoField;
            }
            set
            {
                this.obchodni_jmenoField = value;
            }
        }

        /// <remarks/>
        public ResponseSeznam_subjektuSubjektSeznam_identifikatoru seznam_identifikatoru
        {
            get
            {
                return this.seznam_identifikatoruField;
            }
            set
            {
                this.seznam_identifikatoruField = value;
            }
        }

        /// <remarks/>
        public ResponseSeznam_subjektuSubjektAdresa adresa
        {
            get
            {
                return this.adresaField;
            }
            set
            {
                this.adresaField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("oblast_stav", IsNullable = false)]
        public ResponseSeznam_subjektuSubjektOblast_stav[] limity_stav
        {
            get
            {
                return this.limity_stavField;
            }
            set
            {
                this.limity_stavField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("narizeni_stav", IsNullable = false)]
        public ResponseSeznam_subjektuSubjektNarizeni_stav[] limity_stav_narizeni
        {
            get
            {
                return this.limity_stav_narizeniField;
            }
            set
            {
                this.limity_stav_narizeniField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("podpora", IsNullable = false)]
        public ResponseSeznam_subjektuSubjektPodpora[] seznam_podpor
        {
            get
            {
                return this.seznam_podporField;
            }
            set
            {
                this.seznam_podporField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime datum_zahajeni_cinnosti
        {
            get
            {
                return this.datum_zahajeni_cinnostiField;
            }
            set
            {
                this.datum_zahajeni_cinnostiField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ucetni_obdobi", IsNullable = false)]
        public ResponseSeznam_subjektuSubjektUcetni_obdobi[] seznam_uo
        {
            get
            {
                return this.seznam_uoField;
            }
            set
            {
                this.seznam_uoField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektSeznam_identifikatoru
    {

        private ResponseSeznam_subjektuSubjektSeznam_identifikatoruIdentifikator identifikatorField;

        /// <remarks/>
        public ResponseSeznam_subjektuSubjektSeznam_identifikatoruIdentifikator identifikator
        {
            get
            {
                return this.identifikatorField;
            }
            set
            {
                this.identifikatorField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektSeznam_identifikatoruIdentifikator
    {

        private System.DateTime platnost_odField;

        private string typField;

        private uint valueField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType = "date")]
        public System.DateTime platnost_od
        {
            get
            {
                return this.platnost_odField;
            }
            set
            {
                this.platnost_odField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string typ
        {
            get
            {
                return this.typField;
            }
            set
            {
                this.typField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute()]
        public uint Value
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

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektAdresa
    {

        private string ulicenazField;

        private ushort cisdomhodField;

        private byte cisorhodField;

        private string cobcenazField;

        private string mcastnazField;

        private string obecnazField;

        private ushort pscField;

        private uint kodField;

        /// <remarks/>
        public string ulicenaz
        {
            get
            {
                return this.ulicenazField;
            }
            set
            {
                this.ulicenazField = value;
            }
        }

        /// <remarks/>
        public ushort cisdomhod
        {
            get
            {
                return this.cisdomhodField;
            }
            set
            {
                this.cisdomhodField = value;
            }
        }

        /// <remarks/>
        public byte cisorhod
        {
            get
            {
                return this.cisorhodField;
            }
            set
            {
                this.cisorhodField = value;
            }
        }

        /// <remarks/>
        public string cobcenaz
        {
            get
            {
                return this.cobcenazField;
            }
            set
            {
                this.cobcenazField = value;
            }
        }

        /// <remarks/>
        public string mcastnaz
        {
            get
            {
                return this.mcastnazField;
            }
            set
            {
                this.mcastnazField = value;
            }
        }

        /// <remarks/>
        public string obecnaz
        {
            get
            {
                return this.obecnazField;
            }
            set
            {
                this.obecnazField = value;
            }
        }

        /// <remarks/>
        public ushort psc
        {
            get
            {
                return this.pscField;
            }
            set
            {
                this.pscField = value;
            }
        }

        /// <remarks/>
        public uint kod
        {
            get
            {
                return this.kodField;
            }
            set
            {
                this.kodField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektOblast_stav
    {

        private System.DateTime platnostField;

        private string oblast_kodField;

        private uint limitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime platnost
        {
            get
            {
                return this.platnostField;
            }
            set
            {
                this.platnostField = value;
            }
        }

        /// <remarks/>
        public string oblast_kod
        {
            get
            {
                return this.oblast_kodField;
            }
            set
            {
                this.oblast_kodField = value;
            }
        }

        /// <remarks/>
        public uint limit
        {
            get
            {
                return this.limitField;
            }
            set
            {
                this.limitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektNarizeni_stav
    {

        private System.DateTime platnostField;

        private string kodField;

        private uint limitField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime platnost
        {
            get
            {
                return this.platnostField;
            }
            set
            {
                this.platnostField = value;
            }
        }

        /// <remarks/>
        public string kod
        {
            get
            {
                return this.kodField;
            }
            set
            {
                this.kodField = value;
            }
        }

        /// <remarks/>
        public uint limit
        {
            get
            {
                return this.limitField;
            }
            set
            {
                this.limitField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektPodpora
    {

        private string oblastField;

        private System.DateTime datum_prideleniField;

        private string menaField;

        private uint castka_kcField;

        private decimal castka_euroField;

        private byte forma_podporyField;

        private string ucel_podporyField;

        private byte pravni_akt_poskytnutiField;

        private ResponseSeznam_subjektuSubjektPodporaRezim_podpory rezim_podporyField;

        private uint id_podporyField;

        private uint poskytovatel_idField;

        private string poskytovatel_ojmField;

        private uint poskytovatel_icField;

        private byte stavField;

        private ResponseSeznam_subjektuSubjektPodporaSeznam_priznaky seznam_priznakyField;

        private System.DateTime insdatField;

        private System.DateTime edidatField;

        /// <remarks/>
        public string oblast
        {
            get
            {
                return this.oblastField;
            }
            set
            {
                this.oblastField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime datum_prideleni
        {
            get
            {
                return this.datum_prideleniField;
            }
            set
            {
                this.datum_prideleniField = value;
            }
        }

        /// <remarks/>
        public string mena
        {
            get
            {
                return this.menaField;
            }
            set
            {
                this.menaField = value;
            }
        }

        /// <remarks/>
        public uint castka_kc
        {
            get
            {
                return this.castka_kcField;
            }
            set
            {
                this.castka_kcField = value;
            }
        }

        /// <remarks/>
        public decimal castka_euro
        {
            get
            {
                return this.castka_euroField;
            }
            set
            {
                this.castka_euroField = value;
            }
        }

        /// <remarks/>
        public byte forma_podpory
        {
            get
            {
                return this.forma_podporyField;
            }
            set
            {
                this.forma_podporyField = value;
            }
        }

        /// <remarks/>
        public string ucel_podpory
        {
            get
            {
                return this.ucel_podporyField;
            }
            set
            {
                this.ucel_podporyField = value;
            }
        }

        /// <remarks/>
        public byte pravni_akt_poskytnuti
        {
            get
            {
                return this.pravni_akt_poskytnutiField;
            }
            set
            {
                this.pravni_akt_poskytnutiField = value;
            }
        }

        /// <remarks/>
        public ResponseSeznam_subjektuSubjektPodporaRezim_podpory rezim_podpory
        {
            get
            {
                return this.rezim_podporyField;
            }
            set
            {
                this.rezim_podporyField = value;
            }
        }

        /// <remarks/>
        public uint id_podpory
        {
            get
            {
                return this.id_podporyField;
            }
            set
            {
                this.id_podporyField = value;
            }
        }

        /// <remarks/>
        public uint poskytovatel_id
        {
            get
            {
                return this.poskytovatel_idField;
            }
            set
            {
                this.poskytovatel_idField = value;
            }
        }

        /// <remarks/>
        public string poskytovatel_ojm
        {
            get
            {
                return this.poskytovatel_ojmField;
            }
            set
            {
                this.poskytovatel_ojmField = value;
            }
        }

        /// <remarks/>
        public uint poskytovatel_ic
        {
            get
            {
                return this.poskytovatel_icField;
            }
            set
            {
                this.poskytovatel_icField = value;
            }
        }

        /// <remarks/>
        public byte stav
        {
            get
            {
                return this.stavField;
            }
            set
            {
                this.stavField = value;
            }
        }

        /// <remarks/>
        public ResponseSeznam_subjektuSubjektPodporaSeznam_priznaky seznam_priznaky
        {
            get
            {
                return this.seznam_priznakyField;
            }
            set
            {
                this.seznam_priznakyField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime insdat
        {
            get
            {
                return this.insdatField;
            }
            set
            {
                this.insdatField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime edidat
        {
            get
            {
                return this.edidatField;
            }
            set
            {
                this.edidatField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektPodporaRezim_podpory
    {

        private bool adhocField;

        /// <remarks/>
        public bool adhoc
        {
            get
            {
                return this.adhocField;
            }
            set
            {
                this.adhocField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektPodporaSeznam_priznaky
    {

        private ResponseSeznam_subjektuSubjektPodporaSeznam_priznakyPriznak priznakField;

        /// <remarks/>
        public ResponseSeznam_subjektuSubjektPodporaSeznam_priznakyPriznak priznak
        {
            get
            {
                return this.priznakField;
            }
            set
            {
                this.priznakField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektPodporaSeznam_priznakyPriznak
    {

        private byte kodField;

        private string nazevField;

        private string popisField;

        /// <remarks/>
        public byte kod
        {
            get
            {
                return this.kodField;
            }
            set
            {
                this.kodField = value;
            }
        }

        /// <remarks/>
        public string nazev
        {
            get
            {
                return this.nazevField;
            }
            set
            {
                this.nazevField = value;
            }
        }

        /// <remarks/>
        public string popis
        {
            get
            {
                return this.popisField;
            }
            set
            {
                this.popisField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, Namespace = "http://www.pds.eu/RDM_PUB01B")]
    public partial class ResponseSeznam_subjektuSubjektUcetni_obdobi
    {

        private System.DateTime datum_doField;

        private System.DateTime datum_odField;

        private uint id_uoField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime datum_do
        {
            get
            {
                return this.datum_doField;
            }
            set
            {
                this.datum_doField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime datum_od
        {
            get
            {
                return this.datum_odField;
            }
            set
            {
                this.datum_odField = value;
            }
        }

        /// <remarks/>
        public uint id_uo
        {
            get
            {
                return this.id_uoField;
            }
            set
            {
                this.id_uoField = value;
            }
        }
    }


}
