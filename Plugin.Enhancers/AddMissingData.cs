using Devmasters;
using Devmasters.Collections;
using HlidacStatu.Lib;
using HlidacStatu.Lib.Data;
using HlidacStatu.Lib.Enhancers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Plugin.Enhancers
{


    public class AddMissingData : IEnhancer
    {


        public string Description
        {
            get
            {
                return "Add missing data";
            }
        }

        public string Name
        {
            get
            {
                return "AddMissingData";
            }
        }
        public void SetInstanceData(object data) { }


        public bool Update(ref Smlouva item)
        {
            //return; //DOTO
            //check missing DS/ICO
            bool changed = false;

            changed = changed | UpdateSubj(item.Platce, item, "platce");
            changed = changed | UpdateSubj(item.VkladatelDoRejstriku, item, "platce");
            for (int i = 0; i < item.Prijemce.Count(); i++)
            {
                changed = changed | UpdateSubj(item.Prijemce[i], item, $"platce[{i}]");
            }
            return changed;
        }

        public bool UpdateSubj(Smlouva.Subjekt subj, Smlouva _item, string path)
        {

            bool changed = false;
            var zahr = Util.DataValidators.ZahranicniAdresa(subj.adresa);
            if (!string.IsNullOrEmpty(zahr) && !string.IsNullOrEmpty(subj.ico))
            {
                var currPref = Util.ParseTools.GetRegexGroupValue(subj.ico, @"^(?<pref>\w{2}-).{1,}", "pref");
                if (string.IsNullOrEmpty(currPref))
                {
                    //NENI PREFIX, DOPLN HO
                    string newico = zahr + "-" + subj.ico;
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněno zahraniční ID subjektu. Doplněn prefix před ID firmy", "", path + ".ico", newico, subj.ico, this));
                    subj.ico = newico;
                    changed = true;
                }
                else if (currPref != zahr)
                {
                    //je jiny PREFIX, uprav ho
                    string newico = zahr + subj.ico.Substring(2);
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Upraveno zahraniční ID subjektu. Doplněn prefix před ID firmy", "", path + ".ico", zahr + "-" + subj.ico, subj.ico, this));
                    subj.ico = newico;
                    changed = true;
                }
            }
            else
            {
                var currPref2 = Util.ParseTools.GetRegexGroupValue(subj.ico, @"^(?<pref>\w{2}-).{1,}", "pref");
                if (!string.IsNullOrEmpty(currPref2) && subj.ico != null)
                {
                    subj.ico = subj.ico.Replace(currPref2, "");
                    changed = true;
                }
            }
            //check formal valid ICO
            string ico = subj.ico;
            if (!string.IsNullOrEmpty(ico)
                && !Devmasters.TextUtil.IsNumeric(ico)
                && Util.DataValidators.IsZahranicniAdresa(subj.adresa) == false
                )
            {
                //neco spatne v ICO
                ico = System.Text.RegularExpressions.Regex.Replace(ico.ToUpper(), @"[^0-9\-.,]", string.Empty);
                if (Util.DataValidators.CheckCZICO(ico))
                {
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Opraveno IČO subjektu", "", path + ".ico", subj.ico, ico, this));
                    subj.ico = ico;
                    changed = true;
                }
            }

            if (string.IsNullOrEmpty(subj.ico)
                && !string.IsNullOrEmpty(subj.datovaSchranka)
                && Util.DataValidators.IsZahranicniAdresa(subj.adresa) == false
                )
            {
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromDS(subj.datovaSchranka, true);
                if (Firma.IsValid(f))
                {
                    subj.ico = f.ICO;
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", path + ".ico", "", f.ICO, this));
                    changed = true;
                }
            }
            else if (!string.IsNullOrEmpty(subj.ico) && string.IsNullOrEmpty(subj.datovaSchranka))
            {
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromIco(subj.ico, true);
                if (Firma.IsValid(f) && f.DatovaSchranka != null && f.DatovaSchranka.Length > 0)
                {
                    subj.datovaSchranka = f.DatovaSchranka[0];
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", path + ".datovaScranka", "", f.DatovaSchranka[0], this));
                    changed = true;
                }
            }
            else if (string.IsNullOrEmpty(subj.ico)
                        && string.IsNullOrEmpty(subj.datovaSchranka)
                        && !string.IsNullOrEmpty(subj.nazev)
                        && Util.DataValidators.IsZahranicniAdresa(subj.adresa) == false
                    )
            {
                //based on name
                //simple compare now
                if (Lib.Data.Firma.Koncovky.Any(m => subj.nazev.Contains(m)))
                {
                    Lib.Data.Firma f = Lib.Data.Firma.FromName(subj.nazev, true);
                    if (Firma.IsValid(f))
                    {
                        subj.ico = f.ICO;
                        subj.datovaSchranka = f.DatovaSchranka.Length > 0 ? f.DatovaSchranka[0] : "";
                        _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", path + ".ico", "", f.ICO, this));
                        if (f.DatovaSchranka.Length > 0)
                            _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", path +".datovaSchranka", "", f.DatovaSchranka[0], this));
                        changed = true;
                    }
                    else
                    {
                        //malinko uprav nazev, zrus koncovku  aposledni carku
                        string modifNazev = Lib.Data.Firma.JmenoBezKoncovky(subj.nazev);


                        f = Lib.Data.Firma.FromName(modifNazev, true);
                        if (Firma.IsValid(f))
                        {
                            subj.ico = f.ICO;
                            subj.datovaSchranka = f.DatovaSchranka.Length > 0 ? f.DatovaSchranka[0] : "";
                            _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", path +".ico", "", f.ICO, this));
                            if (f.DatovaSchranka.Length > 0)
                                _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", path + ".datovaSchranka", "", f.DatovaSchranka[0], this));
                            changed = true;

                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(subj.nazev) && !string.IsNullOrEmpty(subj.ico))
            {
                //dopln chybejici jmeno 
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromIco(subj.ico, true);
                if (Firma.IsValid(f))
                {
                    subj.nazev = f.Jmeno;
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněn Název subjektu", "", path + ".nazev", "", f.ICO, this));
                    changed = true;
                }
            }
            if (string.IsNullOrEmpty(subj.adresa) && !string.IsNullOrEmpty(subj.ico))
            {
                //dopln chybejici jmeno 
                var fm = HlidacStatu.Lib.Data.External.Merk.FromIcoFull(subj.ico);
                if (fm != null)
                {
                    subj.adresa = fm.address.street + " " + fm.address.number + ", " + fm.address.municipality;
                    _item.Enhancements = _item.Enhancements.AddOrUpdate(new Enhancement("Doplněna adresa subjektu", "", path + ".nazev", "", subj.ico, this));
                    changed = true;
                }
            }

            return changed;
        }



    }
}

