using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace MOO_UnitTest
{
    public class ErrorLogTest
    {
        [Fact]
        public void TestAddErrorLog()
        {
            Util.RegisterFactories();
            MOO.Exceptions.ErrorLog.LogError(Util.PROGRAM_NAME, "Test Error Message from MOO.dll", "No Stack", "No Additional Info", 
                    MOO.Exceptions.ErrorLog.ErrorLogType.Info );

            MOO.Exceptions.MOOSqlException SqlEx = new(new Exception("Test SQL Exception"), "SQL Error Goes Here");
            MOO.Exceptions.ErrorLog.LogError(Util.PROGRAM_NAME, SqlEx);
        }
    }
}
