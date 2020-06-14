using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Devmasters.Core;


namespace HlidacStatu.Lib.Data.External
{
    public class Merk
    {
        static string token = Devmasters.Core.Util.Config.GetConfigValue("MerkAPIKey");
        static string rootUrl = "https://api.merk.cz:443/";

        static string searchICOUrl = rootUrl + "company/?regno={0}&country_code=cz";
        static string searchNameUrl = rootUrl + "suggest/?name={0}&country_code=cz";



        public static string FirmaUrl(string ico)
        {
            return string.Format("https://www.detail.cz/firma/cz-{0}", HlidacStatu.Util.ParseTools.IcoToMerkIco(ico));
        }
        public static Firma FromIco(string ico)
        {
            var res = Search(searchICOUrl, ico);
            if (res.Count() == 0)
                return Firma.NotFound;
            else
                return res.First().ToFirma();
        }

        public static CoreCompanyStructure FromIcoFull(string ico)
        {
            var res = Search(searchICOUrl, ico);
            if (res.Count() == 0)
                return null;
            else
                return res.First();
        }

        public static Firma FromName(string name)
        {
            var res = Search(searchNameUrl, name);
            if (res.Count() == 0)
            {
                res = Search(searchNameUrl, Firma.JmenoBezKoncovky(name));
            }
            string n1 = Devmasters.Core.TextUtil.RemoveDiacritics(Firma.JmenoBezKoncovky(name)).ToLowerInvariant();
            var r1 = res
                .Where(r => Devmasters.Core.TextUtil.RemoveDiacritics(Firma.JmenoBezKoncovky(r.name)).ToLowerInvariant() == n1)
                .FirstOrDefault();

            if (r1 == null)
                return Firma.NotFound;
            else
                return r1.ToFirma();
        }



        private static CoreCompanyStructure[] Search(string url, string query)
        {
            try
            {
                var req = Request(string.Format(url, System.Net.WebUtility.UrlDecode(query)));
                CoreCompanyStructure[] res = Newtonsoft.Json.JsonConvert.DeserializeObject<CoreCompanyStructure[]>(req);
                if (res == null)
                    res = new CoreCompanyStructure[] { };
                return res;
            }
            catch (Exception e)
            {
                HlidacStatu.Util.Consts.Logger.Error("Merk deserialization error " + url + " ? " + query, e);
                return new CoreCompanyStructure[] { };
            }


        }



        static string emptyJson = "[]";
        private static string Request(string url)
        {


            using (Devmasters.Net.Web.URLContent http = new Devmasters.Net.Web.URLContent(url))
            {
                http.Timeout = 30000;
                http.RequestParams.Headers.Add("Authorization", "Token " + token);

                try
                {
                    var a = http.GetContent(System.Text.Encoding.UTF8).Text;
                    return a;

                }
                catch (Devmasters.Net.Web.UrlContentException e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Merk request " + url, e);
                    //if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                    //    return string.Empty;
                    //else
                    if (e.InnerException != null && e.InnerException.Message.Contains("204"))
                        return emptyJson;
                    return emptyJson;
                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Error("Merk request " + url, e);
                    //Console.Write(e.ToString());
                    return emptyJson;
                }
            }

        }




        public class CoreCompanyStructure
        {
            public Firma ToFirma()
            {
                var f = new Firma();
                f.ICO = HlidacStatu.Util.ParseTools.MerkIcoToICO(this.regno.ToString());
                f.DIC = this.vatno;
                f.DatovaSchranka = this.databox_ids?.ToArray() ?? new string[] { };
                f.Datum_Zapisu_OR = this.estab_date;
                f.Jmeno = this.name;
                f.Kod_PF = this.legal_form.id;

                f.NACE = this.secondary_industries != null ?
                            new string[] { this.industry.id }.Concat(this.secondary_industries.Select(m => m.id)).ToArray()
                            : new string[] { this.industry.id };

                //f.Popis = 
                f.Source = this.link;
                f.Stav_subjektu = is_active ? 1 : 0;
                //f.VersionUpdate;
                return f;
            }


