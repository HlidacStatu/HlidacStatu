using System;
using System.Collections.Generic;

namespace HlidacStatu.Lib.Analysis.KorupcniRiziko
{
    public partial class KIndexData
    {
        public class COFOG
        {
            public static COFOG Parse(string cofog)
            {
                if (string.IsNullOrEmpty(cofog))
                    return null;
                if (cofog == "0")
                    return null;

                if (cofog.Length == 3)
                    return new COFOG()
                    {
                        Oddil = Convert.ToInt32(cofog[0]),
                        Skupina = Convert.ToInt32(cofog[1]),
                        Trida = Convert.ToInt32(cofog[2]),
                    };

                if (cofog.Length == 4)
                    return new COFOG()
                    {
                        Oddil = Convert.ToInt32(cofog[0] + cofog[1]),
                        Skupina = Convert.ToInt32(cofog[2]),
                        Trida = Convert.ToInt32(cofog[3]),
                    };

                throw new ArgumentException("invalid cofog:" + cofog);
            }

            public int Oddil { get; set; }
            public int Skupina { get; set; } = 0;
            public int Trida { get; set; } = 0;


            public string Popis()
            {
                return popisy[this.ToString()];
            }



            public override string ToString()
            {
                var ret = $"{Oddil:0#}";
                if (Skupina > 0)
                    ret += $".{Skupina}";
                if (Trida > 0)
                    ret += $".{Trida}";
                return ret;
            }


