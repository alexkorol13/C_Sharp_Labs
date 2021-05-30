using System;
using System.Linq;
using System.Windows.Input;

namespace Library
{
    public class AddDefaultGridCommand : ICommand
    {
        public bool CanExecute(object parameter)
        {
            if (parameter is V5MainCollection v5MainCollection)
            {
                if (string.IsNullOrWhiteSpace(v5MainCollection.NewClass.Info) ||
                    v5MainCollection.List.FirstOrDefault(q => q.Info == v5MainCollection.NewClass.Info) != null)
                    return false;
                if (v5MainCollection.NewClass.OY == null || v5MainCollection.NewClass.OY < 2)
                    return false;
                if (v5MainCollection.NewClass.OX == null || v5MainCollection.NewClass.OX < v5MainCollection.NewClass.OY)
                    return false;
                return true;
            }

            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter is V5MainCollection v5MainCollection)
            {
                v5MainCollection.NewClass.InitRandom();
            }
        }

        public event EventHandler CanExecuteChanged;
        

        public virtual void RaiseCanExecuteChanged(object parameter)
        {
            EventHandler canExecuteChanged = this.CanExecuteChanged;
            if (canExecuteChanged != null)
            {
                canExecuteChanged.Invoke(this, EventArgs.Empty);
            }
        }
    }
}