            public Status status { get; set; }
            public OwningType owning_type { get; set; }
            public IList<BankAccount> bank_accounts { get; set; }
            public IList<Phone> phones { get; set; }
            public IList<object> twitter { get; set; }
            public object vzp_debt { get; set; }
            public IList<object> linkedin { get; set; }
            public int? active_licenses_count { get; set; }
            public bool is_unreliable_vatpayer { get; set; }
            public bool is_vatpayer { get; set; }
            public int? regno { get; set; }
            public IList<SecondaryIndustry> secondary_industries { get; set; }
            public object salutation { get; set; }
            public DateTime? updated_src { get; set; }
            public int? years_in_business { get; set; }
            public IList<TurnoverHistory> turnover_history { get; set; }
            public object taxno { get; set; }
            public Court court { get; set; }
            public Profit profit { get; set; }
            public DateTime? updated_from_src { get; set; }
            public object vat_registration_type { get; set; }
            public BankAccountsBalance bank_accounts_balance { get; set; }
            public Ebidta ebidta { get; set; }
            public object esi { get; set; }
            public Gps gps { get; set; }
            public Body body { get; set; }
            public DateTime? updated { get; set; }
            public IList<Contract> contracts { get; set; }
            public bool is_active { get; set; }
            public object government_grants { get; set; }
            public LegalForm legal_form { get; set; }
            public string link { get; set; }
            public int? business_premises_count { get; set; }
            public object trademarks { get; set; }

            public Address address { get; set; }

            public IList<Email> emails { get; set; }
            public IList<MagnitudeHistory> magnitude_history { get; set; }
            public object unreliable_vatpayer_since { get; set; }
            public string vatno { get; set; }
            public string name { get; set; }
            public IList<Web> webs { get; set; }
            public Industry industry { get; set; }
            public IList<string> databox_ids { get; set; }
            public Magnitude magnitude { get; set; }
            public IList<Mobile> mobiles { get; set; }
            public object insolvency_cases { get; set; }
            public DateTime? vat_registration_date { get; set; }
            public IList<object> facebook { get; set; }
            public DateTime? estab_date { get; set; }
            public Turnover turnover { get; set; }
            public class Status
            {
                public string text { get; set; }
                public int? id { get; set; }
            }

            public class OwningType
            {
                public string text { get; set; }
                public int? id { get; set; }
            }

            public class BankAccount
            {
                public string account_number { get; set; }
                public string bank_code { get; set; }
            }

            public class Phone
            {
                public DateTime? acc { get; set; }
                public string score { get; set; }
                public string number { get; set; }
            }

            public class SecondaryIndustry
            {
                public string text { get; set; }
                public string id { get; set; }
            }

            public class TurnoverHistory
            {
                public int? turnover_id { get; set; }
                public int? year { get; set; }
            }

            public class Court
            {
                public string file_nr { get; set; }
                public string name { get; set; }
            }

            public class Profit
            {
                public object amount { get; set; }
                public int? year { get; set; }
            }

            public class BankAccountsBalance
            {
                public object amount { get; set; }
                public int? year { get; set; }
            }

            public class Ebidta
            {
                public object amount { get; set; }
                public int? year { get; set; }
            }

            public class Gps
            {
                public double? lat { get; set; }
                public double? lon { get; set; }
            }

            public class Address
            {
                public string number_descriptive { get; set; }
                public string municipality_part { get; set; }
                public string text { get; set; }
                public string municipality { get; set; }
                public string number { get; set; }
                public string municipality_first { get; set; }
                public string street_fixed { get; set; }
                public string street { get; set; }
                public int? postal_code { get; set; }
                public string number_orientation { get; set; }
                public string country_code { get; set; }
            }

            public class Person
            {
                public string first_name { get; set; }
                public string last_name { get; set; }
                public string company_role { get; set; }
                public string gender { get; set; }
                public int? age { get; set; }
                public string degree_after { get; set; }
                public string link { get; set; }
                public string country_code { get; set; }
                public Address address { get; set; }
                public string salutation { get; set; }
                public string degree_before { get; set; }
                public int? company_role_id { get; set; }
            }

            public class Body
            {
                public IList<Person> persons { get; set; }
                public int? average_age { get; set; }
            }

