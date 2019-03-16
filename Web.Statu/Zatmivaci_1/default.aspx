<%@ Page Language="C#" AutoEventWireup="true" %>

<%@ OutputCache Duration="60" VaryByParam="none" %>

<script runat="server">
    static System.Text.StringBuilder tmp = new StringBuilder();
    static int[] poslanciCSSD = new int[] {
            467, 6002, 6136, 6182, 5447, 5342, 354, 5456, 5911,  5462, 6194, 5918, 5921, 6113, 5402, 6206,
        68, 6212, 5433, 5279, 5269, 5487, 6232, 5490, 5960, 5965, 5999, 6238, 6284,
        422, 5979, 5866, 5272, 6160, 252, 6157, 5989
    };
    int[] poslanciKSCM = new int[] {
        5768, 6149,  6183, 5307,
        6188, 5261, 303, 305, 4778,
        5461, 5920, 5934, 5313, 309,
        6279, 6220, 5290, 5938, 5948,
        6223,  5322, 6228, 6233,
        5962, 5502, 5282, 5506, 6169,
        6164, 5268, 6159
    };
    int[] poslanciODS = new int[] {
            4,
        5903, 6208,
        6175,5520,
        5844
    };
    int[] poslanciKDU = new int[] {
        6178,
        6142, 6205,
        6210, 6217,
    };
    int[] poslanciOstatni = new int[] {
            6191
    };

    int[] slibili = new int[] {
        //237,
        467, 6002, 6182, 5447, 5342, 354, 5462, 6194, 5921, 6113, 5402, 6212, 5433, 5269, 5487, 6232, 5490, 5960, 5965, 5999, 6238, 6284, 422, 5979, 5899, 5272, 252, 6157, 5989,

        5768, 6188, 5261, 305, 4778, 5461, 5920, 5934, 5313, 6279, 5290, 5938, 6223, 5322,6233, 5962, 5502, 5282, 5506, 6169, 6164,

        //6074, 
        6208, 

        //6138, 
        6178, 6142, 6205, 6210,

        6191
    };

    static string poslanci = @"237|Mgr.|Sobotka|Bohuslav| |23.10.1971|M|08.12.1998||
