using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace PluginRunner.AssemblyLogic
{
    public class AssemblyLoader
    {
        private readonly string _basePath = string.Empty;
        public AssemblyLoader(string basePath)
        {
            _basePath = basePath;
        }

        public T Load<T>(string fileName) where T : class
        {
            var fullPath = Path.Combine(_basePath, fileName);
            var pluginRunnerContext = new AssemblyResolver(fullPath);
            return ResolveInstance<T>(pluginRunnerContext);
        }

        private T ResolveInstance<T>(AssemblyResolver context) where T : class
        {
            var types = GetTypesWithInterface<T>(context.Assembly);
            var type = types.First();
            return Activator.CreateInstance(type) as T;
        }

        private IEnumerable<Type> GetTypesWithInterface<T>(Assembly asm) where T : class
        {
            var it = typeof(T);
            return asm.GetLoadableTypes().Where(it.IsAssignableFrom).ToList();
        }
    }
}
