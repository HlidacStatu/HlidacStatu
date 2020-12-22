using System;
using System.Collections.Generic;
using System.Text;

namespace HlidacStatu.Q.Simple.Tasks
{
    public class Voice2Text
    {
        public const string QName = "voice2text";
        public const string QName_failed = QName + "_failed";
        public const string QName_done = QName + "_done";

        public static int[] Priorities = new int[] { 2, 1, 0 };

        public static string QName_priority(int priority)
        {
            if (priority < 0)
                priority = 0;
            else if (priority > 2)
                priority = 2;

            return priority == 0 ? QName : QName+"_" + priority.ToString();
        }

        public string dataset { get; set; }
        public string itemid { get; set; }
    }

}
