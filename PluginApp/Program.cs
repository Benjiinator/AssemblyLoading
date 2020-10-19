using PluginApp.AssemblyLogic;
using PluginRunnerContract;
using System;
using System.Diagnostics;
using System.IO;

namespace PluginApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var basePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
            try
            {
                var assemblyLoader = new AssemblyLoader(basePath);
                var runnerInstance = assemblyLoader.Load<IPluginRunner>("PluginRunner.dll");
                runnerInstance.Initialize();
                runnerInstance.Start();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadKey();
        }
    }
}
