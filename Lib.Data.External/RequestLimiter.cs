using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Data.External
{
    public abstract class RequestLimiter<TResult, TParameters>
    {
        DateTime lastReq = DateTime.Now.AddYears(-1);
        object lockObj = new object();

        public int MinIntervalBetweenRequests { get; private set; }
        public RequestLimiter(int minIntervalBetweenRequestsInMs)
        {
            this.MinIntervalBetweenRequests = minIntervalBetweenRequestsInMs;
        }


        public TResult Request(TParameters data)
        {
            lock (lockObj)
            {
                DateTime now = DateTime.Now;
                long diffInMs = Convert.ToInt64((now - lastReq).TotalMilliseconds);
                if (diffInMs < MinIntervalBetweenRequests)
                {
                    System.Threading.Thread.Sleep((int)((MinIntervalBetweenRequests - diffInMs) * 1.1 + 10));
                }
                lastReq = DateTime.Now;
                return DoRequest(data);

            }
        }
        protected abstract TResult DoRequest(TParameters data);


    }
}
