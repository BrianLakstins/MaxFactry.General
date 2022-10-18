﻿// <copyright file="MaxGeneralApiController.cs" company="Lakstins Family, LLC">
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
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer
{

    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Web;
    using System.Web.Http;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.AspNet.BusinessLayer;
    using MaxFactry.General.AspNet.PresentationLayer;
    using MaxFactry.General.AspNet.IIS.Mvc4.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.General.PresentationLayer;
    using MaxFactry.Base.DataLayer;

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

            lsR = MaxDataLibrary.GetStorageKey(null);

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

        [HttpPost]
        [ActionName("profilearchive")]
        public int ProfileArchive()
        {
            MaxProfileIndexEntity loProfile = MaxProfileIndexEntity.Create();
            int lnR = loProfile.ArchiveLastUpdatedOver90();
            return lnR;
        }

        [HttpPost]
        [ActionName("emailarchive")]
        public int EmailArchive()
        {
            MaxEmailEntity loEmail = MaxEmailEntity.Create();
            int lnR = loEmail.ArchiveCreatedOver30();
            return lnR;
        }

        [HttpPost]
        [ActionName("formarchive")]
        public int FormArchive()
        {
            MaxFormEntity loForm = MaxFormEntity.Create();
            int lnR = loForm.ArchiveCreatedOver30();
            return lnR;
        }
    }
}