using Devmasters.Core;
using Devmasters.Core.Collections;
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
                return "Add Missing Data";
            }
        }

        public void Update(ref Smlouva item)
        {
            //return; //DOTO
            //check missing DS/ICO

            Lib.Data.Smlouva.Subjekt subj = item.Platce;
            //check formal valid ICO
            string ico = subj.ico;
            if (!string.IsNullOrEmpty(ico) && !Devmasters.Core.TextUtil.IsNumeric(ico))
            {
                //neco spatne v ICO
                ico = System.Text.RegularExpressions.Regex.Replace(ico.ToUpper(), @"[^0-9\-.,]", string.Empty);
                item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Opraveno IČO subjektu", "", "platce.ico", subj.ico, ico, this));
                subj.ico = ico;
            }

            if (string.IsNullOrEmpty(subj.ico) && !string.IsNullOrEmpty(subj.datovaSchranka))
            {
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromDS(subj.datovaSchranka, true);
                if (Firma.IsValid(f))
                {
                    item.Platce.ico = f.ICO;
                    item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", "platce.ico", "", f.ICO, this));
                }
            }
            else if (!string.IsNullOrEmpty(subj.ico) && string.IsNullOrEmpty(subj.datovaSchranka))
            {
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromIco(subj.ico, true);
                if (Firma.IsValid(f) && f.DatovaSchranka != null && f.DatovaSchranka.Length > 0)
                {
                    item.Platce.datovaSchranka = f.DatovaSchranka[0];
                    item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", "platce.datovaSchranka", "", f.DatovaSchranka[0], this));
                }
            }
            else if (string.IsNullOrEmpty(subj.ico) && string.IsNullOrEmpty(subj.datovaSchranka) && !string.IsNullOrEmpty(subj.nazev))
            {
                //based on name
                //simple compare now
                if (Lib.Data.Firma.Koncovky.Any(m => subj.nazev.Contains(m)))
                {
                    Lib.Data.Firma f = Lib.Data.Firma.FromName(subj.nazev, true);
                    if (Firma.IsValid(f))
                    {
                        item.Platce.ico = f.ICO;
                        item.Platce.datovaSchranka = f.DatovaSchranka.Length > 0 ? f.DatovaSchranka[0] : "";
                        item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", "Platce.ico", "", f.ICO, this));
                        if (f.DatovaSchranka.Length > 0)
                            item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", "Platce.datovaSchranka", "", f.DatovaSchranka[0], this));
                    }
                    else
                    {
                        //malinko uprav nazev, zrus koncovku  aposledni carku
                        string modifNazev = Lib.Data.Firma.JmenoBezKoncovky(subj.nazev);


                        f = Lib.Data.Firma.FromName(modifNazev, true);
                        if (Firma.IsValid(f))
                        {
                            item.Platce.ico = f.ICO;
                            item.Platce.datovaSchranka = f.DatovaSchranka.Length > 0 ? f.DatovaSchranka[0] : "";
                            item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", "Platce.ico", "", f.ICO, this));
                            if (f.DatovaSchranka.Length > 0)
                                item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", "Platce.datovaSchranka", "", f.DatovaSchranka[0], this));

                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(subj.nazev) && !string.IsNullOrEmpty(subj.ico))
            {
                //dopln chybejici jmeno a adresu
                HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromIco(subj.ico, true);
                if (Firma.IsValid(f))
                {
                    subj.nazev = f.Jmeno;
                    item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněn Název subjektu", "", "Platce.nazev", "", f.ICO, this));
                }
            }


            for (int i = 0; i < item.Prijemce.Count(); i++)
            {
                Smlouva.Subjekt ss = item.Prijemce[i];
                ico = ss.ico;
                if (!string.IsNullOrEmpty(ico) && !Devmasters.Core.TextUtil.IsNumeric(ico))
                {
                    //neco spatne v ICO
                    ico = System.Text.RegularExpressions.Regex.Replace(ico.ToUpper(), @"[^A-Z0-9\-.,]", string.Empty);
                    item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Opraveno IČO subjektu", "", "item.Prijemce[" + i.ToString() + "].ico", ss.ico, ico, this));
                    ss.ico = ico;
                }

                if (string.IsNullOrEmpty(ss.ico) && !string.IsNullOrEmpty(ss.datovaSchranka))
                {
                    HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromDS(ss.datovaSchranka, true);
                    if (Firma.IsValid(f))
                    {
                        item.Prijemce[i].ico = f.ICO;
                        item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO smluvní strany", "", "Prijemce[" + i.ToString() + "].ico", "", f.ICO, this));
                    }
                }
                else if (!string.IsNullOrEmpty(ss.ico) && string.IsNullOrEmpty(ss.datovaSchranka))
                {
                    HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromIco(ss.ico, true);
                    if (Firma.IsValid(f) && f.DatovaSchranka != null && f.DatovaSchranka.Length > 0)
                    {
                        item.Prijemce[i].datovaSchranka = f.DatovaSchranka[0];
                        item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka smluvní strany", "", "item.Prijemce[" + i.ToString() + "].datovaSchranka", "", f.DatovaSchranka[0], this));
                    }
                }
                else if (string.IsNullOrEmpty(ss.ico) && string.IsNullOrEmpty(ss.datovaSchranka) && !string.IsNullOrEmpty(ss.nazev))
                {
                    //based on name
                    //simple compare now
                    if (Lib.Data.Firma.Koncovky.Any(m => ss.nazev.Contains(m)))
                    {
                        Lib.Data.Firma f = Lib.Data.Firma.FromName(ss.nazev, true);
                        if (Firma.IsValid(f))
                        {
                            item.Prijemce[i].ico = f.ICO;
                            item.Prijemce[i].datovaSchranka = f.DatovaSchranka.Length > 0 ? f.DatovaSchranka[0] : "";
                            item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO smluvní strany", "", "item.Prijemce[" + i.ToString() + "].ico", "", f.ICO, this));
                            if (f.DatovaSchranka.Length > 0)
                                item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka smluvní strany", "", "item.Prijemce[" + i.ToString() + "].datovaSchranka", "", f.DatovaSchranka[0], this));
                        }
                        else
                        {
                            //malinko uprav nazev, zrus koncovku  aposledni carku
                            string modifNazev = Lib.Data.Firma.JmenoBezKoncovky(ss.nazev);


                            f = Lib.Data.Firma.FromName(modifNazev, true);
                            if (Firma.IsValid(f))
                            {
                                item.Prijemce[i].ico = f.ICO;
                                item.Prijemce[i].datovaSchranka = f.DatovaSchranka.Length > 0 ? f.DatovaSchranka[0] : "";
                                item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněno IČO subjektu", "", "item.Prijemce[" + i.ToString() + "].ico", "", f.ICO, this));
                                if (f.DatovaSchranka.Length > 0)
                                    item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněna datová schránka subjektu", "", "item.Prijemce[" + i.ToString() + "].datovaSchranka", "", f.DatovaSchranka[0], this));

                            }
                        }
                    }

                }
                if (string.IsNullOrEmpty(ss.nazev) && !string.IsNullOrEmpty(ss.ico))
                {
                    //dopln chybejici jmeno a adresu
                    HlidacStatu.Lib.Data.Firma f = HlidacStatu.Lib.Data.Firma.FromIco(ss.ico, true);
                    if (Firma.IsValid(f))
                    {
                        item.Prijemce[i].nazev = f.Jmeno;
                        item.Enhancements = item.Enhancements.AddOrUpdate(new Enhancement("Doplněn název subjektu", "", "Platce.Prijemce[" + i.ToString() + "].nazev", "", f.Jmeno, this));
                    }
                }



            }
        }



    }
}

