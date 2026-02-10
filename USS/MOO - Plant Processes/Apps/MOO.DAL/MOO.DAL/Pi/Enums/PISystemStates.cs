using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.Pi.Enums
{
    /// <summary>
    /// Commonly used digital states that we can insert into PI
    /// </summary>
    /// <remarks>There are more of these, look at Pi System Management Tools ->Digital States -> System for the codes.  Codes are all negative</remarks>
    public enum PISystemStates
    {
        No_Sample = -211,
        No_Result = -212,
        Unit_Down = -213,
        Sample_Bad = -214, 
        Equip_Fail = -215,
        No_Lab_Data = -216,
        Bad_Lab_Data = -219,
        Failed = -241,
        No_Data = -248,
        Bad_Data = -290
    }
}
