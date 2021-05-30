using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Library.Annotations;
using Newtonsoft.Json;

namespace Library
{
    public class V5MainCollection : INotifyPropertyChanged
    {

        public NewClass NewClass { get; set; }
        
        private ObservableCollection<V5Data> _list = new ObservableCollection<V5Data>();
        public ObservableCollection<V5Data> List
        {
            get => _list;
            set
            {
                _list = value;
                OnPropertyChanged(nameof(List));
            } 
        } 

        public int Count()
        {
            return List.Count;
        }

        public void Add(V5Data item)
        {
            try
            {
                List.Add(item);
                IsChanged = true;
                OnPropertyChanged(nameof(List));
            }
            catch (Exception ex)
            {
                ErrorMessage = "Add Element: " + ex.Message;
            }
        }

        public bool Remove(string id, DateTime date)
        {
            var retVal = false;
            foreach (var item in List)
            {
                if (item.Date == date && item.Info == id)
                {
                    List.Remove(item);
                    retVal = true;
                }
            }

            return retVal;
        }

        public void AddDefaults()
        {
            var list = List;
            for (var i = 0; i < 3; i++)
            {
                var grid2d = new Grid2D(0.1f, 0.2f, 5, 10);
                var dataOnGrid = new V5DataOnGrid("default data on grid" + i.ToString(), DateTime.Now, grid2d);
                dataOnGrid.InitRandom(10, 100);
                list.Add(dataOnGrid);

                var collection = new V5DataCollection("default data collection " + i.ToString(), DateTime.Now);
                collection.InitRandom(10, 20, 20, 0, 100);
                list.Add(collection);
            }

            List = list;
            IsChanged = true;
            OnPropertyChanged(nameof(List));
        }

        public override string ToString()
        {
            var str = "";
            foreach (var item in List)
            {
                str += item.ToString();
            }

            return str;
        }

        private V5DataCollection _selectedDataCollection;

        public V5DataCollection SelectedDataCollection
        {
            get =>_selectedDataCollection;
            set
            {
                _selectedDataCollection = value;
                OnPropertyChanged(nameof(SelectedDataCollection));
            }
        }
        private bool _isChanged;

        public V5MainCollection()
        {
            NewClass = new NewClass(this);
        }

        public bool IsChanged
        {
            get => _isChanged;
            set
            {
                _isChanged = value;
                OnPropertyChanged(nameof(IsChanged));
            } 
        }

        public void Save(string filename)
        {
            try
            {
                var sr = JsonConvert.SerializeObject(List);
                File.WriteAllText(filename, sr);
            }
            finally
            {
                IsChanged = false;
                OnPropertyChanged();
            }
        }

        public void Load(string filename)
        {

            try
            {
                var lst = new ObservableCollection<V5Data>();
                var str = File.ReadAllText(filename);
                var list = JsonConvert.DeserializeObject<IEnumerable<V5Data>>(str);
                foreach (var one in list)
                {
                    if (one.Info.ToLower().Contains("grid"))
                    {
                        var grid = new V5DataOnGrid(one.Info, one.Date);
                        lst.Add(grid);
                    }

                    if (one.Info.ToLower().Contains("collection"))
                    {
                        var grid = new V5DataCollection(one.Info, one.Date);
                        lst.Add(grid);
                    }
                    
                }
                List = new ObservableCollection<V5Data>(lst?.ToList());
            }
            catch (Exception e)
            {
                ErrorMessage = e.Message;
            }
            finally
            {
                IsChanged = true;
                OnCollectionChanged(NotifyCollectionChangedAction.Add);
                OnPropertyChanged(nameof(List));
            }
        }


        public void AddFromFile(string filename)
        {
            try
            {
                var dg = new V5DataOnGrid(filename);
                Add(dg);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void AddDefaultDataOnGrid()
        {
            var grid = new Grid2D();
            var doG = new V5DataOnGrid("Default DoG", default, grid);
            doG.InitRandom(0, 10);
            Add(doG);
        }

        public void RemoveAt(int index)
        {
            List.RemoveAt(index);
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public string ErrorMessage { get; set; }

        public void AddDefaultDataCollection()
        {
            var dc = new V5DataCollection();
            dc.InitRandom(2, 5, 5, 0, 5);
            Add(dc);
        }

        [field: NonSerialized] public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        [field: NonSerialized] public event NotifyCollectionChangedEventHandler CollectionChanged;

        public void OnCollectionChanged(NotifyCollectionChangedAction ev)
        {
            if (CollectionChanged != null)
            {
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            }
        }
    }
}