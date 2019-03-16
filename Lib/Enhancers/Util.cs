using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HlidacStatu.Lib.Enhancers
{
    public static class Util
    {

        private static object objLock = new object();
        private static List<IEnhancer> enhancers = null;

        public static List<IEnhancer> GetEnhancers(string moreFromPath = null)
        {
            if (enhancers != null)
                return enhancers;

            lock (objLock)
            {

                ////load DLL's from disk
                List<System.Reflection.Assembly> dlls = new List<System.Reflection.Assembly>();
                string pathForDlls = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                foreach (var file in System.IO.Directory.EnumerateFiles(pathForDlls, "*plugin*.dll"))
                {
                    try
                    {
                        string filename = System.IO.Path.GetFileNameWithoutExtension(file);
                        dlls.Add(System.Reflection.Assembly.Load(filename));
                    }
                    catch (Exception)
                    {
                    }
                }



                var typeI = typeof(IEnhancer);
                List<System.Type> types = null;
                try
                {
                    types = AppDomain.CurrentDomain.GetAssemblies()
                        .Union(dlls)
                        .SelectMany(s => s.GetTypes())
                        .Where(p =>
                            typeI.IsAssignableFrom(p)
                            && p.IsAbstract == false
                            )
                        .ToList();

                }
                catch (System.Reflection.ReflectionTypeLoadException lex)
                {
                    StringBuilder sb = new StringBuilder();
                    if (lex.LoaderExceptions != null)
                        foreach (Exception ex in lex.LoaderExceptions)
                            sb.AppendFormat("{0}\n----------------\n", ex.ToString());

                    HlidacStatu.Util.Consts.Logger.Fatal("Cannot make list of enhancer instances, reason: " + sb.ToString(), lex);

                }
                catch (Exception e)
                {
                    HlidacStatu.Util.Consts.Logger.Fatal("Cannot make list of enhancer instances ", e);

                    throw;
                }

                var ps = new List<IEnhancer>();
                foreach (var type in types)
                {
                    try
                    {
                        IEnhancer parser = (IEnhancer)Activator.CreateInstance(type);
                        ps.Add(parser);
                        HlidacStatu.Util.Consts.Logger.Info("Creating instance of parser " + type.FullName);

                    }
                    catch (Exception e)
                    {
                        //NoveInzeraty.Lib.Constants.NIRoot.Error("Cannot make instance of parser " + type.FullName, e);
                    }
                }
                enhancers = ps;

                return enhancers;
            }
        }
    }
}
