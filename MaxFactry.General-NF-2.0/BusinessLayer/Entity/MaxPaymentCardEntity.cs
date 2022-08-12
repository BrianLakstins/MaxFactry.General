// <copyright file="MaxPaymentCardEntity.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.BusinessLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;

    /// <summary>
    /// Entity to represent content in a web site.
    /// </summary>
    public class MaxPaymentCardEntity : MaxEntity
    {
        /// <summary>
        /// Internal list of numbers to use for string matching.
        /// </summary>
        public const string NumberList = "0123456789";

        /// <summary>
        /// Initializes a new instance of the MaxPaymentCardEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxPaymentCardEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxPaymentCardEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxPaymentCardEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets the name of the network issuing the payment card
        /// </summary>
        public string IssuingNetwork
        {
            get
            {
                return this.GetString(this.DataModel.IssuingNetwork);
            }
        }

        /// <summary>
        /// Gets the Ranges of valid IIN
        /// </summary>
        public string IINRanges
        {
            get
            {
                return this.GetString(this.DataModel.IINRanges);
            }
        }

        /// <summary>
        /// Gets the Comma separated list of valid lengths
        /// </summary>
        public string LengthList
        {
            get
            {
                return this.GetString(this.DataModel.LengthList);
            }
        }

        /// <summary>
        /// Gets the Method used to validate entire number
        /// </summary>
        public string Validation
        {
            get
            {
                return this.GetString(this.DataModel.Validation);
            }
        }

        /// <summary>
        /// Gets the Notes associated with this data
        /// </summary>
        public string Notes
        {
            get
            {
                return this.GetString(this.DataModel.Notes);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxPaymentCardDataModel DataModel
        {
            get
            {
                return (MaxPaymentCardDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a MaxPaymentCardEntity
        /// </summary>
        /// <returns>Blank MaxPaymentCardEntity</returns>
        public static MaxPaymentCardEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxPaymentCardEntity),
                typeof(MaxPaymentCardDataModel)) as MaxPaymentCardEntity;
        }

        /// <summary>
        /// Checks Mod 10 result
        /// <see href="http://orb-of-knowledge.blogspot.com/2009/08/extremely-fast-luhn-function-for-c.html"/>
        /// </summary>
        /// <param name="lsCardDigitList">List of only numbers</param>
        /// <returns>True if valid mod 10</returns>
        public static bool IsLuhnValid(string lsCardDigitList)
        {
            int[] laDELTAS = new int[] { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            int lnCheckSum = 0;
            char[] laCharList = lsCardDigitList.ToCharArray();
            for (int lnC = laCharList.Length - 1; lnC > -1; lnC--)
            {
                int lnH = ((int)laCharList[lnC]) - 48;
                lnCheckSum += lnH;
                if (((lnC - laCharList.Length) % 2) == 0)
                {
                    lnCheckSum += laDELTAS[lnH];
                }
            }

            bool lbR = false;
            if ((lnCheckSum % 10) == 0)
            {
                lbR = true;
            }

            return lbR;
        }

        /// <summary>
        /// Extracts just the numbers from a string of possible credit card number information
        /// </summary>
        /// <param name="lsCardText">Credit card format (could have spaces and/or dashes)</param>
        /// <returns>List of numbers only.</returns>
        public static string GetCardDigitList(string lsCardText)
        {
            string lsR = string.Empty;
            for (int lnN = 0; lnN < lsCardText.Length; lnN++)
            {
                if (NumberList.IndexOf(lsCardText[lnN]) >= 0)
                {
                    lsR += lsCardText[lnN];
                }
            }

            return lsR;
        }

        /// <summary>
        /// Loads a payment card entity based on the number
        /// </summary>
        /// <param name="lsNumber">Number on the payment card</param>
        /// <returns>True if found, false if not.</returns>
        public bool LoadByNumber(string lsNumber)
        {
            MaxEntityList loList = MaxPaymentCardEntity.Create().LoadAllCache();
            int lnMatchLengthCurrent = 0;
            bool lbR = false;
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxPaymentCardEntity loEntity = loList[lnE] as MaxPaymentCardEntity;
                if (loEntity.IssuingNetwork == "Diners Club United States & Canada") 
                {
                    //// see loEntity.Notes.  IINRanges is same as Mastercard, so let it match Mastercard when we only know the number.
                }
                else
                {
                    int lnMatchLength = loEntity.GetNumberMatchLength(lsNumber);
                    if (lnMatchLength > lnMatchLengthCurrent)
                    {
                        bool lbReplace = true;
                        if (this.IssuingNetwork == "Discover Card" && loEntity.IssuingNetwork == "Maestro")
                        {
                            //// Discover Card and Maestro numbers overlap and Maestro ranges are longer and broader, but should not replace Discover.
                            lbReplace = false;
                        }

                        if (lbReplace)
                        {
                            lnMatchLengthCurrent = lnMatchLength;
                            this.Load(loEntity.Data);
                            lbR = true;
                        }
                    }
                }
            }

            return lbR;
        }

        /// <summary>
        /// Gets the number of characters that match the payment card IIN
        /// </summary>
        /// <param name="lsNumber">Number on the payment card</param>
        /// <returns>Number of matching characters.</returns>
        public int GetNumberMatchLength(string lsNumber)
        {
            string lsDigitList = GetCardDigitList(lsNumber);
            string[] laMatch = this.IINRanges.Split(',');
            int lnR = 0;
            foreach (string lsMatch in laMatch)
            {
                if (lsMatch.IndexOf("-") >= 0)
                {
                    string[] laRange = lsMatch.Split('-');
                    int lnRangeStart = MaxConvertLibrary.ConvertToInt(this.GetType(), laRange[0]);
                    int lnRangeEnd = MaxConvertLibrary.ConvertToInt(this.GetType(), laRange[1]);
                    for (int lnRange = lnRangeStart; lnRange <= lnRangeEnd; lnRange++)
                    {
                        if (lsDigitList.IndexOf(lnRange.ToString()) == 0)
                        {
                            if (lnRange.ToString().Length > lnR)
                            {
                                lnR = lnRange.ToString().Length;
                            }
                        }
                    }
                }
                else if (lsDigitList.IndexOf(lsMatch) == 0)
                {
                    if (lsMatch.Length > lnR)
                    {
                        lnR = lsMatch.Length;
                    }
                }
            }

            return lnR;
        }

        /// <summary>
        /// Checks to see if a payment card number is valid
        /// </summary>
        /// <param name="lsNumber">The number provided for the payment card.</param>
        /// <returns>True if passes all validity checks, false if not.</returns>
        public bool IsValid(string lsNumber)
        {
            bool lbR = false;
            string lsDigitList = GetCardDigitList(lsNumber);
            //// Check the length
            string[] laMatch = this.LengthList.Split(',');
            foreach (string lsMatch in laMatch)
            {
                if (lsMatch.IndexOf("-") >= 0)
                {
                    string[] laRange = lsMatch.Split('-');
                    int lnRangeStart = MaxConvertLibrary.ConvertToInt(this.GetType(), laRange[0]);
                    int lnRangeEnd = MaxConvertLibrary.ConvertToInt(this.GetType(), laRange[1]);
                    for (int lnRange = lnRangeStart; lnRange <= lnRangeEnd; lnRange++)
                    {
                        if (lsDigitList.Length.Equals(lnRange))
                        {
                            lbR = true;
                        }
                    }
                }
                else 
                {
                    int lnRange = MaxConvertLibrary.ConvertToInt(this.GetType(), lsMatch);
                    if (lsDigitList.Length.Equals(lnRange))
                    {
                        lbR = true;
                    }
                }
            }

            if (lbR && this.Validation.ToLower().Equals("luhn"))
            {
                //// Check the Luhn result
                lbR = IsLuhnValid(lsDigitList);
            }

            return lbR;
        }

        /// <summary>
        /// Gets a string that can be used to sort a list of this entity.
        /// </summary>
        /// <returns>Lowercase version of Name passed to 100 characters.</returns>
        public override string GetDefaultSortString()
        {
            return this.IssuingNetwork.ToLower() + new string(' ', 100 - this.IssuingNetwork.Length) + 
                this.IINRanges.ToLower() + new string(' ', 100 - this.IINRanges.Length) +
                base.GetDefaultSortString();
        }
    }
}
