using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace MOO.Enums.Extension
{
    /// <summary>
    /// Class used for adding extension to the Enum so you can have a description attribute
    /// </summary>
    public static class EnumExtension
    {


        /// <summary>
        ///     A generic extension method that aids in reflecting 
        ///     and retrieving any attribute that is applied to an `Enum`.
        ///     example usage: var displayAttribute =  myEnumVal.GetAttribute&lt;DisplayAttribute&gt;()
        /// </summary>
        /// <typeparam name="TAttribute">The attribute you want to retrieve</typeparam>
        /// <param name="enumValue">The Enum value you want to pull the attribute from</param>
        public static TAttribute GetAttribute<TAttribute>(this Enum enumValue)
                where TAttribute : Attribute
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<TAttribute>();
        }

        /// <summary>
        /// Gets the display attribute of the enum
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static DisplayAttribute GetDisplay(this Enum enumValue)
        {
            //return enumValue.GetType()
            //                .GetMember(enumValue.ToString())
            //                .First()
            //                .GetCustomAttribute<TAttribute>();


            var field = enumValue.GetType().GetField(enumValue.ToString());
            if (field == null)
                return null;
            if (Attribute.GetCustomAttribute(field, typeof(DisplayAttribute)) is DisplayAttribute attribute)
                return attribute;
            else
                return null;
        }



        /// <summary>
        /// Adds an extension function to an enum to get it's description
        /// </summary>
        /// <param name="enumValue"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum enumValue)
        {
            var field = enumValue.GetType().GetField(enumValue.ToString());
            if(field == null)
                return enumValue.ToString();
            if (Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                return attribute.Description;
            else
                return enumValue.ToString();
        }


        /// <summary>
        /// Returns an enumerable list of the descriptions of an Enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions<T>() where T : Enum
        {
            var typ = typeof(T);
            return GetDescriptions(typ);
        }

        /// <summary>
        /// Returns an enumerable list of the descriptions of an Enum
        /// </summary>
        /// <param name="EnumType">Enum Type to get descriptions of</param>
        /// <returns></returns>
        public static IEnumerable<string> GetDescriptions(Type EnumType)
        {
            var vals = Enum.GetValues(EnumType);
            var retVal = new List<string>();
            foreach (Enum val in vals)
            {
                retVal.Add(val.GetDescription());
            }
            return retVal;
        }

        /// <summary>
        /// Gets the Enum value by the description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Description">The Enum Description to search for</param>
        /// <param name="IgnoreCase">Whether to Ignore case sensitivity</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException"></exception>
        public static T GetEnumByDescription<T>(string Description, bool IgnoreCase = true) where T : Enum
        {
            var typ = typeof(T);
            var vals = Enum.GetValues(typ);
            foreach (Enum val in vals)
            {
                if (IgnoreCase)
                {
                    if (val.GetDescription().Trim() == Description.Trim())
                        return (T)(object)val;
                }
                else
                {
                    if (val.GetDescription().ToUpper().Trim() == Description.ToUpper().Trim())
                        return (T)(object)val;
                }
                        
            }
            //if we get here, then the value was not found
            throw new System.ArgumentException($"Requested Enum Description '{Description}' was not found.");

        }

    }


}