            public class Contract
            {
                public string url { get; set; }
                public string payer_name { get; set; }
                public DateTime? contract_date { get; set; }
                public int? payer_regno { get; set; }
                public DateTime? published_date { get; set; }
                public double price_with_vat { get; set; }
                public bool has_multiple_contractors { get; set; }
                public string subject { get; set; }
            }

            public class LegalForm
            {
                public string text { get; set; }
                public int? id { get; set; }
            }

            public class Email
            {
                public DateTime? acc { get; set; }
                public string score { get; set; }
                public string email { get; set; }
            }

            public class MagnitudeHistory
            {
                public int? magnitude_id { get; set; }
                public int? year { get; set; }
            }

            public class Web
            {
                public DateTime? acc { get; set; }
                public string url { get; set; }
                public string score { get; set; }
            }

            public class Industry
            {
                public string text { get; set; }
                public string id { get; set; }
            }

            public class Magnitude
            {
                public string trend { get; set; }
                public string lower_bound { get; set; }
                public int? id { get; set; }
                public string upper_bound { get; set; }
                public string text { get; set; }
            }

            public class Mobile
            {
                public DateTime? acc { get; set; }
                public string score { get; set; }
                public string number { get; set; }
            }

            public class Turnover
            {
                public string trend { get; set; }
                public string lower_bound { get; set; }
                public int? id { get; set; }
                public string upper_bound { get; set; }
                public string text { get; set; }
            }








            public class CoreCompanyStructure2
            {
                public Firma ToFirma()
                {
                    var f = new Firma();
                    f.ICO = HlidacStatu.Util.ParseTools.MerkIcoToICO(this.regno.ToString());
                    f.DIC = this.vatno;
                    f.DatovaSchranka = new string[] { this.databox_id };
                    f.Datum_Zapisu_OR = this.estab_date;
                    f.Jmeno = this.name;
                    f.Kod_PF = this.legal_form.id;

                    f.NACE = this.secondary_industries != null ?
                                new string[] { this.industry.id }.Concat(this.secondary_industries.Select(m => m.id)).ToArray()
                                : new string[] { this.industry.id };

                    //f.Popis = 
                    f.Source = this.link;
                    f.Stav_subjektu = is_active ? 1 : 0;
                    //f.VersionUpdate;
                    return f;
                }


                public Body body { get; set; }
                public Owning_Type owning_type { get; set; }
                public Bank_Accounts[] bank_accounts { get; set; }
                public string[] phones { get; set; }
                public object[] twitter { get; set; }
                public Secondary_Industries[] secondary_industries { get; set; }
                public int? vzp_debt { get; set; }
                public object[] linkedin { get; set; }
                public bool is_unreliable_vatpayer { get; set; }
                public bool is_vatpayer { get; set; }
                public int? regno { get; set; }
                public string databox_id { get; set; }
                public object salutation { get; set; }
                public int? years_in_business { get; set; }
                public Turnover_History[] turnover_history { get; set; }
                public object taxno { get; set; }
                public Court court { get; set; }
                public Profit profit { get; set; }
                public object vat_registration_type { get; set; }
                public Bank_Accounts_Balance bank_accounts_balance { get; set; }
                public Ebidta ebidta { get; set; }
                public Esi[] esi { get; set; }
                public Gps gps { get; set; }
                public Status status { get; set; }
                public Contract[] contracts { get; set; }
                public bool is_active { get; set; }
                public Legal_Form legal_form { get; set; }
                public string link { get; set; }
                public int? business_premises_count { get; set; }
                public Trademark[] trademarks { get; set; }
                public Address address { get; set; }
                public Email[] emails { get; set; }
                public Magnitude_History[] magnitude_history { get; set; }
                public object unreliable_vatpayer_since { get; set; }
                public string vatno { get; set; }
                public string name { get; set; }
                public Web[] webs { get; set; }
                public Industry industry { get; set; }
                public Magnitude magnitude { get; set; }
                public Phone[] mobiles { get; set; }
                public object insolvency_cases { get; set; }
                public object[] facebook { get; set; }
                public DateTime? estab_date { get; set; }
                public Turnover turnover { get; set; }

                public class Phone
                {
                    public string score { get; set; }
                    public string number { get; set; }
                }

                public class Web
                {
                    public string score { get; set; }
                    public string url { get; set; }
                }

