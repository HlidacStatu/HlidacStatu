using HlidacStatu.Q.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HlidacStatu.Q.ClassificationRepair
{
    public class ProcessMessage : IMessageHandler<ClassificationFeedback>
    {
        public void Handle(ClassificationFeedback message)
        {
            Console.WriteLine($"Message with id: {message.IdSmlouvy} is being proceesed. Prop cat = {message.ProposedCategories}; sender = {message.FeedbackEmail}");
        }
    }
}
