// <copyright file="MaxAuthorizeAttribute.cs" company="Lakstins Family, LLC">
// Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// </copyright>

#region License
// <license>
// This software is provided 'as-is', without any express or implied warranty. In no 
// event will the author be held liable for any damages arising from the use of this 
// software.
//  
// Permission is granted to anyone to use this software for any purpose, including 
// commercial applications, and to alter it and redistribute it freely, subject to the 
// following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the following) in the product documentation is required.
// 
// Portions Copyright (c) Brian A. Lakstins (http://www.lakstins.com/brian/)
// 
// 2. Altered source versions must be plainly marked as such, and must not be 
// misrepresented as being the original software.
// 
// 3. This notice may not be removed or altered from any source distribution.
// </license>
#endregion

#region Change Log
// <changelog>
// <change date="6/5/2015" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using System.Reflection;
    using System.ComponentModel.DataAnnotations;
    using MaxFactry.Core;

    /// <summary>
    /// Attribute to make sure that request is authorized.
    /// </summary>
    public class MaxDateFutureValidationAttribute : ValidationAttribute
    {
        public string MonthPropertyName { get; set; }
        public string YearPropertyName { get; set; }

        /// <summary>
        /// Initializes a new instance of the MaxDateFutureValidationAttribute class.
        /// </summary>
        public MaxDateFutureValidationAttribute()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxDateFutureValidationAttribute class.
        /// </summary>
        public MaxDateFutureValidationAttribute(string lsMonthPropertyName, string lsYearPropertyName)
            : base()
        {
            this.MonthPropertyName = lsMonthPropertyName;
            this.YearPropertyName = lsYearPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext loContext)
        {
            if (value is DateTime)
            {
                if (((DateTime)value) > DateTime.Now)
                {
                    return ValidationResult.Success;
                }
                else
                {
                    return new ValidationResult("Date is in the past.");
                }
            }
            else
            {
                Type loType = value.GetType();
                MaxIndex loPropertyIndex = MaxFactryLibrary.GetPropertyListValue(value);
                int lnMonth = 0;
                int lnYear = 0;
                string[] laKey = loPropertyIndex.GetSortedKeyList();
                foreach (string lsKey in laKey)
                {
                    if (lsKey.Equals(MonthPropertyName))
                    {
                        lnMonth = MaxConvertLibrary.ConvertToInt(this.GetType(), loPropertyIndex[lsKey]);
                    }
                    else if (lsKey.Equals(YearPropertyName))
                    {
                        lnYear = MaxConvertLibrary.ConvertToInt(this.GetType(), loPropertyIndex[lsKey]);
                    }
                }

                if (lnYear >= DateTime.UtcNow.Year && lnMonth > 0 && lnMonth < 13)
                {
                    DateTime loTest = new DateTime(lnYear, lnMonth, 1).AddMonths(1);
                    if (loTest > DateTime.Now)
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Date is in the past.");
                    }
                }
            }

            return new ValidationResult("Date is not valid.");
        }
    }
}