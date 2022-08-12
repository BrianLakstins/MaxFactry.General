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
    using System.ComponentModel.DataAnnotations;
    using MaxFactry.General.BusinessLayer;

    /// <summary>
    /// Attribute to make sure that request is authorized.
    /// </summary>
    public class MaxCreditCardValidationAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the MaxCreditCartValidationAttribute class.
        /// </summary>
        public MaxCreditCardValidationAttribute()
            : base()
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext loContext)
        {
            if (null != value)
            {
                string lsNumber = value.ToString();
                MaxPaymentCardEntity loEntity = MaxPaymentCardEntity.Create();
                if (loEntity.LoadByNumber(lsNumber))
                {
                    if (loEntity.IsValid(lsNumber))
                    {
                        return ValidationResult.Success;
                    }
                    else
                    {
                        return new ValidationResult("Credit card number is not valid.");
                    }
                }
                else
                {
                    return new ValidationResult("Credit card number type was not found.");
                }
            }

            return new ValidationResult("Credit card number was not supplied.");
        }
    }
}