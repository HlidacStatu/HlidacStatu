using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HlidacStatu.Service.WebShots
{
    public class InfinitiveTask
    {
        public static async Task Run(Action<string> action, string tag, TimeSpan period, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(period, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                    action(tag);
            }
        }

        public static void RunSync(Action<string> action, string tag, TimeSpan period, CancellationToken cancellationToken)
        {
            try
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    if (!cancellationToken.IsCancellationRequested)
                        action(tag);
                    System.Threading.Thread.Sleep(period);
                }

            }
            catch (Exception e)
            {
                Program.logger.Error("RunSync got exception ", e);
                return;

                //throw;
            }

        }
    }
}
