using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

//namespace FIVESTARVC.Models
//{
//    public class MyList : IEnumerable<Room>
//    {
//        Room[] m_Items = null;
//        int freeIndex = 0;

//        public MyList()
//        {
//            // For the sake of simplicity lets keep them as arrays
//            // ideally it should be link list
//            m_Items = new Room[100];
//        }

//        public void Add(Room item)
//        {
//            // Let us only worry about adding the item 
//            m_Items[freeIndex] = item;
//            freeIndex++;
//        }

//        #region IEnumerable<T> Members

//        public IEnumerator<Room> GetEnumerator()
//        {
//            foreach (Room t in m_Items)
//            {
//                // Lets check for end of list (its bad code since we used arrays)
//                if (t == null) // this wont work is T is not a nullable type
//                {
//                    break;
//                }

//                // Return the current element and then on next function call 
//                // resume from next element rather than starting all over again;
//                yield return t;
//            }
//        }

//        #endregion

//        #region IEnumerable Members

//        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
//        {
//            // Lets call the generic version here
//            return this.GetEnumerator();
//        }

//    }
//}