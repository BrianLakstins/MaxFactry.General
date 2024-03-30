// <copyright file="MaxUSStateRepositoryProvider.cs" company="Lakstins Family, LLC">
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
// <change date="8/1/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="12/2/2014" author="Brian A. Lakstins" description="Update to match interface.  Add laFields.">
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
    /// Default Provider for MaxUSStateRepository
    /// </summary>
    public class MaxUSStateRepositoryProvider : MaxBaseReadRepositoryDefaultProvider, IMaxUSStateRepositoryProvider
    {
        /// <summary>
        /// Private list of states
        /// </summary>
        private MaxDataList _oUSStateDataList = null;

        /// <summary>
        /// Initializes the list of states
        /// </summary>
        /// <param name="lsName">Name of the provider</param>
        /// <param name="loConfig">Configuration information for the provider</param>
        public override void Initialize(string lsName, MaxIndex loConfig)
        {
            base.Initialize(lsName, loConfig);
            string[] laStateData = new string[69];
            laStateData[0] = "Alabama|1|AL|0";
            laStateData[1] = "Missouri|29|MO|0";
            laStateData[2] = "Alaska|2|AK|0";
            laStateData[3] = "Montana|30|MT|0";
            laStateData[4] = "Arizona|4|AZ|0";
            laStateData[5] = "Nebraska|31|NE|0";
            laStateData[6] = "Arkansas|5|AR|0";
            laStateData[7] = "Nevada|32|NV|0";
            laStateData[8] = "California|6|CA|0";
            laStateData[9] = "New Hampshire|33|NH|0";
            laStateData[10] = "Colorado|8|CO|0";
            laStateData[11] = "New Jersey|34|NJ|0";
            laStateData[12] = "Connecticut|9|CT|0";
            laStateData[13] = "New Mexico|35|NM|0";
            laStateData[14] = "Delaware|10|DE|0";
            laStateData[15] = "New York|36|NY|0";
            laStateData[16] = "District of Columbia|11|DC|5";
            laStateData[17] = "North Carolina|37|NC|0";
            laStateData[18] = "Florida|12|FL|0";
            laStateData[19] = "North Dakota|38|ND|0";
            laStateData[20] = "Georgia|13|GA|0";
            laStateData[21] = "Ohio|39|OH|0";
            laStateData[22] = "Oklahoma|40|OK|0";
            laStateData[23] = "Oregon|41|OR|0";
            laStateData[24] = "Hawaii|15|HI|0";
            laStateData[25] = "Pennsylvania|42|PA|0";
            laStateData[26] = "Idaho|16|ID|0";
            laStateData[27] = "Rhode Island|44|RI|0";
            laStateData[28] = "Illinois|17|IL|0";
            laStateData[29] = "South Carolina|45|SC|0";
            laStateData[30] = "Indiana|18|IN|0";
            laStateData[31] = "South Dakota|46|SD|0";
            laStateData[32] = "Iowa|19|IA|0";
            laStateData[33] = "Tennessee|47|TN|0";
            laStateData[34] = "Kansas|20|KS|0";
            laStateData[35] = "Texas|48|TX|0";
            laStateData[36] = "Kentucky|21|KY|0";
            laStateData[37] = "Utah|49|UT|0";
            laStateData[38] = "Louisiana|22|LA|0";
            laStateData[39] = "Vermont|50|VT|0";
            laStateData[40] = "Maine|23|ME|0";
            laStateData[41] = "Virginia|51|VA|0";
            laStateData[42] = "Maryland|24|MD|0";
            laStateData[43] = "Washington|53|WA|0";
            laStateData[44] = "Massachusetts|25|MA|0";
            laStateData[45] = "West Virginia|54|WV|0";
            laStateData[46] = "Michigan|26|MI|0";
            laStateData[47] = "Wisconsin|55|WI|0";
            laStateData[48] = "Minnesota|27|MN|0";
            laStateData[49] = "Wyoming|56|WY|0";
            laStateData[50] = "Mississippi|28|MS|0";
            laStateData[51] = "American Samoa|60|AS|1";
            laStateData[52] = "Federated States of Micronesia|64|FM|3";
            laStateData[53] = "Guam|66|GU|1";
            laStateData[54] = "Marshall Islands|68|MH|3";
            laStateData[55] = "Northern Mariana Islands|69|MP|1";
            laStateData[56] = "Palau|70|PW|4";
            laStateData[57] = "Puerto Rico|72|PR|1";
            laStateData[58] = "U.S. Minor Outlying Islands|74|UM|2";
            laStateData[59] = "Virgin Islands of the U.S.|78|VI|1";
            laStateData[60] = "Baker Island|81|UM|6";
            laStateData[61] = "Howland Island|84|UM|6";
            laStateData[62] = "Jarvis Island|86|UM|6";
            laStateData[63] = "Johnston Atoll|67|UM|6";
            laStateData[64] = "Kingman Reef|89|UM|6";
            laStateData[65] = "Midway Islands|71|UM|6";
            laStateData[66] = "Navassa Island|76|UM|6";
            laStateData[67] = "Palmyra Atoll|95|UM|6";
            laStateData[68] = "Wake Island|79|UM|6";

            object loStateList = this.GetConfigValue(loConfig, "USStateList");
            if (loStateList is string[])
            {
                laStateData = (string[])loStateList;
            }

            MaxUSStateDataModel loDataModel = (MaxUSStateDataModel)MaxDataLibrary.GetDataModel(typeof(MaxUSStateDataModel));
            this._oUSStateDataList = new MaxDataList(loDataModel);
            foreach (string lsStateData in laStateData)
            {
                string[] laState = lsStateData.Split('|');

                MaxData loData = new MaxData(loDataModel);
                loData.Set(loDataModel.Abbreviation, laState[2]);
                loData.Set(loDataModel.Code, MaxConvertLibrary.ConvertToInt(typeof(MaxUSStateDataModel), laState[1]));
                loData.Set(loDataModel.Name, laState[0]);
                loData.Set(loDataModel.Status, MaxConvertLibrary.ConvertToInt(typeof(MaxUSStateDataModel), laState[3]));
                this._oUSStateDataList.Add(loData);
            }
        }

        /// <summary>
        /// Selects data using a provider
        /// </summary>
        /// <param name="loDataModel">Data model used to determine the structure.</param>
        /// <param name="laStatus">Array with list of status numbers.</param>
        /// <returns>List of data from select</returns>
        public MaxDataList SelectAllByStatus(MaxUSStateDataModel loDataModel, int[] laStatus)
        {
            MaxDataList loR = new MaxDataList(loDataModel);
            for (int lnD = 0; lnD < this._oUSStateDataList.Count; lnD++)
            {
                MaxData loData = this._oUSStateDataList[lnD];
                int lnDataStatus = MaxConvertLibrary.ConvertToInt(loDataModel.GetType(), loData.Get(loDataModel.Status));
                foreach (int lnStatus in laStatus)
                {
                    if (lnStatus == lnDataStatus)
                    {
                        loR.Add(loData);
                    }
                }
            }

            return loR;
        }

        /// <summary>
        /// Selects all states.  All variables are ignored.
        /// </summary>
        /// <param name="loData">Data to use.</param>
        /// <param name="loDataQuery">Query to use.</param>
        /// <param name="lnPageIndex">First page.</param>
        /// <param name="lnPageSize">Size of page.</param>
        /// <param name="lsOrderBy">Sort information.</param>
        /// <param name="lnTotal">Total count.</param>
        /// <param name="laDataNameList">Fields to include.</param>
        /// <returns>Data list representing list of states.</returns>
        public override MaxDataList Select(MaxData loData, MaxDataQuery loDataQuery, int lnPageIndex, int lnPageSize, string lsOrderBy, params string[] laDataNameList)
        {
            MaxDataList loR = this.SelectAll(loData);
            return loR;
        }

        /// <summary>
        /// Selects all states.
        /// </summary>
        /// <param name="loData">storage name</param>
        /// <param name="laDataNameList">list of fields to return from select</param>
        /// <returns>List of all states</returns>
        public override MaxDataList SelectAll(MaxData loData, params string[] laDataNameList)
        {
            MaxDataList loR = new MaxDataList(loData.DataModel);
            for (int lnD = 0; lnD < this._oUSStateDataList.Count; lnD++)
            {
                MaxData loDataReturn = this._oUSStateDataList[lnD];
                loR.Add(loDataReturn);
            }

            loR.Total = loR.Count;

            return loR;
        }
    }
}
