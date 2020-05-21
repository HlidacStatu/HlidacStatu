using HlidacStatu.Q.Messages;
using HlidacStatu.Q.Subscriber;
using System;
using System.Threading.Tasks;

namespace HlidacStatu.Q.ClassificationRepair
{
    public class ProcessClassificationFeedback : IMessageHandlerAsync<ClassificationFeedback>
    {
        public async Task HandleAsync(ClassificationFeedback message)
        {
            Console.WriteLine($"Message with id: {message.IdSmlouvy} is being proceesed. Prop cat = {message.ProposedCategories}; sender = {message.FeedbackEmail}");
            await Task.Delay(30000);
            Console.WriteLine($"Message with id: {message.IdSmlouvy} Finished processing");
        }
    }
}