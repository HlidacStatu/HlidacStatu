using System;
using System.Linq;
using System.Collections.Generic;
using HlidacStatu.Util;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Data.Entity.Infrastructure;
using static Devmasters.DT.Util;
using HlidacStatu.Lib.Db.Insolvence;

namespace HlidacStatu.Lib.Data.Insolvence
{
    public partial class Rizeni
    {

        public bool ExistInDb()
        {
            using (HlidacStatu.Lib.Db.Insolvence.InsolvenceEntities idb = new Db.Insolvence.InsolvenceEntities())
            {
                return idb.Rizeni.Any(m => m.SpisovaZnacka == this.SpisovaZnacka);
            }
        }
        public void SaveToDb(bool rewrite, bool skipOsobaIdLink = false)
        {
            PrepareForSave(skipOsobaIdLink);
            using (HlidacStatu.Lib.Db.Insolvence.InsolvenceEntities idb = new Db.Insolvence.InsolvenceEntities())
            {
                var exists = idb.Rizeni.Where(m => m.SpisovaZnacka == this.SpisovaZnacka)?.ToList()?.FirstOrDefault();
                bool addNew = exists == null;

                if (exists != null && rewrite == true)
                {
                    foreach (var d in idb.Dokumenty.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList())
                        idb.Dokumenty.Remove(d);

                    foreach (var d in idb.Dluznici.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList())
                        idb.Dluznici.Remove(d);
                    foreach (var d in idb.Veritele.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList())
                        idb.Veritele.Remove(d);
                    foreach (var d in idb.Spravci.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList())
                        idb.Spravci.Remove(d);

                    idb.Rizeni.Remove(exists);

                    idb.SaveChanges();
                    addNew = true;
                }

                if (addNew)
                {
                    var r = new HlidacStatu.Lib.Db.Insolvence.Rizeni();
                    r.DatumZalozeni = this.DatumZalozeni ?? new DateTime(1990, 1, 1);
                    r.SpisovaZnacka = this.SpisovaZnacka;
                    r.OnRadar = this.OnRadar;
                    r.PosledniZmena = this.PosledniZmena;
                    r.Soud = this.Soud ?? "";
                    r.Stav = this.Stav ?? "";

                    idb.Rizeni.Add(r);

                    foreach (var td in this.Dluznici)
                    {
                        var d = ToIOsoba<Dluznici>(td);
                        idb.Dluznici.Add(d);
                    }
                    foreach (var td in this.Veritele)
                    {
                        var d = ToIOsoba<Veritele>(td);
                        idb.Veritele.Add(d);
                    }
                    foreach (var td in this.Spravci)
                    {
                        var d = ToIOsoba<Spravci>(td);
                        idb.Spravci.Add(d);
                    }
                    foreach (var td in this.Dokumenty)
                    {
                        var d = ToDbDokument(td);
                        idb.Dokumenty.Add(d);
                    }
                }
                else // update existing
                {
                    var sameR = exists.DatumZalozeni == (this.DatumZalozeni ?? new DateTime(1990, 1, 1))
                                && exists.SpisovaZnacka == this.SpisovaZnacka
                                && exists.OnRadar == this.OnRadar
                                && exists.PosledniZmena == this.PosledniZmena
                                && exists.Soud == (this.Soud ?? "")
                                && exists.Stav == (this.Stav ?? "");

                    if (sameR == false)
                    {
                        exists.DatumZalozeni = (this.DatumZalozeni ?? new DateTime(1990, 1, 1));
                        exists.SpisovaZnacka = this.SpisovaZnacka;
                        exists.OnRadar = this.OnRadar;
                        exists.PosledniZmena = this.PosledniZmena;
                        exists.Soud = (this.Soud ?? "");
                        exists.Stav = (this.Stav ?? "");
                    }

                    #region Dluznici
                    var dbDluznici = idb.Dluznici.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList();
                    //update existing
                    foreach (var d in this.Dluznici)
                    {
                        for (int i = 0; i < dbDluznici.Count(); i++)
                        {
                            var dd = dbDluznici[i];
                            if (d.IdOsoby == dd.IdOsoby && d.IdPuvodce == dd.IdPuvodce)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToIOsoba<Dluznici>(d), dd, false, "pk"))
                                    dd = (Dluznici)UpdateIOsoba(d, dd);
                            }
                        }
                    }

