using MOO.DAL.West_Main.Enums;
using MOO.DAL.West_Main.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MOO.DAL.West_Main.Models
{
    public sealed class VMS_Analog
    {
        public WestMainPlants Plant { get; set; }
        public short Point_Id { get; set; }
        public decimal? Dmart_Id { get; set; }
        public string Point_Name { get; set; }
        public string Description { get; set; }
        public string Eng_Units { get; set; }
        public short? Related_Contact { get; set; }
        public VMSAnaPointType Point_Type { get; set; }
        public VMSAnaEqType Equation_Type { get; set; }

        /// <summary>
        /// Scan rate in seconds (either 10 or 30)
        /// </summary>
        public short? Scan_Rate { get; set; }
        public double? Low_Limit { get; set; }
        public double? High_Limit { get; set; }


        private readonly short?[] _processPoints = new short?[6];

        /// <summary>
        /// Array of proecess points refers to properties (Pnt1, Pnt2, Pnt3... Pnt6)
        /// </summary>
        public short?[] ProcessPoints { get { return _processPoints; } }

        /// <summary>
        /// Related Process point 1
        /// </summary>
        public short? Pnt1 { get { return _processPoints[0]; } set { _processPoints[0] = value; } }


        /// <summary>
        /// Related Process point 2
        /// </summary>
        public short? Pnt2 { get { return _processPoints[1]; } set { _processPoints[1] = value; } }


        /// <summary>
        /// Related Process point 3
        /// </summary>
        public short? Pnt3 { get { return _processPoints[2]; } set { _processPoints[2] = value; } }


        /// <summary>
        /// Related Process point 4
        /// </summary>
        public short? Pnt4 { get { return _processPoints[3]; } set { _processPoints[3] = value; } }


        /// <summary>
        /// Related Process point 5
        /// </summary>
        public short? Pnt5 { get { return _processPoints[4]; } set { _processPoints[4] = value; } }


        /// <summary>
        /// Related Process point 6
        /// </summary>
        public short? Pnt6 { get { return _processPoints[5]; } set { _processPoints[5] = value; } }


        private readonly short?[] _integerConstants = new short?[4];

        /// <summary>
        /// Array of proecess points refers to properties (Ic1, Ic2, Ic3, Ic4)
        /// </summary>
        public short?[] IntegerConstants { get { return _integerConstants; } }

        /// <summary>
        /// Integer Constant 1
        /// </summary>
        public short? Ic1 { get { return _integerConstants[0]; } set { _integerConstants[0] = value; } }
        /// <summary>
        /// Integer Constant 2
        /// </summary>
        public short? Ic2 { get { return _integerConstants[1]; } set { _integerConstants[1] = value; } }
        /// <summary>
        /// Integer Constant 3
        /// </summary>
        public short? Ic3 { get { return _integerConstants[2]; } set { _integerConstants[2] = value; } }
        /// <summary>
        /// Integer Constant 4
        /// </summary>
        public short? Ic4 { get { return _integerConstants[3]; } set { _integerConstants[3] = value; } }
        public double? Constant { get; set; }

        /// <summary>
        /// Date of last update in the database
        /// </summary>
        public DateTime Last_Update { get; set; }

        /// <summary>
        /// Gets all analog points dependent on this point
        /// </summary>
        /// <param name="DigPoint"></param>
        /// <returns></returns>
        public List<VMS_Analog> GetDependencies()
        {
            return VMS_AnalogSvc.GetDependencies(this);
        }


        /// <summary>
        /// Converts an int to a array of binary booleans
        /// </summary>
        /// <param name="Value"></param>
        /// <returns></returns>
        public static bool[] GetBitArray(int Value)
        {
            string bin = Convert.ToString(Value, 2);
            bool[] retVal = new bool[32];
            for (int counter = 0; counter < bin.Length; counter++)
            {
                retVal[counter] = (bin[bin.Length - 1 - counter] == '1');
            }
            return retVal;
        }
    }
}
