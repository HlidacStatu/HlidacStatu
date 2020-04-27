﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace HlidacStatu.Lib.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DbEntities : DbContext
    {
        public DbEntities()
            : base("name=DbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<DatoveSchranky> DatoveSchrankies { get; set; }
        public virtual DbSet<WatchDog> WatchDogs { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<AspNetUserToken> AspNetUserTokens { get; set; }
        public virtual DbSet<SmlouvyId> SmlouvyIds { get; set; }
        public virtual DbSet<FirmaVazby> FirmaVazby { get; set; }
        public virtual DbSet<Osoba> Osoba { get; set; }
        public virtual DbSet<OsobaExternalId> OsobaExternalId { get; set; }
        public virtual DbSet<OsobaVazby> OsobaVazby { get; set; }
        public virtual DbSet<FirmaEvent> FirmaEvent { get; set; }
        public virtual DbSet<InvoiceItems> InvoiceItems { get; set; }
        public virtual DbSet<Invoices> Invoices { get; set; }
        public virtual DbSet<Audit> Audit { get; set; }
        public virtual DbSet<NespolehlivyPlatceDPH> NespolehlivyPlatceDPH { get; set; }
        public virtual DbSet<Review> Review { get; set; }
        public virtual DbSet<Bookmark> Bookmark { get; set; }
        public virtual DbSet<UserOptions> UserOptions { get; set; }
        public virtual DbSet<ItemToOcrQueue> ItemToOcrQueue { get; set; }
        public virtual DbSet<EventType> EventType { get; set; }
        public virtual DbSet<OsobaEvent> OsobaEvent { get; set; }
        public virtual DbSet<FirmaHint> FirmaHint { get; set; }
        public virtual DbSet<UcetniJednotka> UcetniJednotka { get; set; }
        public virtual DbSet<TipUrl> TipUrl { get; set; }
        public virtual DbSet<ClassificationOverride> ClassificationOverride { get; set; }
    
        public virtual int Firma_Save(string iCO, string dIC, Nullable<System.DateTime> datum_zapisu_OR, Nullable<byte> stav_subjektu, string jmeno, string jmenoAscii, Nullable<int> kod_PF, string source, string popis, Nullable<int> versionUpdate, string krajId, string okresId, Nullable<int> status)
        {
            var iCOParameter = iCO != null ?
                new ObjectParameter("ICO", iCO) :
                new ObjectParameter("ICO", typeof(string));
    
            var dICParameter = dIC != null ?
                new ObjectParameter("DIC", dIC) :
                new ObjectParameter("DIC", typeof(string));
    
            var datum_zapisu_ORParameter = datum_zapisu_OR.HasValue ?
                new ObjectParameter("Datum_zapisu_OR", datum_zapisu_OR) :
                new ObjectParameter("Datum_zapisu_OR", typeof(System.DateTime));
    
            var stav_subjektuParameter = stav_subjektu.HasValue ?
                new ObjectParameter("Stav_subjektu", stav_subjektu) :
                new ObjectParameter("Stav_subjektu", typeof(byte));
    
            var jmenoParameter = jmeno != null ?
                new ObjectParameter("Jmeno", jmeno) :
                new ObjectParameter("Jmeno", typeof(string));
    
            var jmenoAsciiParameter = jmenoAscii != null ?
                new ObjectParameter("JmenoAscii", jmenoAscii) :
                new ObjectParameter("JmenoAscii", typeof(string));
    
            var kod_PFParameter = kod_PF.HasValue ?
                new ObjectParameter("Kod_PF", kod_PF) :
                new ObjectParameter("Kod_PF", typeof(int));
    
            var sourceParameter = source != null ?
                new ObjectParameter("Source", source) :
                new ObjectParameter("Source", typeof(string));
    
            var popisParameter = popis != null ?
                new ObjectParameter("Popis", popis) :
                new ObjectParameter("Popis", typeof(string));
    
            var versionUpdateParameter = versionUpdate.HasValue ?
                new ObjectParameter("VersionUpdate", versionUpdate) :
                new ObjectParameter("VersionUpdate", typeof(int));
    
            var krajIdParameter = krajId != null ?
                new ObjectParameter("KrajId", krajId) :
                new ObjectParameter("KrajId", typeof(string));
    
            var okresIdParameter = okresId != null ?
                new ObjectParameter("okresId", okresId) :
                new ObjectParameter("okresId", typeof(string));
    
            var statusParameter = status.HasValue ?
                new ObjectParameter("status", status) :
                new ObjectParameter("status", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("Firma_Save", iCOParameter, dICParameter, datum_zapisu_ORParameter, stav_subjektuParameter, jmenoParameter, jmenoAsciiParameter, kod_PFParameter, sourceParameter, popisParameter, versionUpdateParameter, krajIdParameter, okresIdParameter, statusParameter);
        }
    
        public virtual int SmlouvaId_Save(string id, Nullable<int> active, Nullable<System.DateTime> created, Nullable<System.DateTime> updated)
        {
            var idParameter = id != null ?
                new ObjectParameter("id", id) :
                new ObjectParameter("id", typeof(string));
    
            var activeParameter = active.HasValue ?
                new ObjectParameter("active", active) :
                new ObjectParameter("active", typeof(int));
    
            var createdParameter = created.HasValue ?
                new ObjectParameter("created", created) :
                new ObjectParameter("created", typeof(System.DateTime));
    
            var updatedParameter = updated.HasValue ?
                new ObjectParameter("updated", updated) :
                new ObjectParameter("updated", typeof(System.DateTime));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("SmlouvaId_Save", idParameter, activeParameter, createdParameter, updatedParameter);
        }
    
        public virtual ObjectResult<Nullable<int>> UserOption_Add(Nullable<int> optionId, string userId, string value, Nullable<int> languageId)
        {
            var optionIdParameter = optionId.HasValue ?
                new ObjectParameter("optionId", optionId) :
                new ObjectParameter("optionId", typeof(int));
    
            var userIdParameter = userId != null ?
                new ObjectParameter("userId", userId) :
                new ObjectParameter("userId", typeof(string));
    
            var valueParameter = value != null ?
                new ObjectParameter("value", value) :
                new ObjectParameter("value", typeof(string));
    
            var languageIdParameter = languageId.HasValue ?
                new ObjectParameter("languageId", languageId) :
                new ObjectParameter("languageId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<Nullable<int>>("UserOption_Add", optionIdParameter, userIdParameter, valueParameter, languageIdParameter);
        }
    
        public virtual ObjectResult<UserOption_Get_Result> UserOption_Get(Nullable<int> optionId, string userId, Nullable<int> languageId)
        {
            var optionIdParameter = optionId.HasValue ?
                new ObjectParameter("optionId", optionId) :
                new ObjectParameter("optionId", typeof(int));
    
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var languageIdParameter = languageId.HasValue ?
                new ObjectParameter("languageId", languageId) :
                new ObjectParameter("languageId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<UserOption_Get_Result>("UserOption_Get", optionIdParameter, userIdParameter, languageIdParameter);
        }
    
        public virtual int UserOption_Remove(Nullable<int> optionId, string userId, Nullable<int> languageId)
        {
            var optionIdParameter = optionId.HasValue ?
                new ObjectParameter("optionId", optionId) :
                new ObjectParameter("optionId", typeof(int));
    
            var userIdParameter = userId != null ?
                new ObjectParameter("UserId", userId) :
                new ObjectParameter("UserId", typeof(string));
    
            var languageIdParameter = languageId.HasValue ?
                new ObjectParameter("languageId", languageId) :
                new ObjectParameter("languageId", typeof(int));
    
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("UserOption_Remove", optionIdParameter, userIdParameter, languageIdParameter);
        }
    }
}
