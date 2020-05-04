using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace HlidacStatu.Q.Tasks
{
    public class TaskHandler
    {

        public static Task Default<T>(T m, Func<T, bool> action)
            where T: BaseTask
        {
            return Task.Factory.StartNew(
                 () =>
                 {
                     Console.WriteLine($"start: {m.TaskID}");
                     HlidacStatu.Util.Consts.Logger.Debug($"Starting task {m.TaskID}");

                     var result = action(m);

                     HlidacStatu.Util.Consts.Logger.Debug($"Ending task {m.TaskID}");
                 })
                .ContinueWith(task =>
                {
                    Console.WriteLine($"Finish: {m.TaskID}");
                    if (task.IsCompleted && !task.IsFaulted)
                    {
                        HlidacStatu.Util.Consts.Logger.Debug($"Finishing OK task {m.TaskID}");
                    }
                    else
                    {
                        HlidacStatu.Util.Consts.Logger.Error($"Finishing task {m.TaskID} with error ", task.Exception);
                        // Don't catch this, it is caught further up the hierarchy and results in being sent to the default error queue
                        // on the broker
                        throw new EasyNetQException($"Task {m.TaskID}", task.Exception);
                    }
                }
                );
        }
    }
}
