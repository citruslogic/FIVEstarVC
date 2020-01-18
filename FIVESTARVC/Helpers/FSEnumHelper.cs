﻿using System;
using System.ComponentModel;
using System.Reflection;

namespace FIVESTARVC.Models
{
    public static class FSEnumHelper
    {
        /// <summary>
        /// Retrieve the description on the enum, e.g.
        ///[Description("Air Force")]
        ///[Display(Name = "Air Force")]
        ///AIRFORCE
        /// Then when you pass in the enum, it will retrieve the description
        /// </summary>
        /// <param name="en">The Enumeration</param>
        /// <returns>A string representing the friendly name</returns>
        public static string GetDescription(Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

    }

}