                public class Email
                {
                    public string score { get; set; }
                    public string email { get; set; }
                }
                public class Body
                {
                    public Person[] persons { get; set; }
                    public int? average_age { get; set; }
                }

                public class Person
                {
                    public string first_name { get; set; }
                    public string last_name { get; set; }
                    public string company_role { get; set; }
                    public string gender { get; set; }
                    public int? age { get; set; }
                    public string link { get; set; }
                    public string country_code { get; set; }
                    public string degree_after { get; set; }
                    public string salutation { get; set; }
                    public string degree_before { get; set; }
                }

                public class Owning_Type
                {
                    public string text { get; set; }
                    public int? id { get; set; }
                }

                public class Court
                {
                    public string file_nr { get; set; }
                    public string name { get; set; }
                }

                public class Profit
                {
                }

                public class Bank_Accounts_Balance
                {
                }

                public class Ebidta
                {
                }

                public class Gps
                {
                    public float? lat { get; set; }
                    public float? lon { get; set; }
                }

                public class Status
                {
                    public string text { get; set; }
                    public int? id { get; set; }
                }

                public class Legal_Form
                {
                    public string text { get; set; }
                    public int? id { get; set; }
                }

                public class Address
                {
                    public string number_descriptive { get; set; }
                    public string municipality_part { get; set; }
                    public Region region { get; set; }
                    public string municipality { get; set; }
                    public string number { get; set; }
                    public County county { get; set; }
                    public string municipality_first { get; set; }
                    public string street_fixed { get; set; }
                    public string street { get; set; }
                    public int? postal_code { get; set; }
                    public string number_orientation { get; set; }
                    public string country_code { get; set; }
                }

                public class Region
                {
                    public string text { get; set; }
                    public string id { get; set; }
                }

                public class County
                {
                    public string text { get; set; }
                    public string id { get; set; }
                }

                public class Industry
                {
                    public string text { get; set; }
                    public string id { get; set; }
                }

                public class Magnitude
                {
                    public string trend { get; set; }
                    public string lower_bound { get; set; }
                    public int? id { get; set; }
                    public string upper_bound { get; set; }
                    public string text { get; set; }
                }

                public class Turnover
                {
                    public string trend { get; set; }
                    public string lower_bound { get; set; }
                    public int? id { get; set; }
                    public string upper_bound { get; set; }
                    public string text { get; set; }
                }

                public class Bank_Accounts
                {
                    public string account_number { get; set; }
                    public string bank_code { get; set; }
                }

                public class Secondary_Industries
                {
                    public string text { get; set; }
                    public string id { get; set; }
                }

                public class Turnover_History
                {
                    public int? turnover_id { get; set; }
                    public int? year { get; set; }
                }

                public class Esi
                {
                    public string status { get; set; }
                    public DateTime? allocation_date { get; set; }
                    public string project_name { get; set; }
                    public string esi_program { get; set; }
                    public DateTime? iterim_payment_date { get; set; }
                    public int? allocation_amount { get; set; }
                    public string eu_fund { get; set; }
                    public int? total_paid_from_project_start { get; set; }
                }

                public class Contract
                {
                    public string url { get; set; }
                    public string gov_name { get; set; }
                    public DateTime? published_date { get; set; }
                    public DateTime? contract_date { get; set; }
                    public string price_foreign_currency { get; set; }
                    public int? gov_regno { get; set; }
                    public float? price { get; set; }
                    public bool has_multiple_contractors { get; set; }
                    public string subject { get; set; }
                    public float? price_vat { get; set; }
                }

                public class Trademark
                {
                    public string status { get; set; }
                    public string tm_type { get; set; }
                    public DateTime? stamp_renewed { get; set; }
                    public string url { get; set; }
                    public int? nr_application { get; set; }
                    public string nr_register { get; set; }
                    public string product_or_service_classes { get; set; }
                    public string source { get; set; }
                    public DateTime? stamp_application { get; set; }
                    public string reproduction { get; set; }
                    public string image_classes { get; set; }
                    public DateTime? stamp_registered { get; set; }
                    public string url_doc { get; set; }
                    public string wording { get; set; }
                }

                public class Magnitude_History
                {
                    public int? magnitude_id { get; set; }
                    public int? year { get; set; }
                }

            }


        }
    }
}