using System;
using System.Collections.Generic;
using System.Text;

namespace PluginContract
{
    public interface IPlugin
    {
        void Initialize();

        void Start();

        void Stop();
    }
}
