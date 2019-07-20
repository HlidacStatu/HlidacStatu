using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Service.WebShots
{
    static class Program
    {
        public static Devmasters.Core.Logging.Logger logger = new Devmasters.Core.Logging.Logger("HlidacStatu.Service.Webshots");
        public static System.Globalization.CultureInfo culture = System.Globalization.CultureInfo.GetCultureInfo("en-US");

        static void Main(string[] args)
        {
            System.Threading.ThreadPool.GetMaxThreads(out int maxWT, out int complPT);
            logger.Info($"Starting main process; ThreadPool.MaxThreads={maxWT} / {complPT}");
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(CurrentDomain_ProcessExit);
            AppDomain.CurrentDomain.DomainUnload += new EventHandler(CurrentDomain_ProcessExit);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomain_UnhandledException);

            System.Globalization.CultureInfo.DefaultThreadCurrentCulture = culture;
            System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = culture;

            int newMaxT = 30000;
            logger.Info($"Setting ThreadPool.MaxThreads : {newMaxT}");
            System.Threading.ThreadPool.SetMaxThreads(newMaxT,newMaxT);

            System.Diagnostics.Process.GetCurrentProcess().PriorityClass = System.Diagnostics.ProcessPriorityClass.BelowNormal;
            bool console = false;

            //force console param

            console = true;

            try
            {
                if (args.Count() > 0)
                {
                    logger.Info("starting with console parameter");
                    console = (args[0] == "/console");
                    //var tm = HlidacStatu.Lib.OCR.TesseractOCR.Instance().OCRImage(@"c:\!\a.jpg");
                }

                if (System.Diagnostics.Debugger.IsAttached || console)
                {
                    Service1 service = new Service1();
                    logger.Info("Starting service process manualy");
                    service.Start();
                }
                else
                {
                    ServiceBase[] ServicesToRun;
                    ServicesToRun = new ServiceBase[] { new Service1() };
                    logger.Info("Starting service process via servicebase");
                    ServiceBase.Run(ServicesToRun);

                }

            }
            catch (Exception e)
            {
                logger.Fatal("Unexpected error: ", e);

            }
        }
        public static void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            logger.Warning("Process exit");

        }

        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs ex)
        {
            try
            {
                if (ex.ExceptionObject != null)
                    logger.Error(new Devmasters.Core.Logging.LogMessage()
                                                            .SetVersionOfAllAssemblies()
                                                            .SetException(ex.ExceptionObject as Exception)
                                                            .SetStack(Devmasters.Core.Logging.Logger.GetCallingMethod(true))
                                                            .SetMessage("Worker UnhandledException")
                                                      );

                else
                    logger.Error(new Devmasters.Core.Logging.LogMessage()
                                                            .SetVersionOfAllAssemblies()
                                                            .SetStack(Devmasters.Core.Logging.Logger.GetCallingMethod(true))
                                                            .SetMessage("Worker UnhandledException unknown ExceptionObject")
                                                      );
            }
            catch (Exception e)
            {
                logger.Error("CurrentDomain_UnhandledException", e);
            }

        }
    }
}
