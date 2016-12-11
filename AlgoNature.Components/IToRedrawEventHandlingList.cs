using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlgoNature.Components
{
    class RedrawException : Exception
    {
        //public RedrawException() : RedrawException() { }
        public RedrawException(string UniqueID) : base("RedrawException:" + UniqueID) { }
        public static bool Equals(Exception exc, IToRedrawEventHandlingList obj)
            => exc.Message == ("RedrawException:" + obj.UniqueIDToRedrawException());
        public static bool Equals(Exception exc, List<string> IDs)
        {
            foreach (string id in IDs)
            {
                if ("RedrawException:" + id == exc.Message)
                {
                    return true;
                }
            }
            return false;
        }
    }
    interface IToRedrawEventHandlingList
    {
        void ThrowRedrawException();
        string UniqueIDToRedrawException();
    }
}
