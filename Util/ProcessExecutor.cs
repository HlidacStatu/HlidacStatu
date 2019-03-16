using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HlidacStatu.Util
{
    public class ProcessExecutor
    {
        public ProcessStartInfo ProcessInfo = null;
        string outputLogFile = string.Empty;
        string errorLogFile = string.Empty;
        int timeOut = 1;
//        System.Diagnostics.Process process;
        bool log = false;
//        bool finishedProcess = false;
        int exitCode = int.MinValue;

        StringBuilder sbOut = new StringBuilder();
        StringBuilder sbErr = new StringBuilder();

        
        public ProcessExecutor(ProcessStartInfo processInfo, int timeOutInSec)
            : this(processInfo, timeOutInSec, string.Empty, string.Empty, false)
        { }


        public ProcessExecutor(ProcessStartInfo processInfo, int timeOutInSec, string outputLogFile, string errorLogFile, bool logAll)
        {
            this.timeOut = timeOutInSec * 1000;
            this.ProcessInfo = processInfo;
            this.outputLogFile = outputLogFile;
            this.errorLogFile = errorLogFile;

            log = logAll;

            processInfo.CreateNoWindow = false;
            processInfo.RedirectStandardOutput = true;
            processInfo.RedirectStandardError = true;
            processInfo.UseShellExecute = false;

        }

        void  process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            sbErr.AppendLine(e.Data);
        }

        void  process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            sbOut.AppendLine(e.Data);
        }

        void process_Exited(object sender, EventArgs e)
        {
//            finishedProcess = true;
        }


        public string StandardOutput
        {
            get
            {
                //return process.StandardOutput.ReadToEnd();
                return sbOut.ToString();
            }
        }


        public string ErrorOutput
        {
            get
            {
                //return process.StandardOutput.ReadToEnd();
                return sbErr.ToString();
            }
        }

        public string PathWithArguments
        {
            get
            {
                return ProcessInfo.FileName + " " + ProcessInfo.Arguments;
            }
        }

        public int ExitCode
        {
            get
            {
                return exitCode;
            }
        }

        public void Start()
        {

            using (Process process = new Process())
            {
                process.StartInfo = ProcessInfo;
                process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
                process.ErrorDataReceived +=new DataReceivedEventHandler(process_ErrorDataReceived);
                process.Exited += new EventHandler(process_Exited);

                
//                finishedProcess = false;
                process.Start();
                if (this.ProcessInfo.RedirectStandardError)
                    process.BeginErrorReadLine();
                if (this.ProcessInfo.RedirectStandardOutput)
                    process.BeginOutputReadLine();


                bool finishedOK = process.WaitForExit(timeOut);
                if (!finishedOK)
                {
                    process.Kill();
                    process.WaitForExit(1000); //wait 1 sec for end
                }
                if (process.ExitCode != 0 && finishedOK == false)
                {
                    //string err = process.StandardError.ReadToEnd();
                }
                exitCode = process.ExitCode;

            }
        }



    }
}
