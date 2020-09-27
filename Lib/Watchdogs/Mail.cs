using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HlidacStatu.Lib.Data;
using static HlidacStatu.Lib.Data.WatchDog;

namespace HlidacStatu.Lib.Watchdogs
{
    public partial class Mail
    {
        public enum SendStatus
        {
            Sent,
            NoDataToSend,
            Disabled,
            SendingError
        }
        internal static SendStatus SendWatchdog(WatchDog watchdog,
            AspNetUser aspnetUser,
            bool force = false, string[] specificContacts = null,
            DateTime? fromSpecificDate = null, DateTime? toSpecificDate = null,
            string openingText = null,
            int maxDegreeOfParallelism = 20,
            Action<string> logOutputFunc = null,
            Action<Devmasters.Batch.ActionProgressData> progressOutputFunc = null
            )
        {
            return SendWatchdogsInOneEmail(new WatchDog[] { watchdog },
            aspnetUser,
            force, specificContacts,
            fromSpecificDate,toSpecificDate,
            openingText
            );
        }


            internal static SendStatus SendWatchdogsInOneEmail(IEnumerable<WatchDog> watchdogs,
            AspNetUser aspnetUser,
            bool force = false, string[] specificContacts = null,
            DateTime? fromSpecificDate = null, DateTime? toSpecificDate = null,
            string openingText = null
            )
        {
            bool saveWatchdogStatus =
                force == false
                && fromSpecificDate.HasValue == false
                && toSpecificDate.HasValue == false;


            WatchDog[] userWatchdogs = watchdogs.ToArray();

            AspNetUser user = aspnetUser;

            if (user == null)
            {
                foreach (var wdtmp in userWatchdogs)
                {
                    if (wdtmp != null && wdtmp.StatusId != 0)
                    {
                        wdtmp.StatusId = 0;
                        wdtmp.Save();
                    }
                }
                return SendStatus.Disabled;
            }
            if (user.EmailConfirmed == false)
            {
                bool repeated = false;
                foreach (var wdtmp in userWatchdogs)
                {
                    if (wdtmp != null && wdtmp.StatusId > 0)
                    {
                        wdtmp.DisableWDBySystem(DisabledBySystemReasons.NoConfirmedEmail, repeated);
                        repeated = true;
                    }
                }
                return SendStatus.Disabled;
            } //user.EmailConfirmed == false
            string emailContact = user.Email;

            //process wds

            List<RenderedContent> parts = new List<RenderedContent>();
            foreach (var wd1 in userWatchdogs)
            {
                if ((force || Tools.ReadyToRun(wd1.Period, wd1.LastSearched, DateTime.Now)) == false)
                    continue;


                //specific Watchdog
                List<IWatchdogProcessor> wdProcessorsForWD1 = wd1.GetWatchDogProcessors();

                DateTime? fromDate = fromSpecificDate;
                DateTime? toDate = toSpecificDate;
                if (fromDate.HasValue == false && wd1.LatestRec.HasValue)
                    fromDate = new DateTime(wd1.LatestRec.Value.Ticks, DateTimeKind.Utc);
                if (fromDate.HasValue == false) //because of first search (=> no .LastSearched)
                    fromDate = DateTime.Now.Date.AddMonths(-1); //from previous month


                if (toDate.HasValue == false)
                    toDate = Tools.RoundWatchdogTime(wd1.Period, DateTime.Now);

                List<RenderedContent> wdParts = new List<RenderedContent>();
                foreach (var wdp in wdProcessorsForWD1)
                {
                    try
                    {

                        var results = wdp.GetResults(fromDate, toDate, 30);
                        if (results.Total > 0)
                        {
                            RenderedContent rres = wdp.RenderResults(results, 5);
                            wdParts.Add(Template.DataContent(results.Total, rres));
                            wdParts.Add(Template.Margin(50));

                        }
                    }
                    catch (Exception ex)
                    {
                        Util.Consts.Logger.Error("SingleEmailPerUserProcessor GetResults/RenderResults error", ex);
                    }
                }
                if (wdParts.Count() > 0)
                {
                    //add watchdog header
                    parts.Add(Template.TopHeader(wd1.Name, Util.RenderData.GetIntervalString(fromDate.Value, toDate.Value)));
                    parts.AddRange(wdParts);


                }

                if (saveWatchdogStatus)
                {
                    wd1.LatestRec = toDate.Value;
                    wd1.LastSearched = toDate.Value;
                    wd1.Save();
                }
            } //foreach wds

            if (parts.Count > 0)
            {
                //send it

                if (!string.IsNullOrEmpty(openingText))
                    parts.Insert(0, Template.Paragraph(openingText));

                if (DateTime.Now < new DateTime(2020, 4, 2))
                {
                    parts.Insert(0, Template.Margin());
                    parts.Insert(0, Template.Paragraph(
                    "<b style='color:red'>NOVINKA!</b> "
                    + "Pokud máte na Hlídači státu nastaveno více hlídačů nových informací, nebudeme Vám je už posílat v jednotlivých mailech. "
                    + "Místo toho obdržíte jeden souhrnný, ve kterém najdete vše najednou!  "
                    + "Pokud vám vyhovuje každý hlídač v samostatném mailu, snadno to změníte přes odkaz v patičce."
                    ,
                    "NOVINKA! "
                    + "Pokud máte na Hlídači státu nastaveno více hlídačů nových informací, nebudeme Vám je už posílat v jednotlivých mailech. "
                    + "Místo toho obdržíte jeden souhrnný, ve kterém najdete vše najednou!  "
                    + "Pokud vám vyhovuje každý hlídač v samostatném mailu, snadno to změníte přes odkaz v patičce."
                    ));
                }
                else
                {
                    parts.Insert(0, Template.Margin());
                    parts.Insert(0, Template.Paragraph(
                    "Seznam nalezených nových informací, které vás zajímají, pěkně pohromadě v jednom souhrnném mailu."
                    ,
                    "Seznam nalezených nových informací, které vás zajímají, pěkně pohromadě v jednom souhrnném mailu."
                    ));
                }
                parts.Insert(0, Template.Margin());

                var content = RenderedContent.Merge(parts);

                content.ContentHtml = Template.EmailBodyTemplateHtml
                    .Replace("#BODY#", content.ContentHtml)
                    .Replace("#FOOTERMSG#", Template.DefaultEmailFooterHtml)
                    ;
                content.ContentText = null;
                //Template.EmailBodyTemplateText
                //.Replace("#BODY#", content.ContentText)
                //.Replace("#FOOTERMSG#", Template.DefaultEmailFooterText);

                bool sent = false;
                if (specificContacts != null && specificContacts.Length > 0)
                {
                    foreach (var email in specificContacts)
                    {
                        Email.SendEmail(email, $"({DateTime.Now.ToShortDateString()}) Nové informace nalezené na Hlídači státu", content, fromEmail: "hlidac@hlidacstatu.cz");
                        return SendStatus.Sent;
                    }
                }
                else
                {
                    sent = Email.SendEmail(emailContact, $"({DateTime.Now.ToShortDateString()}) Nové informace nalezené na Hlídači státu", content, fromEmail: "hlidac@hlidacstatu.cz");
                }
                if (sent)
                {
                    if (saveWatchdogStatus)
                    {
                        DateTime dt = DateTime.Now;
                        foreach (var wd in userWatchdogs)
                        {
                            wd.LastSent = dt;
                            wd.Save();
                        }
                    }
                    return SendStatus.Sent;
                }
                else
                    return SendStatus.SendingError;

            }
            else
                return SendStatus.NoDataToSend;
        }
    }
}
