using System;
using System.Collections.Generic;

namespace HlidacStatu.Plugin.TransparetniUcty
{
    public class AutoParser : BaseTransparentniUcetParser
    {
        public override string Name { get { return "AutoParser"; } }

        public AutoParser(IBankovniUcet ucet) : base(ucet)
        {
        }

		private Dictionary<String, Func<IBankovniUcet, BaseTransparentniUcetParser>> BanksFactories = new Dictionary<string, Func<IBankovniUcet, BaseTransparentniUcetParser>>
		{
			{ "www.fio.cz", ucet => new Fio(ucet) },
			{ "ib.fio.cz", ucet => new Fio(ucet) },
			{ "www.kb.cz", ucet => new KB(ucet) },
			{ "www.csas.cz", ucet => new CS(ucet) },
			{ "www.rb.cz", ucet => new RB(ucet) },
			{ "www.csob.cz", ucet => new CSOB(ucet) },
			{ "www.moneta.cz", ucet => new Moneta(ucet) },
			{ "transparentniucty.moneta.cz", ucet => new Moneta(ucet) },
		};


		protected override IEnumerable<IBankovniPolozka> DoParse(DateTime? fromDate = default(DateTime?), DateTime? toDate = default(DateTime?))
        {
			var domain = Ucet.Url?.ToLower()?.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[1] ?? "";
			BanksFactories.TryGetValue(domain, out var factory);

			if (factory != null)
			{
				return factory(Ucet).GetPolozky(fromDate, toDate);
			}
			else
			{
				TULogger.Fatal("Not parser for " + Ucet.Url, new NotImplementedException());
				return new SimpleBankovniPolozka[] { };
			}
        }
    }
}
