using System;
using System.Linq;

namespace HlidacStatu.Lib.Data
{

    public partial class UserOptions
    {

        public enum ParameterType
        {
            DatLabLastUpdate = 1,

        }

        public ParameterType Parameter
        {
            get
            {
                return (ParameterType)this.OptionId;
            }
            set
            {
                this.OptionId = (int)value;
            }
        }

        public UserOptions()
        {
        }

        protected UserOptions(ParameterType option, int? languageId = null)
        {
            this.LanguageId = languageId;
            this.UserId = null;
            this.OptionId = (int)option;


            var r = new DbEntities().UserOption_Get((int)option, null, LanguageId)
                .FirstOrDefault();
            this.Created = r?.Created ?? DateTime.Now;
            this.Value = r?.Value;
        }

        public UserOptions(AspNetUser user, ParameterType option, int? languageId = null)
        {
            throw new NotImplementedException();
            //this.LanguageId = languageId;
            //this.UserId = user.Id;
            //this.OptionId = (int)option;
        }

        public virtual void Save()
        {
            new DbEntities().UserOption_Add(this.OptionId, this.UserId, this.Value, this.LanguageId);
        }

        public virtual string GetValue()
        {
            return this.Value;
        }
        public virtual void SetValue(string value)
        {
            this.Value = value;
        }

        public virtual void Remove()
        {
            new DbEntities().UserOption_Remove(this.OptionId, this.UserId, this.LanguageId);
        }

        protected static UserOptions Get(int customerId, ParameterType option, int? languageid)
        {
            var r = new DbEntities().UserOption_Get((int)option, customerId, languageid);
            return (UserOptions)r.FirstOrDefault();
        }

    }
    public partial class UserOption_Get_Result
    {
        public static explicit operator UserOptions(UserOption_Get_Result res)
        {
            if (res == null)
                return null;
            var uo= new UserOptions()
            {
                pk = res.pk,
                Created = res.Created,
                LanguageId = res.LanguageId,
                OptionId = res.OptionId,
                UserId = res.UserId
            };
            uo.SetValue(res.Value);
            return uo;
        }
    }
}
