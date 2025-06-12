// <copyright file="MaxGeneralApiController.cs" company="Lakstins Family, LLC">
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
// <change date="12/10/2019" author="Brian A. Lakstins" description="Initial creation">
// <change date="5/31/2020" author="Brian A. Lakstins" description="Add endpoints to start archive process">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.  Updated to use DataKey.">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Remove api calls for archiving data">
// <change date="6/11/2025" author="Brian A. Lakstins" description="Update for ApplicationKey">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using MaxFactry.Core;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.Base.DataLayer.Library;

    [System.Web.Http.AllowAnonymous]
    [MaxEnableCorsAttribute]
    public class MaxGeneralApiController : MaxBaseApiController
    {
        [HttpGet]
        [HttpPost]
        [ActionName("index")]
        public async Task<HttpResponseMessage> Index()
        {
            Guid loId = Guid.NewGuid();
            string lsR = string.Empty;

            lsR = string.Empty;
            try
            {
                Stream loContent = await this.Request.Content.ReadAsStreamAsync();
                StreamReader loReader = new StreamReader(loContent);
                lsR = loReader.ReadToEnd();
                loReader.Close();
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(MaxEnumGroup.LogError, "General API Error.", loE));
            }

            lsR = MaxDataLibrary.GetApplicationKey();

            HttpResponseMessage loR = new HttpResponseMessage();
            loR.Content = new StringContent(lsR);
            loR.Content.Headers.Remove("Content-Type");
            loR.Content.Headers.Add("Content-Type", "text/plain");

            MaxOwinLibrary.SetStorageKeyForClient(HttpContext.Current.Request.Url, false, string.Empty);
            return loR;
        }

        [HttpGet]
        [HttpPost]
        [ActionName("index2")]
        public string Index2(string lsId)
        {
            Guid loId = Guid.NewGuid();
            string lsR = lsId;

            lsR = lsId;

            return lsR;
        }

        [HttpGet]
        [HttpOptions]
        [ActionName("daterange")]
        public async Task<HttpResponseMessage> DateRange()
        {
            HttpStatusCode loStatus = HttpStatusCode.OK;
            var loResponseItem = new
            {
                Key = "Key",
                Name = "Name"
            };

            MaxApiResponseViewModel loR = this.GetResponse(loResponseItem);
            if (this.Request.Method == HttpMethod.Get)
            {
                var loRequestItem = new
                {
                    RangeIndexList = "RangeIndexList"
                };

                MaxApiRequestViewModel loRequest = await this.GetRequest();

                string[] laDateRangeName = _sDateRangeNameText.Split(new char[] { ',' });
                string lsRangeIndexList = loRequest.Item.GetValueString(loRequestItem.RangeIndexList);
                if (!string.IsNullOrEmpty(lsRangeIndexList))
                {
                    string[] laRangeIndex = lsRangeIndexList.Split(new char[] { ',' });
                    for (int lnR = 0; lnR < laRangeIndex.Length; lnR++)
                    {
                        int lnRangeIndex = MaxConvertLibrary.ConvertToInt(typeof(object), laRangeIndex[lnR]);
                        MaxIndex loItem = GetDateFilter(lnRangeIndex, DateTime.MinValue, DateTime.MaxValue);
                        loR.ItemList.Add(loItem);
                    }
                }
                else
                {
                    for (int lnD = 0; lnD < laDateRangeName.Length; lnD++)
                    {
                        MaxIndex loItem = GetDateFilter(lnD, DateTime.MinValue, DateTime.MaxValue);
                        loR.ItemList.Add(loItem);
                    }
                }
            }

            return this.GetResponseMessage(loR, loStatus);
        }
    }
}