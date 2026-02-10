using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL_UnitTest
{
    /// <summary>
    /// use this class as the base class for all tests in this project
    /// </summary>
    public abstract class BaseTestClass
    {
        public BaseTestClass()
        {
            Util.CheckDMARTIsDev();
        }
    }
}
