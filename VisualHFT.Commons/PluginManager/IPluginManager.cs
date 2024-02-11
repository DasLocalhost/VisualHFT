using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisualHFT.PluginManager;

namespace VisualHFT.Commons.PluginManager
{
    public interface IPluginManager
    {
        public List<IPlugin> AllPlugins { get; }
    }
}