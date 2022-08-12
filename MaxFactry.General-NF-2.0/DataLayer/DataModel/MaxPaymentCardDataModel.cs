// <copyright file="MaxPaymentCardDataModel.cs" company="Lakstins Family, LLC">
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
// <change date="3/23/2015" author="Brian A. Lakstins" description="Initial Release">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer
{
    using System;
    using MaxFactry.Base.DataLayer;

    /// <summary>
    /// Data Model for Payment Cards
    /// <see href="http://en.wikipedia.org/wiki/Bank_card_number"/>
    /// </summary>
    public class MaxPaymentCardDataModel : MaxDataModel
    {
        /// <summary>
        /// Name of the network issuing the payment card
        /// </summary>
        public readonly string IssuingNetwork = "IssuingNetwork";

        /// <summary>
        /// Ranges of valid IIN
        /// </summary>
        public readonly string IINRanges = "IINRanges";

        /// <summary>
        /// Comma separated list of valid lengths
        /// </summary>
        public readonly string LengthList = "LengthList";

        /// <summary>
        /// Method used to validate entire number
        /// </summary>
        public readonly string Validation = "Validation";

        /// <summary>
        /// Notes associated with this data
        /// </summary>
        public readonly string Notes = "Notes";

        /// <summary>
        /// Initializes a new instance of the MaxPaymentCardDataModel class
        /// </summary>
        public MaxPaymentCardDataModel()
        {
            this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxPaymentCardRepositoryProvider);
            this.RepositoryType = typeof(MaxPaymentCardRepository);
            this.AddType(this.IssuingNetwork, typeof(MaxShortString));
            this.AddType(this.IINRanges, typeof(MaxShortString));
            this.AddType(this.LengthList, typeof(MaxShortString));
            this.AddType(this.Validation, typeof(MaxShortString));
            this.AddType(this.Notes, typeof(string));
        }
    }
}
