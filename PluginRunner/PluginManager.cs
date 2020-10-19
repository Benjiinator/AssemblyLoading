using PluginContract;
using PluginRunner.AssemblyLogic;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace PluginRunner
{
    public class PluginManager
    {
        private readonly string _basePath = string.Empty;
        private List<IPlugin> _plugins;
        public PluginManager(string basePath)
        {
            _basePath = basePath;
            _plugins = new List<IPlugin>();
        }

        public void LoadPlugins()
        {
            Console.WriteLine("Loading plugins");
            var plugins = GetPlugins();
            Console.WriteLine($"{plugins.Count()} plugins found");
            if(plugins?.Any() == true)
            {
                foreach (var kvp in plugins)
                {
                    if (_plugins.Count() == 0)
                    {
                        var assemblyLoader = new AssemblyLoader(_basePath);
                        var plugin = assemblyLoader.Load<IPlugin>(kvp.Key);
                        if (plugin != null)
                        {
                            _plugins.Add(plugin);
                            plugin.Initialize();
                            plugin.Start();
                        }
                    }
                }
            }
        }

        private Dictionary<string, string> GetPlugins()
        {
            try
            {
                // Get all dll files in directory
                var files = Directory.GetFiles(_basePath, "*Plugin.dll")
                    .Select(f => FileVersionInfo.GetVersionInfo(f));

                var sorted = files.Where(f => !f.InternalName.Contains("Contract") && !f.InternalName.Contains("Runner"));
                return sorted.ToDictionary(v => v.FileName, v => v.FileVersion);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return null;
        }
    }
}
