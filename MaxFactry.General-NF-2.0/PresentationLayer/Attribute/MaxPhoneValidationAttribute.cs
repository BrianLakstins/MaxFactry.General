// <copyright file="MaxPhoneValidationAttribute.cs" company="Lakstins Family, LLC">
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
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Attribute to make sure that request is authorized.
    /// </summary>
    public class MaxPhoneValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MaxCreditCartValidationAttribute class.
        /// </summary>
        public MaxPhoneValidationAttribute()
            : base()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext loContext)
        {
            if (null != value)
            {
                string lsValue = value.ToString();
                string lsPhone = string.Empty;
                foreach (char loChar in lsValue.ToCharArray())
                {
                    if (char.IsDigit(loChar))
                    {
                        lsPhone += loChar;
                    }
                }

                if (lsPhone.Length.Equals(10))
                {
                    return ValidationResult.Success;
                }

                if (lsPhone.Length.Equals(11) && lsPhone.Substring(0, 1) == "1")
                {
                    return ValidationResult.Success;
                }

                return new ValidationResult("Phone number is not valid.");
            }

            return new ValidationResult("Phone number was not supplied.");
        }
    }
}