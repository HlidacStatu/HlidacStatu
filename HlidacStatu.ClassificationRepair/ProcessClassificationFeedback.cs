using HlidacStatu.Q.Messages;
using HlidacStatu.Q.Subscriber;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HlidacStatu.ClassificationRepair
{
    public class ProcessClassificationFeedback : IMessageHandlerAsync<ClassificationFeedback>
    {
        private readonly ILogger _logger;
        private readonly IStemmerService _stemmer;
        private readonly IHlidacService _hlidac;
        private readonly IEmailService _email;

        public ProcessClassificationFeedback(ILogger<ProcessClassificationFeedback> logger,
                                             IStemmerService stemmerService,
                                             IHlidacService hlidacService,
                                             IEmailService emailService)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _stemmer = stemmerService ?? throw new ArgumentNullException(nameof(stemmerService));
            _hlidac = hlidacService ?? throw new ArgumentNullException(nameof(hlidacService));
            _email = emailService ?? throw new ArgumentNullException(nameof(emailService));
        }

        public async Task HandleAsync(ClassificationFeedback message)
        {
            try
            {
                _logger.LogInformation($"New message with idSmlouvy={message.IdSmlouvy} accepted.");

                IEnumerable<string> textySmlouvy = await _hlidac.GetTextSmlouvy(message.IdSmlouvy);

                string textSmlouvy = string.Join('\n', textySmlouvy);

                //var explainTask = _stemmer.ExplainCategories(textSmlouvy); todo: čekám až kuba dodělá endpoint
                var documentNgramTask = _stemmer.Stem(textSmlouvy);
                var bullshitNgramsTask = _stemmer.GetBullshitStems();
                var allNgramsTask = _stemmer.GetAllStems();

                //await Task.WhenAll(explainTask, documentNgramTask, bullshitNgramsTask, allNgramsTask); todo: čekám až kuba dodělá endpoint
                await Task.WhenAll(documentNgramTask, bullshitNgramsTask, allNgramsTask);

                var missingNgrams = documentNgramTask.Result
                    .Except(bullshitNgramsTask.Result)
                    .Except(allNgramsTask.Result);

                // poslat mail
                await SendMail(message.FeedbackEmail, message.IdSmlouvy,
                    message.ProposedCategories, "",
                    missingNgrams);
                _logger.LogInformation($"Message with idSmlouvy={message.IdSmlouvy} processed.");
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex, $"Failed when processing idSmlouvy={message.IdSmlouvy}.");
                throw;
            }
        }

        private async Task SendMail(string feedbackMail, string idSmlouvy,
            string proposedCategories, string explainResult, IEnumerable<string> missingNgrams)
        {
            _logger.LogInformation($"Sending email.");
            string[] recipients = new string[]
            {
                //"michal@michalblaha.cz",
                "petr@hlidacstatu.cz",
                //"lenka@hlidacstatu.cz"
            };

            string subject = "Zprava z HlidacStatu.cz: Změna kategorie klasifikace";
            string body = $@"Návrh na opravu klasifikace od uživatele: {feedbackMail}: <br />
                idSmlouvy: {idSmlouvy} <br />
                navrhované kategorie: {proposedCategories} <br />
                <br />
                Explain: <br />
                {explainResult} <br />
                <br />
                Missing N-grams: <br />
                {string.Join("<br />", missingNgrams)} <br />
                <br />
                --- Konec zprávy ---";

            await _email.SendEmailAsync(recipients, subject, body, feedbackMail);
            _logger.LogInformation($"Email sent.");
        }
    }
}