467|Bc.|Adámek|František| |28.02.1955|M|||
6002|MUDr.|Běhounek|Jiří| |13.05.1952|M|||
6136|Mgr.|Benešová|Marie| |17.04.1948|Ž|||
6182| |Birke|Jan| |01.06.1969|M|||
5447|Mgr.|Bohdalová|Vlasta| |18.09.1957|Ž|||
5342|PhDr.|Böhnisch|Robin| |23.11.1976|M|||
354| |Černý|Karel| |12.01.1965|M|07.12.1998||
5456| |Dolejš|Richard| |13.08.1970|M|||
5911| |Foldyna|Jaroslav| |26.06.1960|M|||
5462| |Hamáček|Jan| |04.11.1978|M|||
6194|MUDr.|Havíř|Pavel| |21.02.1955|M|||
5918|MUDr.|Holík|Pavel| |23.05.1957|M|||
5921|Mgr.|Huml|Stanislav| |30.07.1955|M|||
6113|Mgr.|Jakubčík|Igor| |12.05.1967|M|||
5402|Mgr.|Jandák|Vítězslav| |03.08.1947|M|||
6206|Bc.|Kailová|Zuzana| |21.07.1974|Ž|||
68|Ing.|Klučka|Václav| |20.07.1953|M|||
6212|Mgr.|Kořenek|Petr| |23.08.1966|M|||
5433|MUDr.|Koskuba|Jiří| |14.12.1955|M|||
5279|MUDr.|Krákora|Jaroslav| |25.10.1955|M|||
5269|Ing.|Mládek|Jan|CSc.|01.06.1960|M|||
5487|Ing.|Petrů|Jiří| |17.08.1956|M|||
6232|JUDr. Ing.|Pleticha|Lukáš| |18.10.1974|M|||
5490| |Ploc|Pavel| |15.06.1964|M|||
5960|Ing.|Rykala|Adam| |03.09.1986|M|||
5965|Mgr.|Sklenák|Roman| |23.07.1970|M|||
5999| |Strnadlová|Miroslava| |20.11.1954|Ž|||
6238|JUDr.|Stupčuk|Štěpán| |09.03.1976|M|||
6284|Bc.|Syblík|Zdeněk| |07.05.1956|M|||
422|Ing.|Urban|Milan| |15.10.1958|M|09.12.1998||
5979|Mgr.|Váhalová|Dana| |28.05.1966|Ž|||
5866| |Velebný|Ladislav| |11.04.1957|M|||
5272|Ing.|Votava|Václav| |18.06.1956|M|||
6160|Ing.|Wernerová|Markéta| |02.08.1983|Ž|||
252|PhDr.|Zaorálek|Lubomír| |06.09.1956|M|09.12.1998||
6157| |Zavadil|Jaroslav| |12.04.1944|M|||
5989|Ing.|Zemek|Václav| |13.04.1974|M|||
5768|MUDr.|Adam|Vojtěch| |12.09.1950|M|||
6149|Ing.|Aulická Jírovcová|Hana| |07.04.1981|Ž|||
6183|Mgr.|Borka|Jaroslav| |18.10.1952|M|||
5307|RSDr.|Černý|Alexander| |21.05.1953|M|||
6188|Ing.|Číp|René| |10.12.1974|M|||
5261|Ing.|Dolejš|Jiří| |24.01.1961|M|||
303|JUDr.|Filip|Vojtěch| |13.01.1955|M|07.12.1998||
305|doc. PhDr.|Grebeníček|Miroslav|CSc.|21.03.1947|M|07.12.1998||
4778|JUDr.|Grospič|Stanislav| |25.07.1964|M|||
5461|PaedDr.|Halíková|Milada| |06.01.1950|Ž|||
5920| |Hubáčková|Gabriela| |28.01.1975|Ž|||
5934|Mgr.|Klán|Jan| |29.11.1982|M|||
5313|RNDr.|Koníček|Vladimír| |23.02.1964|M|||
309|Ing.|Kováčik|Pavel| |02.07.1955|M|07.12.1998||
6279|Ing.|Luzar|Leo| |06.12.1964|M|||
6220|Bc.|Mackovík|Stanislav| |11.03.1967|M|||
5290|Mgr.|Marková|Soňa| |25.02.1963|Ž|||
5938|Ing.|Matušovská|Květa| |04.06.1984|Ž|||
5948|RSDr.|Nekl|Josef| |28.05.1953|M|||
6223| |Nohavová|Alena| |07.04.1960|Ž|||
5322|RSDr.|Opálka|Miroslav| |09.10.1952|M|||
6228|Ing.|Pěnčíková|Marie| |31.12.1979|Ž|||
6233|Mgr.|Pojezný|Ivo| |21.05.1961|M|||
5962|Mgr.|Semelová|Marta| |01.03.1960|Ž|||
5502|Mgr.|Snopek|Václav|CSc.|20.08.1951|M|||
5282|Ing.|Šenfeld|Josef| |13.11.1961|M|||
5506|Ing.|Šidlo|Karel| |01.11.1957|M|||
6169|PhDr. Ing.|Valenta|Jiří| |28.12.1965|M|||
6164|Ing.|Vondrášek|Josef| |25.07.1950|M|||
5268|Ing.|Vostrá|Miloslava| |26.05.1965|Ž|||
6159| |Zahradníček|Josef| |18.03.1957|M|||
6074|Prof. PhDr.|Fiala|Petr|Ph.D., LL.M.|01.09.1964|M|||
4| |Benda|Marek| |10.11.1968|M|07.12.1998||
5903|Mgr.|Černochová|Jana| |26.10.1973|Ž|||
6208|prof. Ing.|Karamazov|Simeon|Dr.|19.12.1963|M|||
6175| |Novotný|Martin| |21.01.1972|M|||
5520|Ing.|Vilímec|Vladislav| |05.07.1963|M|||
5844|RNDr.|Zahradník|Jan| |23.03.1949|M|||
6138|MVDr.|Bělobrádek|Pavel|Ph.D., MPA|25.12.1976|M|||
6178|Ing.|Bartošek|Jan| |10.11.1971|M|||
6142|Mgr.|Herman|Daniel| |28.04.1963|M|||
6205|Ing.|Jurečka|Marian| |15.03.1981|M|||
6210|Ing. arch.|Klaška|Jaroslav| |28.06.1962|M|||
6217|Ing.|Kudela|Petr| |10.03.1969|M|||
6191|Ing.|Fiedler|Karel| |31.12.1961|M|||
";


    static string[] p = poslanci.Split(new string[] { "\n" }, StringSplitOptions.None);


    public string Render(int id, string strana, string popis = "")
    {

        string jmenoLine = p.Where(m => m.StartsWith(id.ToString() + "|")).First();
        string[] jmenoparts = jmenoLine.Split('|');
        string jmeno = string.Format("{0} {1} {2} {3}", jmenoparts[1], jmenoparts[3], jmenoparts[2], jmenoparts[4]);
        bool slib = slibili.Contains(id);
        bool zena = jmenoparts[2].EndsWith("ová");

        string s = "<div class='col-xs-12 col-sm-6 col-md-4 text-center center-block' style='border-top:1px solid #ddd'>";
        s += "<h3><a href='http://www.psp.cz/sqw/detail.sqw?id=" + id + "'>" + jmeno + "(" + strana + ")</a></h3>";
        s += "<p class='text-center center-block'><a href='http://www.psp.cz/sqw/detail.sqw?id=" + id + "'><img src='http://www.psp.cz/eknih/cdrom/2013ps/eknih/2013ps/poslanci/i" + id.ToString() + ".jpg' height='180' /></a>";
        s += "<br/>" + popis + "<p>";

        if (slib)
        {
            if (zena)
            {
                s += "<p class='text-danger'>Podvodnice a lhářka</p>";
                s += "<p class=''>Tato poslankyně před volbami slíbila podporovat a schválit plnohodnotný Registr smluv. Tímto hlasování podvedla své voliče a porušila svůj předvolební slib.</p>";
            }
            else
            {
                s += "<p class='text-danger'>Podvodník a lhář</p>";
                s += "<p class=''>Tento poslanec před volbami slíbil podporovat a schválit zcela plnohodnotný Registr smluv. Tímto hlasování podvedl své voliče a porušil svůj předvolební slib.</p>";
            }
        }

        s += "</div>";

        tmp.Append(jmenoLine);

        return s;
    }

