using System;
using System.Collections.Generic;
using System.Text;

namespace PluginRunnerContract
{
    public interface IPluginRunner
    {
        void Initialize();
        void Start();
        void Stop();
    }
}
