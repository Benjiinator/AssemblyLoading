using PluginContract;
using PluginRunner.AssemblyLogic;
using PluginRunnerContract;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace PluginRunner
{
    public class Main : IPluginRunner
    {
        private string _basePath = string.Empty;

        public void Initialize()
        {
            Console.WriteLine("Initialize plugin runner");
            _basePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
        }

        public void Start()
        {
            Console.WriteLine("Start plugin runner");
            try
            {
                var pluginManager = new PluginManager(_basePath);
                Console.WriteLine("Created plugin manager");
                pluginManager.LoadPlugins();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }

        public void Stop()
        {
            Console.WriteLine("Stop plugin runner");
        }
    }
}
