// <copyright file="MaxPaymentCardRepositoryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="7/4/2016" author="Brian A. Lakstins" description="Updated to access provider configuration using base provider methods.">
// <change date="6/5/2020" author="Brian A. Lakstins" description="Updated for change to base.">
// <change date="3/20/2024" author="Brian A. Lakstins" description="Happy birthday to my mom.  Sara Jean Lakstins (Cartwright) - 3/20/1944 to 3/14/2019.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Change parent classs.  Update for changes to parent class.">
// </changelog>
#endregion

namespace MaxFactry.General.DataLayer.Provider
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.Base.DataLayer.Library;
    using MaxFactry.Base.DataLayer.Provider;

    /// <summary>
    /// Default Provider for MaxPaymentCardRepository
    /// </summary>
    public class MaxPaymentCardRepositoryProvider : MaxBaseReadRepositoryDefaultProvider, IMaxPaymentCardRepositoryProvider
    {
        /// <summary>
        /// Private list of data
        /// </summary>
        private MaxDataList _oDataList = null;

        /// <summary>
        /// Initializes the provider
        /// </summary>
        /// <param name="lsName">Name of the provider</param>
        /// <param name="loConfig">Configuration information for the provider</param>
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            base.Initialize(lsName, loConfig);
            string[] laData = new string[21];
            laData[0] = "American Express|34,37|15|Luhn";
            laData[1] = "Bankcard|5610,560221-560225|16|Luhn";
            laData[2] = "China UnionPay|62|16-19|Luhn";
            laData[3] = "Diners Club Carte Blanche|300-305|14|Luhn";
            laData[4] = "Diners Club enRoute|2014,2149|15|";
            laData[5] = "Diners Club International|300-305,309,36,38-39|14|Luhn|20150323 - On November 8, 2004, MasterCard and Diners Club formed an alliance. International cards use the 36 prefix and are treated as MasterCards in Canada and the United States, but are treated as Diners Club cards elsewhere.  Effective October 16, 2009, Diners Club cards beginning with 30, 36, 38 or 39 have been processed by Discover Card.";
            laData[6] = "Diners Club United States & Canada|54,55|16|Luhn|20150323 - On November 8, 2004, MasterCard and Diners Club formed an alliance. Diners Club cards issued in Canada and the United States start with 54 or 55 and are treated as MasterCards worldwide.";
            laData[7] = "Discover Card|6011,622126-622925,644-649,65|16|Luhn|20150323 - Effective October 1, 2006, Discover began using the entire 65 prefix, not just 650. Also, similar to the MasterCard/Diners agreement, China Union Pay cards are now treated as Discover cards and accepted on the Discover network.";
            laData[8] = "InterPayment|636|16-19|Luhn";
            laData[9] = "InstaPayment|637-639|16|Luhn";
            laData[10] = "JCB|3528-3589|16|Luhn";
            laData[11] = "Laser|6304,6706,6771,6709|16-19|Luhn";
            laData[12] = "Dankort|5019|16|Luhn";
            laData[13] = "MasterCard|222100-272099|16|Luhn|20150323 - On November 3rd 2014, Mastercard announced that they were introducing a new series of BIn Ranges that begin with a “2” (222100–272099). The “2” series BINs will be processed the same as the “51–55” series BINs are today. They will become active 14th October 2016.";
            laData[14] = "MasterCard|51-55|16|Luhn";
            laData[15] = "Solo|6334,6767|16,18,19|Luhn";
            laData[16] = "Switch|4903,4905,4911,4936,564182,633110,6333,6759|16,18,19|Luhn|20150323 - Switch was re-branded as Maestro in mid-2007. In 2011, UK Domestic Maestro (formerly Switch) was aligned with the standard international Maestro proposition with the retention of a few residual country specific rules.";
            laData[17] = "Visa|4|13,16|Luhn";
            laData[18] = "Visa Electron|4026,417500,4405,4508,4844,4913,4917|16|Luhn";
            laData[19] = "UATP|1|15|Luhn";
            //// Maestro overlaps Discover.  It should be last so that if a Discover match is found then it is used instead.
            laData[20] = "Maestro|500000-509999,560000-699999|12-19|Luhn|20150323 - Switch was re-branded as Maestro in mid-2007. In 2011, UK Domestic Maestro (formerly Switch) was aligned with the standard international Maestro proposition with the retention of a few residual country specific rules.";

            object loList = this.GetConfigValue(loConfig, "MaxPaymentCardRepositoryProviderDataList");
            if (loList is string[])
            {
                laData = (string[])loList;
            }

            MaxPaymentCardDataModel loDataModel = (MaxPaymentCardDataModel)MaxDataLibrary.GetDataModel(typeof(MaxPaymentCardDataModel));
            this._oDataList = new MaxDataList(loDataModel);
            foreach (string lsData in laData)
            {
                string[] laDataElement = lsData.Split('|');

                MaxData loData = new MaxData(loDataModel);
                loData.Set(loDataModel.IssuingNetwork, laDataElement[0]);
                loData.Set(loDataModel.IINRanges, laDataElement[1]);
                loData.Set(loDataModel.LengthList, laDataElement[2]);
                loData.Set(loDataModel.Validation, laDataElement[3]);
                if (laDataElement.Length > 4)
                {
                    loData.Set(loDataModel.Notes, laDataElement[4]);
                }

                this._oDataList.Add(loData);
            }

            this._oDataList.Total = this._oDataList.Count;
        }

        /// <summary>
        /// Selects all.  Variables are ignored.
        /// </summary>
        /// <param name="loData">Data to use.</param>
        /// <param name="loDataQuery">Query to use.</param>
        /// <param name="lnPageIndex">First page.</param>
        /// <param name="lnPageSize">Size of page.</param>
        /// <param name="lsOrderBy">Sort information</param>
        /// <param name="lnTotal">Total count.</param>
        /// <param name="laDataNameList">Fields to include.</param>
        /// <returns>Data list representing list of states.</returns>
        public override MaxDataList Select(MaxData loData, MaxDataQuery loDataQuery, int lnPageIndex, int lnPageSize, string lsOrderBy, params string[] laDataNameList)
        {
            return this._oDataList;
        }

        /// <summary>
        /// Selects matching data element count.
        /// </summary>
        /// <param name="loData">Element with data used in the filter.</param>
        /// <param name="loDataQuery">Query information to filter results.</param>
        /// <returns>List of data from select.</returns>
        public override int SelectCount(MaxData loData, MaxDataQuery loDataQuery)
        {
            return this._oDataList.Count;
        }
    }
}
