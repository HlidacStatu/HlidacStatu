using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data
{

    public partial class Review
    {
        public enum ReviewAction
        {
            Denied = 0,
            Accepted = 1,
        }

        public enum ItemTypes
        {
            osobaPhoto,
            osobaPopis,
        }

        public ItemTypes ItemType
        {
            get
            {
                return EnumsNET.Enums.Parse<ItemTypes>(this.itemType);
            }
            set
            {
                this.itemType = value.ToString();
            }
        }

        public void Accepted(string user)
        {
            this.reviewedBy = user;
            this.reviewed = DateTime.Now;
            this.reviewResult = (int)ReviewAction.Accepted;

            switch (this.ItemType)
            {
                case ItemTypes.osobaPhoto:
                    var data = Newtonsoft.Json.Linq.JObject.Parse(this.newValue);
                    Osoba o = Osoby.GetByNameId.Get(data.Value<string>("nameId"));
                    if (o != null)
                    {
                        var path = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "original.uploaded.jpg");
                        var pathSmall = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.uploaded.jpg");
                        HlidacStatu.Util.IOTools.MoveFile(pathSmall, HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.jpg"));

                        if (Devmasters.Core.TextUtil.IsValidEmail(this.createdBy))
                        {
                            using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                            {
                                var m = new System.Net.Mail.MailMessage()
                                {
                                    From = new System.Net.Mail.MailAddress("info@hlidacstatu.cz"),
                                    Subject = o.FullName() + " - nová fotka schválena.",
                                    IsBodyHtml = false,
                                    Body = "Vámi nahraná fotka byla právě schválena. Děkujeme za pomoc.\n\nHlídač Státu"
                                };
                                m.BodyEncoding = System.Text.Encoding.UTF8;
                                m.SubjectEncoding = System.Text.Encoding.UTF8;

                                m.To.Add(this.createdBy);
                                m.Bcc.Add("michal@michalblaha.cz");
                                smtp.Send(m);
                            }
                        }

                    }
                    break;
                default:
                    break;
            }

            this.Save();
        }

        public void Denied(string user, string reason)
        {
            this.reviewedBy = user;
            this.reviewed = DateTime.Now;
            this.reviewResult = (int)ReviewAction.Denied;
            this.comment = reason;
            if (Devmasters.Core.TextUtil.IsValidEmail(this.createdBy))
            {
                switch (this.ItemType)
                {
                    case ItemTypes.osobaPhoto:
                        using (System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient())
                        {
                            var data = Newtonsoft.Json.Linq.JObject.Parse(this.newValue);
                            Osoba o = Osoby.GetByNameId.Get(data.Value<string>("nameId"));
                            if (o != null)
                            {
                                var m = new System.Net.Mail.MailMessage()
                                {
                                    From = new System.Net.Mail.MailAddress("info@hlidacstatu.cz"),
                                    Subject = o.FullName() + " - nová fotka neschválena.",
                                    IsBodyHtml = false,
                                    Body = "Vámi nahraná fotka nebyla schválena k uveřejnění.\nDůvod:" + reason + "\n\nDěkujeme za pomoc.\n\nHlídač Státu"
                                };
                                m.BodyEncoding = System.Text.Encoding.UTF8;
                                m.SubjectEncoding = System.Text.Encoding.UTF8;
                                m.To.Add(this.createdBy);
                                m.Bcc.Add("michal@michalblaha.cz");
                                smtp.Send(m);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
            this.Save();
        }

        public string RenderNewValueToHtml()
        {
            return RenderValueToHtml(false);
        }
        public string RenderOldValueToHtml()
        {
            return RenderValueToHtml(true);
        }
        private string RenderValueToHtml(bool oldValue)
        {
            switch (this.ItemType)
            {
                case ItemTypes.osobaPhoto:
                    var data = Newtonsoft.Json.Linq.JObject.Parse(this.newValue);
                    Osoba o = Osoby.GetByNameId.Get(data.Value<string>("nameId"));
                    if (o != null)
                    {
                        if (oldValue)
                            return "<img style='width:150px;height:auto; border:solid #d0d0d0 1px; margin:5px;' src='" + o.GetPhotoUrl() + "' width='150' height='150' />" + o.FullNameWithNarozeni();
                        else
                        {
                            var fn = HlidacStatu.Lib.Init.OsobaFotky.GetFullPath(o, "small.uploaded.jpg");
                            if (System.IO.File.Exists(fn))
                                return "<img style='width:150px;height:auto; border:solid #d0d0d0 1px; margin:5px;' src='data:image/jpeg;base64," 
                                    + System.Convert.ToBase64String(System.IO.File.ReadAllBytes(fn), Base64FormattingOptions.None) + "' />" + o.FullNameWithNarozeni();
                            else
                                return "Žádná fotka" + o.FullNameWithNarozeni();


                        }
                    }
                    return "Osoba nenalezena";
                case ItemTypes.osobaPopis:
                    if (oldValue)
                    {
                        var data1 = Newtonsoft.Json.Linq.JObject.Parse(this.oldValue);
                        var osobaid = data1.Value<string>("id");
                        if (!string.IsNullOrEmpty(osobaid))
                        {
                            var o1 = Osoby.GetByNameId.Get(data1.Value<string>("id"));
                            if (o1 != null)
                            {
                                return $"<a href='{o1.GetUrlOnWebsite()}'>{o1.FullNameWithNarozeni()}</a>";
                            }
                        }
                        return  "(neznama osoba)";
                    }
                    else
                        return this.newValue;
                default:
                    return string.Empty;
            }

        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            using (Lib.Data.DbEntities db = new Data.DbEntities())
            {
                db.Review.Attach(this);
                if (this.Id == 0)
                    db.Entry(this).State = System.Data.Entity.EntityState.Added;               
                else
                    db.Entry(this).State = System.Data.Entity.EntityState.Modified;

                db.SaveChanges();

            }
        }
    }
}