</script>



<!DOCTYPE html>
<html>
<head>
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0">

    <head runat="server">
        <title>Poslanci parlamentu ČR, kteří odmítají kontrolu veřejných prostředků občany</title>
    </head>

    <meta name="author" content="Michal Bláha" />
    <meta name="keywords" content="Registr smluv, Hlídač smluv, politici, smlouvy státu, hajzlici, Hajzlíci, zatmívači">
    <link href="https://fonts.googleapis.com/css?family=Source+Sans+Pro:300,400,600,700&amp;subset=latin-ext" rel="stylesheet">




    <link href="https://www.hlidacstatu.cz/Content/bootstrap.css" rel="stylesheet" />
    <link href="https://www.hlidacstatu.cz/Content/site.css" rel="stylesheet" />
    <script src="https://www.hlidacstatu.cz/Scripts/modernizr-2.8.3.js"></script>
    <script src="https://www.hlidacstatu.cz/Scripts/jquery-1.11.3.min.js"></script>
    <script src="https://www.hlidacstatu.cz/Scripts/bootstrap.js"></script>
    <script src="https://www.hlidacstatu.cz/Scripts/respond.js"></script>
    <script src="https://www.hlidacstatu.cz/Scripts/site.js"></script>

    <link rel="stylesheet" href="https://www.hlidacstatu.cz/content/social-share-kit.css" type="text/css">
    <script type="text/javascript" src="https://www.hlidacstatu.cz/scripts/social-share-kit.min.js"></script>



    <script>
        (function (i, s, o, g, r, a, m) {
            i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                (i[r].q = i[r].q || []).push(arguments)
            }, i[r].l = 1 * new Date(); a = s.createElement(o),
            m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
        })(window, document, 'script', 'https://www.google-analytics.com/analytics.js', 'ga');

        ga('create', 'UA-154075-23', 'auto');
        ga('send', 'pageview');


        var trackOutLink = function (obj, source) {
            var url = obj.getAttribute("href");
            ga('send', 'event', 'outbound', source, url, {
                'transport': 'beacon'
            });
            return true;
        }

    </script>
</head>
<body class="">

    <div id="fb-root"></div>
    <script>
        (function (d, s, id) {
            var js, fjs = d.getElementsByTagName(s)[0];
            if (d.getElementById(id)) return;
            js = d.createElement(s); js.id = id;
            js.src = "//connect.facebook.net/cs_CZ/sdk.js#xfbml=1&version=v2.8";
            fjs.parentNode.insertBefore(js, fjs);
        }(document, 'script', 'facebook-jssdk'));</script>




    <header class="clearfix">
    </header>
    <div class="title">

        <div class="container">
            <div class="col-md-10 col-md-offset-1">
                <h1>Poslanci odmítající veřejnou kontrolu utrácení státu</h1>
                <div style="padding-top: 20px;">
                    <p style="font-size: 16px;">
                        Tito poslanci dne 22. února 2017 podpořili řadu plošných výjimek ze zveřejňování smluv státu. (viz <a target="_blank" href="http://www.psp.cz/sqw/hlasy.sqw?g=65423&l=cz">podrobné hlasování</a>)
                    </p>
                    <p style="font-size: 16px;">
                        Řada poslanců tak porušila svůj slib, který dali veřejnosti před čtyřmi lety - podporovat zveřejňování smluv státu na internetu. Poslanci ČSSD, KSČM, a část poslanců ODS a KDU-ČSL dnes podpořili nejrozsáhlejší výjimky ze zákona o registru smluv a řadu dalších drobnějších.
                    </p>
                    <p style="font-size: 16px;">
                        Vyprázdnili tak čtvrtinu obsahu (smlouvy cca za 148 miliard korun ročně) jediného zákona této sněmovny, který má zabraňovat plýtvání veřejnými prostředky. Přijali plošné výjimky pro státní a městské firmy, které ročně potajmu hospodaří se stovkami veřejných miliard - jde například o všechny dopravní podniky, městské holdingy, Českou poštu, České dráhy či Lesy ČR.
                    </p>
                </div>
            </div>
        </div>
    </div>
    <div class="body">
        <div class="container">



        </div>
    </div>

    <footer>
        <div class="container">
            <p>2017 Michal Bláha. </p>
        </div>
    </footer>

    <!--
        <% Response.Write(tmp.ToString()); %>
         -->
</body>
</html>
