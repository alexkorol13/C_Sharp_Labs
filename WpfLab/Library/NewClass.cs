using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Library.Annotations;

namespace Library
{
    public class NewClass : INotifyPropertyChanged, IDataErrorInfo
    {
        private V5MainCollection _v5MainCollection;

        public NewClass(V5MainCollection collection)
        {
            _v5MainCollection = collection;
            AddDefaultGridCommand = new AddDefaultGridCommand();
        }

        public V5MainCollection V5MainCollection
        {
            get => _v5MainCollection;
            set
            {
                _v5MainCollection = value;
                OnPropertyChanged(nameof(Library.V5MainCollection));
            }
        }

        public void InitRandom()
        {
            if (OX == null || OY == null)
                return;
            var newDataOnGrid = new V5DataOnGrid(Info, DateTime.Now);
            newDataOnGrid.InitRandom((float)OX, (float)OY);
            V5MainCollection.Add(newDataOnGrid);
            
        }


        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public string Error { get; }

        private string _info;
        public string Info
        {
            get => _info;
            set
            {
                _info = value;
                OnPropertyChanged(nameof(Info));
            }
        }

        private float? _oY;
        private float? _oX;
        private float? _step;

        public float? OY
        {
            get => _oY;
            set
            {
                _oY = value;
                OnPropertyChanged(nameof(OY));
            }
        }
        public float? OX
        {
            get => _oX;
            set
            {
                _oX = value;
                OnPropertyChanged(nameof(OX));
            }
        }

        public float? Step
        {
            get => _step;
            set
            {
                _step = value;
                OnPropertyChanged(nameof(Step));
            }
        }
        
        public string this[string columnName] 
        {
            get
            {
                string error=String.Empty;
                switch (columnName)
                {
                    case "Info":
                    {
                        if (string.IsNullOrWhiteSpace(Info) ||
                            V5MainCollection.List.FirstOrDefault(q=>q.Info  == Info) != null)
                        {
                            error = "Не правильно введено поле Info";
                        }                        break;
                    }
                    case "OY":
                    {
                        if (OY == null || OY < 2)
                        {
                            error = "Число узлов оУ  должно быть не меньше 2";
                        }
                        break;
                    }
                    case "OX":
                    {
                        if (OX == null || OX < OY)
                        {
                            error = "Число узлов оX  должно быть больше oY";
                        }
                        break;
                    }
                }
                AddDefaultGridCommand.RaiseCanExecuteChanged(null);
                return error;
            }
        }
        
        public AddDefaultGridCommand AddDefaultGridCommand { get; }
    }
}