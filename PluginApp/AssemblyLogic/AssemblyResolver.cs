using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.DependencyModel.Resolution;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using System.Text;

namespace PluginApp.AssemblyLogic
{
    internal sealed class AssemblyResolver : IDisposable
    {
        private readonly ICompilationAssemblyResolver assemblyResolver;
        private readonly DependencyContext dependencyContext;
        private readonly AssemblyLoadContext loadContext;

        public AssemblyResolver(string path)
        {
            this.Assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(path);
            this.dependencyContext = DependencyContext.Load(this.Assembly);


            this.assemblyResolver = new CompositeCompilationAssemblyResolver
                                    (new ICompilationAssemblyResolver[]
            {
            new AppBaseCompilationAssemblyResolver(Path.GetDirectoryName(path)),
            new ReferenceAssemblyPathResolver(),
            new PackageCompilationAssemblyResolver()
            });

            this.loadContext = AssemblyLoadContext.GetLoadContext(this.Assembly);
            LoadAssemblyReferences(this.loadContext, this.Assembly);
            this.loadContext.Resolving += OnResolving;
        }

        public Assembly Assembly { get; }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void LoadAssemblyReferences(AssemblyLoadContext context, Assembly assembly)
        {
            var references = assembly.GetReferencedAssemblies();
            if (references?.Any() == true)
            {
                var basePath = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);
                foreach (var reference in references)
                {
                    var referenceAssembly = LoadAssembly(context, reference, basePath);
                    if (reference.Name.Contains("DataRobotten"))
                    {
                        if (referenceAssembly != null)
                        {
                            LoadAssemblyReferences(context, referenceAssembly);
                        }
                    }

                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static Assembly LoadAssembly(AssemblyLoadContext context, AssemblyName reference, string basePath)
        {
            try
            {
                using (var rfs = new FileStream(Path.Combine(basePath, $"{reference.Name}.dll"), FileMode.Open, FileAccess.Read))
                {
                    return context.LoadFromStream(rfs);
                }
            }
            catch (Exception)
            {
                try
                {
                    return context.LoadFromAssemblyName(reference);
                }
                catch (Exception ex)
                {
                }
            }
            return null;
        }

        public void Dispose()
        {
            this.loadContext.Resolving -= this.OnResolving;
        }

        private Assembly OnResolving(AssemblyLoadContext context, AssemblyName name)
        {
            bool NamesMatch(RuntimeLibrary runtime)
            {
                return string.Equals(runtime.Name, name.Name, StringComparison.OrdinalIgnoreCase);
            }

            RuntimeLibrary library =
                this.dependencyContext.RuntimeLibraries.FirstOrDefault(NamesMatch);
            if (library != null)
            {
                var wrapper = new CompilationLibrary(
                    library.Type,
                    library.Name,
                    library.Version,
                    library.Hash,
                    library.RuntimeAssemblyGroups.SelectMany(g => g.AssetPaths),
                    library.Dependencies,
                    library.Serviceable);

                var assemblies = new List<string>();
                this.assemblyResolver.TryResolveAssemblyPaths(wrapper, assemblies);
                if (assemblies.Count > 0)
                {
                    return this.loadContext.LoadFromAssemblyPath(assemblies[0]);
                }
            }

            return null;
        }
    }
}