                    if (this.Dluznici.Count() > dbDluznici.Count())
                    {
                        foreach (var d in this.Dluznici)
                            if (!dbDluznici.Any(m => m.IdOsoby == d.IdOsoby && m.IdPuvodce == d.IdPuvodce))
                                idb.Dluznici.Add(ToIOsoba<Dluznici>(d));
                    }

                    if (this.Dluznici.Count() < dbDluznici.Count())
                    {
                        //remove all and add orig
                        foreach (var d in dbDluznici)
                            idb.Dluznici.Remove(d);
                        foreach (var d in this.Dluznici)
                            idb.Dluznici.Add(ToIOsoba<Dluznici>(d));
                    }
                    #endregion

                    #region Veritele
                    var dbVeritele = idb.Veritele.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList();
                    //update existing
                    foreach (var d in this.Veritele)
                    {
                        for (int i = 0; i < dbVeritele.Count(); i++)
                        {
                            var dd = dbVeritele[i];
                            if (d.IdOsoby == dd.IdOsoby && d.IdPuvodce == dd.IdPuvodce)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToIOsoba<Veritele>(d), dd, false, "pk"))
                                    dd = (Veritele)UpdateIOsoba(d, dd);
                            }
                        }
                    }

                    if (this.Veritele.Count() > dbVeritele.Count())
                    {
                        foreach (var d in this.Veritele)
                            if (!dbVeritele.Any(m => m.IdOsoby == d.IdOsoby && m.IdPuvodce == d.IdPuvodce))
                                idb.Veritele.Add(ToIOsoba<Veritele>(d));
                    }

                    if (this.Veritele.Count() < dbVeritele.Count())
                    {
                        //remove all and add orig
                        foreach (var d in dbVeritele)
                            idb.Veritele.Remove(d);
                        foreach (var d in this.Veritele)
                            idb.Veritele.Add(ToIOsoba<Veritele>(d));
                    }
                    #endregion

                    #region Spravci
                    var dbSpravci = idb.Spravci.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList();
                    //update existing
                    foreach (var d in this.Spravci)
                    {
                        for (int i = 0; i < dbSpravci.Count(); i++)
                        {
                            var dd = dbSpravci[i];
                            if (d.IdOsoby == dd.IdOsoby && d.IdPuvodce == dd.IdPuvodce)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToIOsoba<Spravci>(d), dd, false, "pk"))
                                    dd = (Spravci)UpdateIOsoba(d, dd);
                            }
                        }
                    }

                    if (this.Spravci.Count() > dbSpravci.Count())
                    {
                        foreach (var d in this.Spravci)
                            if (!dbSpravci.Any(m => m.IdOsoby == d.IdOsoby && m.IdPuvodce == d.IdPuvodce))
                                idb.Spravci.Add(ToIOsoba<Spravci>(d));
                    }

                    if (this.Spravci.Count() < dbSpravci.Count())
                    {
                        //remove all and add orig
                        foreach (var d in dbSpravci)
                            idb.Spravci.Remove(d);
                        foreach (var d in this.Spravci)
                            idb.Spravci.Add(ToIOsoba<Spravci>(d));
                    }
                    #endregion

                    #region Dokumenty
                    if (System.Diagnostics.Debugger.IsAttached)
                        idb.Database.Log = Console.WriteLine;

                    var dbDokumenty = idb.Dokumenty.Where(m => m.RizeniId == exists.SpisovaZnacka).ToList();
                    //update existing
                    foreach (var d in this.Dokumenty)
                    {
                        for (int i = 0; i < dbDokumenty.Count(); i++)
                        {
                            var dd = dbDokumenty[i];
                            if (d.Id == dd.DokumentId)
                            {
                                //same
                                if (!Validators.AreObjectsEqual(ToDbDokument(d), dd, false))
                                {
                                    dd.DokumentId = d.Id;
                                    dd.Length = (int)d.Lenght;
                                    dd.Oddil = d.Oddil;
                                    dd.Popis = d.Popis;
                                    dd.TypUdalosti = d.TypUdalosti;
                                    dd.Url = d.Url;
                                    dd.WordCount = (int)d.WordCount;
                                }
                            }
                        }
                    }

                    if (this.Dokumenty.Count() > dbDokumenty.Count())
                    {
                        foreach (var d in this.Dokumenty)
                            if (!dbDokumenty.Any(m => m.DokumentId == d.Id))
                                idb.Dokumenty.Add(ToDbDokument(d));
                    }

                    if (this.Dokumenty.Count() < dbDokumenty.Count())
                    {
                        foreach (var d in dbDokumenty)
                            if (!this.Dokumenty.Any(m => m.Id == d.DokumentId))
                                idb.Dokumenty.Remove(d);
                    }
                    #endregion

                }



                try
                {
                    if (idb.ChangeTracker.HasChanges())
                    {
                        HlidacStatu.Util.Consts.Logger.Info($"Updating Rizeni into DB {this.SpisovaZnacka}, {idb.ChangeTracker.Entries().Count(m => m.State != System.Data.Entity.EntityState.Unchanged)} changes.");
                    }
                    idb.Database.CommandTimeout = 120;
                    idb.SaveChanges();

                }

                catch (DbEntityValidationException e)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();

                    foreach (var eve in e.EntityValidationErrors)
                    {
                        sb.AppendFormat(@"Entity of type ""{0}"" in state ""{1}"" 
                   has the following validation errors:\n",
                            eve.Entry.Entity.GetType().Name,
                            eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            sb.AppendFormat(@"- Property: ""{0}"", Error: ""{1}""\n",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    Debug.WriteLine(sb.ToString());

                    throw new DbEntityValidationException(sb.ToString(), e);
                }
                catch (DbUpdateException)
                {
                    //Add your code to inspect the inner exception and/or
                    //e.Entries here.
                    //Or just use the debugger.
                    //Added this catch (after the comments below) to make it more obvious 
                    //how this code might help this specific problem
                    throw;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                    throw;
                }
            }
        }

        public IOsoba UpdateIOsoba(Osoba d, IOsoba dd)
        {
            dd.DatumNarozeni = d.DatumNarozeni < MinSqlDate ? MinSqlDate : d.DatumNarozeni;
            dd.ICO = d.ICO;
            dd.Mesto = d.Mesto;
            dd.Okres = d.Okres;
            dd.PlneJmeno = d.PlneJmeno;
            dd.PSC = d.Psc;
            dd.RC = d.Rc;
            dd.Role = d.Role;
            dd.Typ = d.Typ;
            dd.Zeme = d.Zeme;
            dd.OsobaId = d.OsobaId;
            dd.Zalozen = d.Zalozen;
            dd.Odstranen = d.Odstranen;
            return dd;
        }

        private T ToIOsoba<T>(Osoba td)
            where T : IOsoba, new()
        {
            IOsoba d = new T();
            d.DatumNarozeni = td.DatumNarozeni < MinSqlDate ? MinSqlDate : td.DatumNarozeni;
            d.ICO = td.ICO;
            d.IdOsoby = td.IdOsoby;
            d.IdPuvodce = td.IdPuvodce;
            d.Mesto = Devmasters.TextUtil.ShortenText(td.Mesto, 150);
            d.Okres = td.Okres;
            d.PlneJmeno = Devmasters.TextUtil.ShortenText(td.PlneJmeno, 250);
            d.PSC = td.Psc;
            d.RC = td.Rc;
            d.RizeniId = this.SpisovaZnacka;
            d.Role = td.Role;
            d.Typ = td.Typ;
            d.Zeme = td.Zeme;
            d.OsobaId = td.OsobaId;
            d.Zalozen = td.Zalozen;
            d.Odstranen = td.Odstranen;

            return (T)d;
        }


        private Dokumenty ToDbDokument(Dokument td)
        {
            Db.Insolvence.Dokumenty d = new Db.Insolvence.Dokumenty();
            d.DatumVlozeni = td.DatumVlozeni < MinSqlDate
                                ? MinSqlDate : td.DatumVlozeni;
            d.DokumentId = td.Id;
            d.Length = (int)td.Lenght;
            d.Oddil = td.Oddil;
            d.Popis = td.Popis;
            d.RizeniId = this.SpisovaZnacka;
            d.TypUdalosti = td.TypUdalosti;
            d.Url = td.Url;
            d.WordCount = (int)td.WordCount;
            return d;
        }

    }
}
