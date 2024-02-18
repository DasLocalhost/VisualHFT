using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace VisualHFT.Commons.WPF.ViewModel
{
    /// <summary>
    /// View model that could be opened as a dialog.
    /// </summary>
    public interface IModularViewModel
    {
        ICommand CloseCommand { get; }

        public event EventHandler OnClose;
    }
}
