using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FlightTracker.ViewModels.Commands
{
    /// <summary>
    /// A basic command to run an Action
    /// </summary>
    public class RelayCommand: ICommand
    {
        #region Private Members
        /// <summary>
        /// The action to run
        /// </summary>
        private Action _action;
        #endregion

        #region Public Events
        public event EventHandler CanExecuteChanged = (sender, e) => { };
        #endregion

        #region Constructor
        public RelayCommand(Action action)
        {
            _action = action;
        }
        #endregion

        #region Command Methods
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _action();
        }
        #endregion
    }
}
