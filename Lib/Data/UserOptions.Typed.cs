using System;
using System.Linq;

namespace HlidacStatu.Lib.Data
{

    public abstract class UserOptions<T>
        : UserOptions
    {

        public UserOptions()
        { }

        public UserOptions(ParameterType option, int? languageId = null)
            :base(option,languageId)
        {
        }

        public UserOptions(AspNetUser user, ParameterType option, int? languageId = null)
        {
            var uo = Get(user.Id, option, languageId);
            this.LanguageId = languageId;
            this.UserId = user.Id;
            this.OptionId = (int)option;
            if (uo == null)
                this.Value = null;
            else
            this.Value = uo.GetValue();
        }

        public virtual T GetValue()
        {
            return DeserializeFromString(base.GetValue());
        }
        public virtual void SetValue(T value)
        {
            base.SetValue(SerializeToString(value));
        }

        protected abstract string SerializeToString(T value);
        protected abstract T DeserializeFromString(string value);

        public virtual void Remove()
        {
            new DbEntities().UserOption_Remove(this.OptionId, this.UserId, this.LanguageId);
        }

        //public virtual static UserOptions<T> Create(ParameterType option, T value, AspNetUser customer = null)
        //{
        //    UserOptions o = new UserOptions(customer, option);
        //    o.SetValue(value);
        //    o.Save();
        //    return o;
        //}




    }
}
