using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL
{
    public class PagedData<T> where T : new()
    {
        public T Data { get; set; }
        /// <summary>
        /// Total Rows in the database
        /// </summary>
        public int TotalRows { get; set; }

        public PagedData()
        {
            Data = new T();
        }
    }
}
