using Nancy.Hosting.Self;
using System;
using System.ServiceProcess;
using System.Threading;

namespace HlidacStatu.Service.WebShots
{
    public partial class Service1 : ServiceBase
    {
        public static string ComputerName = "";
        bool isConsole = false;

        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;
        Nancy.Hosting.Self.NancyHost host = null;


        private CancellationTokenSource _cancelSource;

        public void Start()
        {
            this.isConsole = true;
            this.OnStart(new string[] { });
        }

        public Service1()
        {
            InitializeComponent();
            Service1.ComputerName = System.Environment.MachineName;
        }

        protected override void OnStart(string[] args)
        {
            base.OnStart(args);
            Program.logger.Info("SERVICE Starting");


            _cancelSource = new CancellationTokenSource();

            int numOfInstances = 3;
            int.TryParse(Devmasters.Core.Util.Config.GetConfigValue("NumOfInstances"), out numOfInstances);
            try
            {

                HlidacStatu.Util.WebShot.Workers.CreateAndStartWorkers(numOfInstances, _cancelSource);

                var url = Devmasters.Core.Util.Config.GetConfigValue("ListeningUrl");
                if (string.IsNullOrEmpty(url))
                    url = "http://127.0.0.1:9090";

                var hostConfiguration = new HostConfiguration
                {
                    UrlReservations = new UrlReservations() { CreateAutomatically = false },
                    RewriteLocalhost = false
                };
                host = new Nancy.Hosting.Self.NancyHost(new Uri(url));
                host.Start();
                Program.logger.Info($"Nancy Server listening on {url}");
                if (isConsole)
                {
                    do
                    {
                        System.Threading.Thread.Sleep(1000);
                    } while (true);
                }

            }
            finally
            {
                if (isConsole)
                {
                    Program.logger.Info($"Canceling threads");

                    _cancelSource?.Cancel(false);
                    System.Threading.Thread.Sleep(3000);
                    HlidacStatu.Util.WebShot.Workers.CancelWorkers();

                    _cancelSource?.Dispose();

                }
            }
        }
        protected override void OnStop()
        {


            host.Stop();
            host.Dispose();

            try
            {
                _cancelSource?.Cancel(false);
                System.Threading.Thread.Sleep(3000);
                HlidacStatu.Util.WebShot.Workers.CancelWorkers();

                _cancelSource?.Dispose();
                //_initializationTask?.Dispose();
            }
            catch (Exception stopException)
            {
                Program.logger.Debug($"stopException {stopException?.ToString()}");

                //log any errors
            }


            HlidacStatu.Util.WebShot.Chrome.TheOnlyStaticInstance.Dispose();
            System.Threading.Thread.Sleep(500);
            //_shutdownEvent.Set();
            //if (!_thread.Join(3000))
            //{ // give the thread 3 seconds to stop
            //    _thread.Abort();
            //}
        }

        private void WorkerThreadFunc()
        {

        }
    }
}
