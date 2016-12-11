using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Permissions;
//using System.Runtime.Serialization;

namespace AlgoNature.Components
{
    
    class RedrawHandlingList<T> : IList<T> where T : IToRedrawEventHandlingList, IGrowableGraphicChild
    {
        //private static ObjectIDGenerator ObjectIDGen = new ObjectIDGenerator();
        public event RedrawEventHandler Redraw;
        private List<T> objects;
        private List<string> UniqueIds;
        //private bool monitoring;
        //public Guid InstanceID { get; private set; }
        AppDomain currentDomain = AppDomain.CurrentDomain;

        //[SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.ControlAppDomain)]
        public RedrawHandlingList()
        {
            objects = new List<T>();
            UniqueIds = new List<string>();

            handler = new UnhandledExceptionEventHandler(MyHandler);
            currentDomain.UnhandledException += handler;
        }

        //private Task Monitor()
        //{
        //    if (!monitoring)
        //    {
        //        ObservableCollection<T> obj;
        //        monitoring = true;
        //        while (monitoring)
        //        {
        //            try
        //            {
        //                obj = objects;
        //            }
        //            catch (RedrawException)
        //            {
        //                Redraw(this, EventArgs.Empty);
        //            }
        //        }
        //        monitoring = false;
        //    }
        //    return Task.CompletedTask;
        //}

        UnhandledExceptionEventHandler handler;
        void MyHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            //Console.WriteLine("MyHandler caught : " + e.Message);
            //bool equals = false;
            
            if (RedrawException.Equals(e, UniqueIds))
            {
                Redraw(this, EventArgs.Empty);
            }
            else
            {
                currentDomain.UnhandledException -= handler;
                throw e;
                currentDomain.UnhandledException += handler;
            }
        }

        public T this[int index]
        {
            get
            {
                return objects[index];
            }

            set
            {
                objects[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return objects.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Add(T item)
        {
            objects.Add(item);
            UniqueIds.Add(item.UniqueIDToRedrawException());
        }

        public void Clear()
        {
            objects.Clear();
            UniqueIds.Clear();
            currentDomain.UnhandledException -= handler;
        }

        public bool Contains(T item)
        {
            return objects.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            objects.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return objects.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return objects.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            objects.Insert(index, item);
            UniqueIds.Insert(index, item.UniqueIDToRedrawException());
        }

        public bool Remove(T item)
        {
            UniqueIds.Remove(item.UniqueIDToRedrawException());
            return objects.Remove(item);
        }

        public void RemoveAt(int index)
        {
            objects.RemoveAt(index);
            UniqueIds.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return objects.GetEnumerator();
        }
    }
}
