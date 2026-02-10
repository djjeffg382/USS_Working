using System;
using System.Collections.Generic;

namespace MOO.DAL.ToLive.Models
{
    public class Hangup
    {
        public decimal Hangup_Id { get; set; }
        public decimal Crusher_Number { get; set; }
        public DateTime Start_Date { get; set; }
        public DateTime? End_Date { get; set; }
        public string Status { get; set; } = String.Empty;
        public Hangup_Type Hangup_Type { get; set; } = Hangup_Types.Unknown;
        public Hangup_Cleared_By_Type Cleared_By { get; set; } = Hangup_Cleared_By_Types.Unknown;
        public string Hangup_Comments { get; set; } = String.Empty;
        public decimal? Haul_Cycle_Rec_Ident { get; set; }
        public string Truck { get; set; } = String.Empty;
        public string Load_Unit { get; set; } = String.Empty;
        public string Load_Location { get; set; } = String.Empty;
        public string Block { get; set; } = String.Empty;
        public byte[] Hangup_Image { get; set; } = new byte[0];
        public byte[] Truck_Image { get; set; } = new byte[0];

        public static Hangup_Cleared_By_Type GetClearedBy(string type)
        {
            switch (type)
            {
                case "Back Hoe":
                    return Hangup_Cleared_By_Types.Back_Hoe;
                case "Hook":
                    return Hangup_Cleared_By_Types.Hook;
                case "Pile Driver":
                    return Hangup_Cleared_By_Types.Pile_Driver;
                default:
                    return Hangup_Cleared_By_Types.Unknown;
            }
        }
        public static Hangup_Type GetHangupType(string type)
        {
            switch (type)
            {
                case "Bridge Over":
                    return Hangup_Types.Bridge_Over;
                case "Bank":
                    return Hangup_Types.Bank;
                case "Partial":
                    return Hangup_Types.Partial;
                case "Frost":
                    return Hangup_Types.Frost;
                default:
                    return Hangup_Types.Unknown;
            }
        }
    }
    public class Hangup_Cleared_By_Type
    {
        public string DisplayName = String.Empty;
        public int ClearedById;
    }
    public class Hangup_Cleared_By_Types
    {
        public static Hangup_Cleared_By_Type Unknown = new Hangup_Cleared_By_Type()
        {
            DisplayName = "Unknown",
            ClearedById = 0,
        };
        public static Hangup_Cleared_By_Type Back_Hoe = new Hangup_Cleared_By_Type()
        {
            DisplayName = "Back Hoe",
            ClearedById = 1,
        };
        public static Hangup_Cleared_By_Type Hook = new Hangup_Cleared_By_Type()
        {
            DisplayName = "Hook",
            ClearedById = 2,
        };
        public static Hangup_Cleared_By_Type Pile_Driver = new Hangup_Cleared_By_Type()
        {
            DisplayName = "Pile Driver",
            ClearedById = 3,
        };
        public static List<Hangup_Cleared_By_Type> All { get; } = new List<Hangup_Cleared_By_Type>
        {
            Unknown, Back_Hoe, Hook, Pile_Driver
        };
    }
    public class Hangup_Type
    {
        public string DisplayName { get; set; } = string.Empty;
        public int TypeId { get; set; }
    }

    public class Hangup_Types
    {
        public static Hangup_Type Unknown { get; } = new Hangup_Type
        {
            DisplayName = "Unknown",
            TypeId = 0
        };

        public static Hangup_Type Bridge_Over { get; } = new Hangup_Type
        {
            DisplayName = "Bridge Over",
            TypeId = 1
        };

        public static Hangup_Type Bank { get; } = new Hangup_Type
        {
            DisplayName = "Bank",
            TypeId = 2
        };

        public static Hangup_Type Partial { get; } = new Hangup_Type
        {
            DisplayName = "Partial",
            TypeId = 3
        };

        public static Hangup_Type Frost { get; } = new Hangup_Type
        {
            DisplayName = "Frost",
            TypeId = 4
        };

        public static List<Hangup_Type> All { get; } = new List<Hangup_Type>
        {
            Unknown, Bridge_Over, Bank, Partial, Frost
        };
    }
}
