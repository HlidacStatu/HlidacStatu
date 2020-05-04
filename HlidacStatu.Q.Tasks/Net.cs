using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace HlidacStatu.Q.Tasks
{
    public class Net
    {

        public static void PublishIntoQ<T>(T msg, string topic = null)
            where T : BaseTask
        {
            using (var bus = RabbitHutch.CreateBus("host=10.10.100.145;username=code;password=circa3004"))
            {
                if (!string.IsNullOrWhiteSpace(topic))
                    bus.Publish<T>(msg,topic);
                else
                    bus.Publish<T>(msg);
            }
        }

    }
}