            private static Dictionary<string, string> popisy = new Dictionary<string, string>() {
                {"01","VŠEOBECNÉ VEŘEJNÉ SLUŽBY"},
                {"01.1","Zákonodárné, výkonné a další normotvorné orgány, záležitosti finanční, rozpočtové, daňové, zahraniční kromě zahraniční pomoci"},
                {"01.1.1","Zákonodárné, výkonné a další normotvorné orgány"},
                {"01.1.2","Finanční, rozpočtové a daňové záležitosti"},
                {"01.1.3","Zahraniční záležitosti kromě zahraniční pomoci"},
                {"01.2","Zahraniční ekonomická pomoc"},
                {"01.2.1","Ekonomická pomoc adresným příjemcům"},
                {"01.2.2","Ekonomická pomoc směrovaná přes mezinárodní organizace"},
                {"01.3","Všeobecné služby"},
                {"01.3.1","Všeobecné personální služby"},
                {"01.3.2","Rámcové plánovací a statistické služby"},
                {"01.3.3","Ostatní všeobecné služby"},
                {"01.4","Základní výzkum"},
                {"01.4.0","Základní výzkum"},
                {"01.5","Aplikovaný výzkum a vývoj v oblasti všeobecných veřejných služeb"},
                {"01.5.0","Aplikovaný výzkum a vývoj v oblasti všeobecných veřejných služeb"},
                {"01.6 ","Všeobecné veřejné služby jinde neuvedené"},
                {"01.6.0","Všeobecné veřejné služby jinde neuvedené"},
                {"01.7","Transakce v oblasti veřejného (státního) dluhu"},
                {"01.7.0","Transakce v oblasti veřejného (státního) dluhu"},
                {"01.8","Transfery obecného charakteru mezi různými úrovněmi státní a veřejné správy"},
                {"01.8.0","Transfery obecného charakteru mezi různými úrovněmi státní a veřejné správy"},
                {"02","OBRANA"},
                {"02.1","Vojenská obrana (správa a provoz)"},
                {"02.1.0","Vojenská obrana (správa a provoz)"},
                {"02.2","Civilní ochrana"},
                {"02.2.0","Civilní ochrana"},
                {"02.3","Zahraniční vojenská pomoc"},
                {"02.3.0","Zahraniční vojenská pomoc"},
                {"02.4","Aplikovaný výzkum a vývoj v oblasti obrany"},
                {"02.4.0","Aplikovaný výzkum a vývoj v oblasti obrany"},
                {"02.5","Obrana jinde neuvedená"},
                {"02.5.0","Obrana jinde neuvedená"},
                {"03","VEŘEJNÝ POŘÁDEK A BEZPEČNOST"},
                {"03.1","Policejní ochrana"},
                {"03.1.0","Policejní ochrana"},
                {"03.2","Požární ochrana"},
                {"03.2.0","Požární ochrana"},
                {"03.3","Soudy a státní zastupitelství"},
                {"03.3.0","Soudy a státní zastupitelství"},
                {"03.4","Vězeňská správa a vězeňský provoz"},
                {"03.4.0","Vězeňská správa a vězeňský provoz"},
                {"03.5","Aplikovaný výzkum a vývoj v oblasti veřejného pořádku a bezpečnosti"},
                {"03.5.0","Aplikovaný výzkum a vývoj v oblasti veřejného pořádku a bezpečnosti"},
                {"03.6","Veřejný pořádek a bezpečnost jinde neuvedená"},
                {"03.6.0","Veřejný pořádek a bezpečnost jinde neuvedená"},
                {"04","EKONOMICKÉ ZÁLEŽITOSTI"},
                {"04.1","Všeobecné ekonomické, komerční a pracovní záležitosti"},
                {"04.1.1","Všeobecné ekonomické a komerční záležitosti"},
                {"04.1.2","Všeobecné pracovní záležitosti"},
                {"04.2","Zemědělství, lesnictví, rybářství a  myslivost"},
                {"04.2.1","Zemědělství"},
                {"04.2.2","Lesnictví"},
                {"04.2.3","Rybářství a myslivost"},
                {"04.3","Paliva a energetika"},
                {"04.3.1","Uhlí a ostatní tuhá paliva"},
                {"04.3.2","Ropa a zemní plyn"},
                {"04.3.3","Jaderné palivo a ochrana před ionizujícím zářením"},
                {"04.3.4","Ostatní paliva"},
                {"04.3.5","Elektrická energie"},
                {"04.3.6","Energie jiná než elektrická"},
                {"04.4","Těžba a zpracovatelský průmysl"},
                {"04.4.1","Těžba nerostných surovin kromě paliv"},
                {"04.4.2","Průmyslová výroba"},
                {"04.4.3","Stavebnictví"},
                {"04.5","Doprava"},
                {"04.5.1","Silniční doprava"},
                {"04.5.2","Vodní doprava"},
                {"04.5.3","Drážní doprava"},
                {"04.5.4","Letecká doprava"},
                {"04.5.5","Potrubní a ostatní doprava"},
                {"04.6","Pošty a telekomunikace"},
                {"04.6.0","Pošty a telekomunikace"},
                {"04.7","Ekonomická oblast ostatní"},
                {"04.7.1","Velkoobchod a maloobchod včetně skladování"},
                {"04.7.2","Ubytování a stravování"},
                {"04.7.3","Turistický ruch"},
                {"04.7.4","Víceúčelové projekty rozvoje"},
                {"04.8","Aplikovaný výzkum a vývoj v ekonomické oblasti "},
                {"04.8.1","Aplikovaný výzkum a vývoj v oblasti všeobecných ekonomických, komerčních a pracovních záležitostí"},
                {"04.8.2","Aplikovaný výzkum a vývoj v oblasti zemědělství, lesnictví, rybářství a myslivosti"},
                {"04.8.3","Aplikovaný výzkum a vývoj v oblasti paliv a energetiky"},
                {"04.8.4","Aplikovaný výzkum a vývoj v oblasti těžby, průmyslové výroby a stavebnictví"},
                {"04.8.5 ","Aplikovaný výzkum a vývoj v oblasti dopravy"},
                {"04.8.6","Aplikovaný výzkum a vývoj v oblasti pošt a telekomunikací"},
                {"04.8.7","Aplikovaný výzkum a vývoj v oblasti obchodu, turistického ruchu, víceúčelových projektů"},
                {"04.9","Ekonomické záležitosti jinde neuvedené"},
                {"04.9.0","Ekonomické záležitosti jinde neuvedené"},
                {"05","OCHRANA ŽIVOTNÍHO PROSTŘEDÍ"},
                {"05.1","Odstraňování odpadů kromě odpadních vod"},
                {"05.1.0","Odstraňování odpadů kromě odpadních vod"},
                {"05.2","Odstraňování odpadních vod"},
                {"05.2.0","Odstraňování odpadních vod"},
                {"05.3","Kontrola a snižování znečištění"},
                {"05.3.0","Kontrola a snižování znečištění"},
                {"05.4","Ochrana přírody "},
                {"05.4.0","Ochrana přírody"},
                {"05.5","Aplikovaný výzkum a vývoj v oblasti ochrany životního prostředí"},
                {"05.5.0","Aplikovaný výzkum a vývoj v oblasti ochrany životního prostředí"},
                {"05.6","Ochrana životního prostředí jinde neuvedená"},
                {"05.6.0","Ochrana životního prostředí jinde neuvedená"},
                {"06","BYDLENÍ A SPOLEČENSKÁ INFRASTRUKTURA"},
                {"06.1","Rozvoj bydlení"},
                {"06.1.0","Rozvoj bydlení"},
                {"06.2","Územní rozvoj obecně"},
                {"06.2.0","Územní rozvoj obecně"},
                {"06.3","Zásobování vodou"},
                {"06.3.0","Zásobování vodou"},
                {"06.4","Pouliční osvětlení"},
                {"06.4.0","Pouliční osvětlení"},
                {"06.5","Aplikovaný výzkum a vývoj v oblasti rozvoje bydlení a společenské infrastruktury"},
                {"06.5.0","Aplikovaný výzkum a vývoj v oblasti rozvoje bydlení a společenské infrastruktury"},
                {"06.6","Rozvoj bydlení a společenské infrastruktury jinde neuvedený"},
                {"06.6.0","Rozvoj bydlení a společenské infrastruktury jinde neuvedený"},
                {"07","ZDRAVÍ"},
                {"07.1","Léčiva a zdravotnické prostředky"},
                {"07.1.1","Léčiva"},
                {"07.1.2","Ostatní zdravotnické výrobky"},
                {"07.1.3","Léčebné a protetické prostředky"},
                {"07.2","Ambulantní zdravotní péče"},
                {"07.2.1","Ambulantní lékařská péče všeobecná"},
                {"07.2.2","Ambulantní lékařská péče specializovaná (kromě stomatologické péče)"},
                {"07.2.3","Ambulantní stomatologická péče"},
                {"07.2.4","Ambulantní zdravotní péče ostatní"},
                {"07.3","Ústavní zdravotní péče"},
                {"07.3.1","Ústavní zdravotní péče všeobecná"},
                {"07.3.2","Ústavní zdravotní péče specializovaná"},
                {"07.3.3","Služby zdravotnických středisek a středisek péče v mateřství"},
                {"07.3.4","Služby sanatorií, kojeneckých ústavů, zotavoven a léčeben pro dlouhodobě nemocné apod."},
                {"07.4","Veřejné zdravotnické služby"},
                {"07.4.0","Veřejné zdravotnické služby"},
                {"07.5","Aplikovaný výzkum a vývoj v oblasti zdravotnictví"},
                {"07.5.0","Aplikovaný výzkum a vývoj v oblasti zdravotnictví"},
                {"07.6","Zdravotnické služby jinde neuvedené"},
                {"07.6.0","Zdravotnické služby jinde neuvedené"},
                {"08","REKREACE, KULTURA A NÁBOŽENSTVÍ"},
                {"08.1","Rekreační a sportovní služby"},
                {"08.1.0","Rekreační a sportovní služby"},
                {"08.2","Kulturní služby"},
                {"08.2.0","Kulturní služby"},
                {"08.3","Rozhlasové a televizní vysílání a vydavatelské služby"},
                {"08.3.0","Rozhlasové a televizní vysílání a vydavatelské služby"},
                {"08.4","Náboženské a ostatní společenské služby"},
                {"08.4.0","Náboženské a ostatní společenské služby"},
                {"08.5","Aplikovaný výzkum a vývoj v oblasti rekreace, kultury a náboženství"},
                {"08.5.0","Aplikovaný výzkum a vývoj v oblasti rekreace, kultury a náboženství"},
                {"08.6","Rekreační, kulturní a náboženské služby jinde neuvedené"},
                {"08.6.0","Rekreační, kulturní a náboženské služby jinde neuvedené"},
                {"09","VZDĚLÁVÁNÍ"},
                {"09.1","Preprimární a primární vzdělávání"},
                {"09.1.1","Preprimární vzdělávání"},
                {"09.1.2","Primární vzdělávání "},
                {"09.2","Sekundární vzdělávání"},
                {"09.2.1","Nižší sekundární vzdělávání"},
                {"09.2.2","Vyšší sekundární vzdělávání"},
                {"09.3","Postsekundární vzdělávání nižší než terciární"},
                {"09.3.0","Postsekundární vzdělávání nižší než terciární"},
                {"09.4","Terciární vzdělávání"},
                {"09.4.1","První stupeň terciárního vzdělávání"},
                {"09.4.2","Druhý stupeň terciárního vzdělávání"},
                {"09.5","Vzdělávání nedefinované podle úrovně"},
                {"09.5.0","Vzdělávání nedefinované podle úrovně"},
                {"09.6","Vedlejší služby ve vzdělávání"},
                {"09.6.0","Vedlejší služby ve vzdělávání"},
                {"09.7","Aplikovaný výzkum a vývoj v oblasti vzdělávání"},
                {"09.7.0","Aplikovaný výzkum a vývoj v oblasti vzdělávání"},
                {"09.8","Vzdělávání jinde neuvedené"},
                {"09.8.0","Vzdělávání jinde neuvedené"},
                {"10","SOCIÁLNÍ VĚCI"},
                {"10.1","Nemoc a invalidita"},
                {"10.1.1","Nemoc"},
                {"10.1.2","Invalidita"},
                {"10.2","Stáří"},
                {"10.2.0","Stáří"},
                {"10.3","Pozůstalí"},
                {"10.3.0","Pozůstalí"},
                {"10.4","Rodina a děti"},
                {"10.4.0","Rodina a děti"},
                {"10.5","Nezaměstnanost"},
                {"10.5.0","Nezaměstnanost"},
                {"10.6","Bydlení"},
                {"10.6.0","Bydlení"},
                {"10.7","Sociální pomoc jinde neuvedená"},
                {"10.7.0","Sociální pomoc jinde neuvedená"},
                {"10.8","Aplikovaný výzkum a vývoj v oblasti sociálních věcí"},
                {"10.8.0","Aplikovaný výzkum a vývoj v oblasti sociálních věcí"},
                {"10.9","Sociální věci jinde neuvedené"},
                {"10.9.0","Sociální věci jinde neuvedené"}
            };

        }

    }
}
