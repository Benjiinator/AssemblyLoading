using PluginContract;
using PluginShared;
using System;
using System.Collections.Generic;
using System.Text;

namespace DemoPlugin
{
    public class Main : IPlugin
    {
        public void Initialize()
        {
            Console.WriteLine("init plugin");
        }

        public void Start()
        {
            Console.WriteLine("start plugin");
            // do sql client stuff
            var _storageManager = new StorageManager("DataRobotten", null, null);
            Console.WriteLine(_storageManager._connectionString);
        }

        public void Stop()
        {
            Console.WriteLine("stop plugin");
        }
    }
}
