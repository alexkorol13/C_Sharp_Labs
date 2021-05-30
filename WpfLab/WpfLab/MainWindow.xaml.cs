using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Library;

namespace WpfLab
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private V5MainCollection _main = new V5MainCollection();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = _main;
        }

        private void ButtonNew(object sender, RoutedEventArgs e)
        {
            if (_main.IsChanged && UnsavedChanges()) return;
            _main = new V5MainCollection();
            DataContext = _main;
            ErrorMsg();
        }

        private void ButtonOpen(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonSave(object sender, RoutedEventArgs e)
        {
        }

        private void ButtonAddElement(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new Microsoft.Win32.OpenFileDialog();
                var result = dlg?.ShowDialog();
                _main.AddFromFile(dlg.FileName);
            }
            catch (Exception)
            {
                MessageBox.Show("Add error");
            }
            finally
            {
                ErrorMsg();
            }
        }

        private void ButtonV5DataCollection(object sender, RoutedEventArgs e)
        {
            _main.AddDefaultDataCollection();
            ErrorMsg();
        }

        private void ButtonV5MainCollection(object sender, RoutedEventArgs e)
        {
            _main.AddDefaults();
            DataContext = _main;
            ErrorMsg();
        }

        private void ButtonV5DataOnGrid(object sender, RoutedEventArgs e)
        {
            _main.AddDefaultDataOnGrid();
            ErrorMsg();
        }


        private bool UnsavedChanges()
        {
            var msg = MessageBox.Show("Save Changes?", "Save", MessageBoxButton.YesNoCancel);
            switch (msg)
            {
                case MessageBoxResult.Cancel:
                    return true;
                case MessageBoxResult.No:
                    return false;
                case MessageBoxResult.Yes:
                {
                    var dialog = new Microsoft.Win32.SaveFileDialog();
                    if (dialog.ShowDialog() ?? false)
                        _main.Save(dialog.FileName);
                    break;
                }
            }

            return false;
        }

        private void ButtonRemove(object sender, RoutedEventArgs e)
        {
            if (LisBoxMain.SelectedIndex != -1)
                _main?.RemoveAt(LisBoxMain.SelectedIndex);
        }

        private void FilterDataCollection(object sender, FilterEventArgs args)
        {
            var item = args.Item;
            if (item == null) return;
            args.Accepted = item is V5DataCollection ||
                            (item is V5Data v5Data && v5Data.Info.ToLower().Contains("collection"));
        }

        private void FilterDataOnGrid(object sender, FilterEventArgs args)
        {
            var item = args.Item;
            if (item != null != true) return;
            args.Accepted = item is V5DataOnGrid || (item is V5Data v5Data && v5Data.Info.ToLower().Contains("grid"));
        }

        private void ErrorMsg()
        {
            if (_main.ErrorMessage == null) return;
            MessageBox.Show(_main.ErrorMessage, "Error");
            _main.ErrorMessage = null;
        }

        private void ExecutedOpenCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try

            {
                var fd = new Microsoft.Win32.OpenFileDialog();

                if (_main.IsChanged)
                    if (!UnsavedChanges())
                    {
                        if (!(fd?.ShowDialog() ?? false)) return;
                        _main = new V5MainCollection();
                        _main.Load(fd.FileName);
                        DataContext = _main;
                    }
                
                if (!(fd?.ShowDialog() ?? false)) return;
                _main = new V5MainCollection();
                _main.Load(fd.FileName);
                DataContext = _main;
            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }
            finally
            {
                ErrorMsg();
            }
        }

        private void ExecutedSaveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog();
                if (dialog?.ShowDialog() ?? false)
                    _main.Save(dialog.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error ButtonSave" + ex.Message);
            }
            finally
            {
                ErrorMsg();
            }
        }

        private void CanExecutedSaveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = _main.IsChanged;
        }

        private void ExecutedRemoveCommand(object sender, ExecutedRoutedEventArgs e)
        {
            _main?.RemoveAt(LisBoxMain.SelectedIndex);
        }

        private void CanExecutedRemoveCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = LisBoxMain?.SelectedIndex != -1;
        }
    }
}