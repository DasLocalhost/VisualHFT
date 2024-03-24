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
        /// <summary>
        /// Command to apply changes and close a modal window
        /// </summary>
        ICommand OkCommand { get; }

        /// <summary>
        /// Command to close a modal window without saving changes
        /// </summary>
        ICommand CancelCommand { get; }

        /// <summary>
        /// Close event to close modal window
        /// </summary>
        public event EventHandler? OnClose;
    }
}
