// <copyright file="MaxClientToolEntity.cs" company="Lakstins Family, LLC">
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
// follAspNetg restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not claim that 
// you wrote the original software. If you use this software in a product, an 
// acknowledgment (see the follAspNetg) in the product documentation is required.
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
// <change date="1/23/2015" author="Brian A. Lakstins" description="Initial Release">
// <change date="9/4/2020" author="Brian A. Lakstins" description="Add defaults">
// <change date="9/6/2020" author="Brian A. Lakstins" description="Updated to handle versions.  Added latest 3.3 version of Bootstrap and 1 version of Jquery as defaults">
// <change date="9/30/2020" author="Brian A. Lakstins" description="Add jquery plugins">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.BusinessLayer
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text.RegularExpressions;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.Base.DataLayer;
    using MaxFactry.General.AspNet.DataLayer;

    /// <summary>
    /// Entity to represent virtual text file in a web site.
    /// </summary>
    public class MaxClientToolEntity : MaxBaseIdEntity
    {
        public const int LocationHead = 1;

        public const int LocationBody = 5;

        public const int LocationEnd = 9;

        /// <summary>
        /// Key used to store information in the current process index.
        /// </summary>
        private const string MaxClientToolLibraryKey = "__MaxClientToolLibraryKey";

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class
        /// </summary>
        /// <param name="loData">object to hold data</param>
        public MaxClientToolEntity(MaxData loData)
            : base(loData)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxClientToolEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        public string Name
        {
            get
            {
                return this.GetString(this.DataModel.Name);
            }

            set
            {
                this.Set(this.DataModel.Name, value);
            }
        }

        public string Description
        {
            get
            {
                return this.GetString(this.DataModel.Description);
            }

            set
            {
                this.Set(this.DataModel.Description, value);
            }
        }

        public string Version
        {
            get
            {
                return this.GetString(this.DataModel.Version);
            }

            set
            {
                this.Set(this.DataModel.Version, value);
            }
        }

        public string VersionSort
        {
            get
            {
                string lsR = string.Empty;
                string[] laVersion = this.Version.Split(new char[] { '.' });
                foreach (string lsV in laVersion)
                {
                    int lnV = MaxConvertLibrary.ConvertToInt(typeof(object), lsV);
                    if (lnV.ToString() == lsV)
                    {
                        lsR += MaxConvertLibrary.ConvertToSortString(typeof(object), lnV);
                    }
                    else
                    {
                        lsR += MaxConvertLibrary.ConvertToSortString(typeof(object), lsV);
                    }
                }

                return lsR;
            }
        }

        public int Location
        {
            get
            {
                return this.GetInt(this.DataModel.Location);
            }

            set
            {
                this.Set(this.DataModel.Location, value);
            }
        }

        public string LocalUrl
        {
            get
            {
                return this.GetString(this.DataModel.LocalUrl);
            }

            set
            {
                this.Set(this.DataModel.LocalUrl, value);
            }
        }

        public string LocalMinUrl
        {
            get
            {
                return this.GetString(this.DataModel.LocalMinUrl);
            }

            set
            {
                this.Set(this.DataModel.LocalMinUrl, value);
            }
        }

        public string CDNUrl
        {
            get
            {
                return this.GetString(this.DataModel.CDNUrl);
            }

            set
            {
                this.Set(this.DataModel.CDNUrl, value);
            }
        }

        public string CDNMinUrl
        {
            get
            {
                return this.GetString(this.DataModel.CDNMinUrl);
            }

            set
            {
                this.Set(this.DataModel.CDNMinUrl, value);
            }
        }

        public string IncludeFilter
        {
            get
            {
                return this.GetString(this.DataModel.IncludeFilter);
            }

            set
            {
                this.Set(this.DataModel.IncludeFilter, value);
            }
        }

        public string Content
        {
            get
            {
                return this.GetString(this.DataModel.Content);
            }

            set
            {
                this.Set(this.DataModel.Content, value);
            }
        }

        /// <summary>
        /// Gets or sets the names of client tools required for this client tool
        /// Could be name:version to be more precise
        /// </summary>
        public string[] RequiredNameList
        {
            get
            {
                string[] laR = this.GetObject(this.DataModel.RequiredNameListText, typeof(string[])) as string[];
                if (null == laR)
                {
                    laR = new string[] { };
                    this.SetObject(this.DataModel.RequiredNameListText, laR);
                }

                return laR;
            }

            set
            {
                this.SetObject(this.DataModel.RequiredNameListText, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxClientToolDataModel DataModel
        {
            get
            {
                return (MaxClientToolDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        protected override MaxIndex GetDefaultIdIndex()
        {
            MaxIndex loR = new MaxIndex();
            DateTime ldMinDate = DateTime.MinValue.AddYears(1);
            loR.Add("{02D03AB6-AFA0-4CCF-B584-9FB12F89CEDA}", ldMinDate);
            loR.Add("{DD6B09FC-31AE-413B-A2E7-33E96E630CE3}", ldMinDate);
            loR.Add("{1E962139-3102-4A96-BCF7-930EB3CECDA0}", ldMinDate);
            loR.Add("{BFBE7296-E274-471A-99C7-7E134C2A374A}", ldMinDate);
            loR.Add("{FAFB76C7-9294-43BE-8F1D-DD2C3B1258CE}", ldMinDate);
            loR.Add("{BDBF7B4A-CDA9-4622-81B3-386BC84229F6}", ldMinDate);
            loR.Add("{96934142-3CA9-4F11-AACC-32DDEFDDCE1E}", ldMinDate);
            loR.Add("{97305B8E-33FF-4D11-9667-AB5C6CC0B63C}", ldMinDate);
            loR.Add("{46B22314-CE2C-4164-BC83-9B33728313F4}", ldMinDate);
            loR.Add("{51B80D0A-0E01-4707-9133-EC5785853172}", ldMinDate);
            loR.Add("{CD3F98A9-5CB0-45C9-8364-C0E370AFFC38}", ldMinDate);
            loR.Add("{1F179171-66DD-4A0A-B35B-E8E7B658E66F}", ldMinDate);
            loR.Add("{4BE8582B-887F-47C9-88F4-6C461BDB4D2E}", ldMinDate);
            loR.Add("{DF9045B3-6F97-4DE2-A113-7095DA47DF4A}", ldMinDate);
            loR.Add("{8DDC6BD4-A64C-4F7B-A914-282AEA9A03B3}", ldMinDate);
            loR.Add("{834752ED-C574-4367-8D48-F3AC33B30B77}", ldMinDate);
            loR.Add("{5B454709-9BCB-46C5-9009-295B1168ED88}", ldMinDate);
            loR.Add("{0681F4B9-5D47-44CB-ADAC-BFAD5F059A89}", ldMinDate);
            loR.Add("{08853D5E-A085-4D1B-A68A-D4E42F4B0CAD}", ldMinDate);
            loR.Add("{1EF183AE-C2F8-452C-BCBC-C36978052EF3}", ldMinDate);
            loR.Add("{C4E9970C-A30A-4114-B294-3C0B7DFDBCD9}", ldMinDate);
            loR.Add("{79C48175-CD68-4969-9FEB-CE674F08F7AA}", ldMinDate);
            loR.Add("{D8CE326D-ECE8-47F2-912C-BC91A4CC2566}", ldMinDate);
            loR.Add("{CAB42C75-90A0-43AD-A072-BC8B0F392246}", ldMinDate);
            loR.Add("{76886110-1C3B-417B-A7E1-785B11FB15A9}", ldMinDate);
            loR.Add("{21594F39-3F84-4896-97EF-C1C08AE03FA6}", new DateTime(2020, 10, 1));
            loR.Add("{F1C190A2-2006-4C8C-B1A0-5D0476ACD54E}", new DateTime(2020, 10, 1));
            //loR.Add("", ldMinDate);
            return loR;
        }

        protected override void AddDefault(string lsId)
        {
            MaxClientToolEntity loEntity = MaxClientToolEntity.Create();
            loEntity.SetId(MaxConvertLibrary.ConvertToGuid(typeof(object), lsId));
            loEntity.IsActive = true;
            if (lsId == "{02D03AB6-AFA0-4CCF-B584-9FB12F89CEDA}")
            {
                //// efactrysolutions@gmail.com account - https://console.cloud.google.com/apis/credentials/key/17384739-e5e9-4d88-967d-c8fdb489d78e?project=efactrymaps
                loEntity.Name = "GoogleMapsApi";
                loEntity.Version = "3.42";
                loEntity.Location = LocationHead;
                loEntity.CDNUrl = "https://maps.googleapis.com/maps/api/js?v=3.42&key=AIzaSyAq2ONbki01zyb9IpUVM_7M7-mH6NShi2w";
            }
            else if (lsId == "{DD6B09FC-31AE-413B-A2E7-33E96E630CE3}")
            {
                //// https://developers.google.com/recaptcha/docs/v3
                //// https://developers.google.com/recaptcha/docs/display
                loEntity.Name = "reCAPTCHA";
                loEntity.Version = "2";
                loEntity.Location = LocationHead;
                loEntity.CDNUrl = "https://www.google.com/recaptcha/api.js";
            }
            else if (lsId == "{1E962139-3102-4A96-BCF7-930EB3CECDA0}")
            {
                //// https://github.com/scottjehl/Respond
                loEntity.Name = "Respond";
                loEntity.Version = "1.4.2";
                loEntity.Location = LocationHead;
                loEntity.IncludeFilter = "lt IE 9";
                loEntity.LocalUrl = "ct/Respond/1.4.2/respond.src.js";
                loEntity.LocalMinUrl = "ct/Respond/1.4.2/respond.min.js";
            }
            else if (lsId == "{BFBE7296-E274-471A-99C7-7E134C2A374A}")
            {
                //// https://getbootstrap.com/docs/3.3/getting-started/#download
                loEntity.Name = "Bootstrap";
                loEntity.Version = "3.3.5";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/Bootstrap/3.3.5/DefaultFull/css/bootstrap.css";
                loEntity.LocalMinUrl = "ct/Bootstrap/3.3.5/DefaultFull/css/bootstrap.min.css";
                loEntity.RequiredNameList = new string[] { "Bootstrap-Bug:3.3.5", "JQuery:1", "Respond:1.4.2" };

            }
            else if (lsId == "{FAFB76C7-9294-43BE-8F1D-DD2C3B1258CE}")
            {
                loEntity.Name = "Bootstrap-Theme";
                loEntity.Version = "3.3.5";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/Bootstrap/3.3.5/DefaultFull/css/bootstrap-theme.css";
                loEntity.LocalMinUrl = "ct/Bootstrap/3.3.5/DefaultFull/css/bootstrap-theme.min.css";
                loEntity.RequiredNameList = new string[] { "Bootstrap:3" };
            }
            else if (lsId == "{BDBF7B4A-CDA9-4622-81B3-386BC84229F6}")
            {
                loEntity.Name = "Bootstrap-Bug";
                loEntity.Version = "3.3.5";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/Bootstrap/3.3.5/BrowserBugs/js/Bugs.js";
                loEntity.LocalMinUrl = "";
            }
            else if (lsId == "{96934142-3CA9-4F11-AACC-32DDEFDDCE1E}")
            {
                loEntity.Name = "Bootstrap-Bug";
                loEntity.Version = "3.3.5";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/Bootstrap/3.3.5/BrowserBugs/css/Bugs.css";
                loEntity.LocalMinUrl = "";
            }
            else if (lsId == "{97305B8E-33FF-4D11-9667-AB5C6CC0B63C}")
            {
                loEntity.Name = "JQuery";
                loEntity.Version = "1.11.3";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/JQuery/1.11.3/jquery-1.11.3.js";
                loEntity.LocalMinUrl = "ct/JQuery/1.11.3/jquery-1.11.3.min.js";
            }
            else if (lsId == "{46B22314-CE2C-4164-BC83-9B33728313F4}")
            {
                loEntity.Name = "JQuery-UI";
                loEntity.Version = "1.11.4";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/JQuery.UI/1.11.4/Full/jquery-ui.css";
                loEntity.LocalMinUrl = "ct/JQuery.UI/1.11.4/Full/jquery-ui.min.css";
            }
            else if (lsId == "{51B80D0A-0E01-4707-9133-EC5785853172}")
            {
                loEntity.Name = "JQuery-Migrate";
                loEntity.Version = "1.2.1";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/JQuery-Migrate/1.2.1/jquery-migrate-1.2.1.js";
                loEntity.LocalMinUrl = "ct/JQuery-Migrate/1.2.1/jquery-migrate-1.2.1.min.js";
            }
            else if (lsId == "{CD3F98A9-5CB0-45C9-8364-C0E370AFFC38}")
            {
                loEntity.Name = "Require";
                loEntity.Version = "2.1.20";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/Require/2.1.20/Require.js";
                loEntity.LocalMinUrl = "ct/Require/2.1.20/Require.min.js";
            }
            else if (lsId == "{1F179171-66DD-4A0A-B35B-E8E7B658E66F}")
            {
                loEntity.Name = "SmartmenusBootstrapAddon";
                loEntity.Version = "1.0.0-beta1";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/Smartmenus/1.0.0-beta1/addons/bootstrap/jquery.smartmenus.bootstrap.css";
                loEntity.LocalMinUrl = "";
            }
            else if (lsId == "{4BE8582B-887F-47C9-88F4-6C461BDB4D2E}")
            {
                loEntity.Name = "JQuery-Plugin-Lazyload";
                loEntity.Version = "1.9.3";
                loEntity.Location = LocationHead;
                loEntity.LocalUrl = "ct/JQuery-Plugin/LazyLoad/1.9.3/jquery.lazyload.js";
                loEntity.LocalMinUrl = "ct/JQuery-Plugin/LazyLoad/1.9.3/jquery.lazyload.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }
            else if (lsId == "{DF9045B3-6F97-4DE2-A113-7095DA47DF4A}")
            {
                loEntity.Name = "Bootstrap";
                loEntity.Version = "3.3.5";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/Bootstrap/3.3.5/DefaultFull/js/bootstrap.js";
                loEntity.LocalMinUrl = "ct/Bootstrap/3.3.5/DefaultFull/js/bootstrap.min.js";
            }
            else if (lsId == "{8DDC6BD4-A64C-4F7B-A914-282AEA9A03B3}")
            {
                loEntity.Name = "Smartmenus";
                loEntity.Version = "1.0.0-beta1";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/Smartmenus/1.0.0-beta1/jquery.smartmenus.js";
                loEntity.LocalMinUrl = "ct/Smartmenus/1.0.0-beta1/jquery.smartmenus.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }
            else if (lsId == "{834752ED-C574-4367-8D48-F3AC33B30B77}")
            {
                loEntity.Name = "SmartmenusBootstrapAddon";
                loEntity.Version = "1.0.0-beta1";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/Smartmenus/1.0.0-beta1/addons/bootstrap/jquery.smartmenus.bootstrap.js";
                loEntity.LocalMinUrl = "ct/Smartmenus/1.0.0-beta1/addons/bootstrap/jquery.smartmenus.bootstrap.min.js";
                loEntity.RequiredNameList = new string[] { "Smartmenus:1.0.0-beta1", "Bootstrap:3" };
            }
            else if (lsId == "{5B454709-9BCB-46C5-9009-295B1168ED88}")
            {
                loEntity.Name = "JQuery-UI";
                loEntity.Version = "1.11.4";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/JQuery.UI/1.11.4/Full/jquery-ui.js";
                loEntity.LocalMinUrl = "ct/JQuery.UI/1.11.4/Full/jquery-ui.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }
            else if (lsId == "{0681F4B9-5D47-44CB-ADAC-BFAD5F059A89}")
            {
                loEntity.Name = "JQuery-Validate";
                loEntity.Version = "1.14.0";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/JQuery-Validate/1.14.0/jquery.validate.js";
                loEntity.LocalMinUrl = "ct/JQuery-Validate/1.14.0/jquery.validate.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }
            else if (lsId == "{08853D5E-A085-4D1B-A68A-D4E42F4B0CAD}")
            {
                loEntity.Name = "Knockout";
                loEntity.Version = "3.3.0";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/knockout/3.3.0/knockout-3.3.0.js";
                loEntity.LocalMinUrl = "ct/knockout/3.3.0/knockout-3.3.0.min.js";
            }
            else if (lsId == "{1EF183AE-C2F8-452C-BCBC-C36978052EF3}")
            {
                loEntity.Name = "TinyMCE";
                loEntity.Version = "4.2.7";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/TinyMCE/4.2.7/Development/js/tinymce/tinymce.js";
                loEntity.LocalMinUrl = "ct/TinyMCE/4.2.7/Default/tinymce.min.js";
            }
            else if (lsId == "{C4E9970C-A30A-4114-B294-3C0B7DFDBCD9}")
            {
                //// https://getbootstrap.com/docs/3.3/getting-started/#download
                loEntity.Name = "Bootstrap";
                loEntity.Version = "3.3.7";
                loEntity.Location = LocationHead;
                loEntity.CDNMinUrl = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/css/bootstrap.min.css";
                loEntity.RequiredNameList = new string[] { "JQuery" };

            }
            else if (lsId == "{79C48175-CD68-4969-9FEB-CE674F08F7AA}")
            {
                //// https://getbootstrap.com/docs/3.3/getting-started/#download
                loEntity.Name = "Bootstrap";
                loEntity.Version = "3.3.7";
                loEntity.Location = LocationEnd;
                loEntity.CDNMinUrl = "https://maxcdn.bootstrapcdn.com/bootstrap/3.3.7/js/bootstrap.min.js";
            }
            else if (lsId == "{D8CE326D-ECE8-47F2-912C-BC91A4CC2566}")
            {
                ////https://code.jquery.com/
                loEntity.Name = "JQuery";
                loEntity.Version = "1.12.4";
                loEntity.Location = LocationHead;
                loEntity.CDNUrl = "https://code.jquery.com/jquery-1.12.4.js";
                loEntity.CDNMinUrl = "https://code.jquery.com/jquery-1.12.4.min.js";
            }
            else if (lsId == "{CAB42C75-90A0-43AD-A072-BC8B0F392246}")
            {
                //// https://code.jquery.com/ui/
                loEntity.Name = "JQuery-UI";
                loEntity.Version = "1.12.1";
                loEntity.Location = LocationEnd;
                loEntity.CDNUrl = "https://code.jquery.com/ui/1.12.1/jquery-ui.js";
                loEntity.CDNMinUrl = "https://code.jquery.com/ui/1.12.1/jquery-ui.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }
            else if (lsId == "{76886110-1C3B-417B-A7E1-785B11FB15A9}")
            {
                //// https://code.jquery.com/ui/
                loEntity.Name = "JQuery-UI-Theme-base";
                loEntity.Version = "1.12.1";
                loEntity.Location = LocationEnd;
                loEntity.CDNUrl = "https://code.jquery.com/ui/1.12.1/themes/base/jquery-ui.css";
                loEntity.RequiredNameList = new string[] { "JQuery-UI" };
            }
            else if (lsId == "{21594F39-3F84-4896-97EF-C1C08AE03FA6}")
            {
                //// https://github.com/Mottie/tablesorter
                loEntity.Name = "JQuery-Plugin-tablesorter";
                loEntity.Version = "2.31.3";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/JQuery-Plugin/tablesorter/2.31.3/js/jquery.tablesorter.js";
                loEntity.LocalMinUrl = "ct/JQuery-Plugin/tablesorter/2.31.3/js/jquery.tablesorter.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }
            else if (lsId == "{F1C190A2-2006-4C8C-B1A0-5D0476ACD54E}")
            {
                //// https://harvesthq.github.io/chosen/
                loEntity.Name = "JQuery-Plugin-chosen";
                loEntity.Version = "1.87";
                loEntity.Location = LocationEnd;
                loEntity.LocalUrl = "ct/JQuery-Plugin/chosen/1.87/chosen.jquery.js";
                loEntity.LocalMinUrl = "ct/JQuery-Plugin/chosen/1.87/chosen.jquery.min.js";
                loEntity.RequiredNameList = new string[] { "JQuery" };
            }

            loEntity.Insert();
        }

        protected override void UpdateDefault(string lsId)
        {
            MaxClientToolEntity loEntity = MaxClientToolEntity.Create();
            Guid loId = MaxConvertLibrary.ConvertToGuid(typeof(object), lsId);
            if (loEntity.LoadByIdCache(loId))
            {
                if (lsId == "{21594F39-3F84-4896-97EF-C1C08AE03FA6}")
                {
                    loEntity.Location = LocationEnd;
                    loEntity.LocalUrl = "ct/JQuery-Plugin/tablesorter/2.31.3/js/jquery.tablesorter.js";
                    loEntity.LocalMinUrl = "ct/JQuery-Plugin/tablesorter/2.31.3/js/jquery.tablesorter.min.js";
                }

                loEntity.Update();
            }

            base.UpdateDefault(lsId);
        }

        /// <summary>
        /// Creates a new instance of the MaxVirtualTextFileEntity class.
        /// </summary>
        /// <returns>a new instance of the MaxVirtualTextFileEntity class.</returns>
        public static MaxClientToolEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxClientToolEntity),
                typeof(MaxClientToolDataModel)) as MaxClientToolEntity;
        }

        public static Dictionary<int, string> LocationIndex
        {
            get
            {
                Dictionary<int, string> loR = new Dictionary<int, string>();
                loR.Add(LocationHead, "HEAD");
                loR.Add(LocationBody, "BODY");
                loR.Add(LocationEnd, "END");
                return loR;
            }
        }

        /// <summary>
        /// Indicates to include the tool on the current web page.
        /// </summary>
        /// <param name="lsTool">Name and possibly verion of the tool.</param>
        public static void Include(string lsTool)
        {
            List<Guid> loIdList = GetIdListForProcess();
            //// Load client tool records that match the name and version and add them to the list
            MaxEntityList loList = MaxClientToolEntity.Create().LoadAllActiveMatchCache(lsTool);
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxClientToolEntity loEntity = loList[lnE] as MaxClientToolEntity;
                if (!loIdList.Contains(loEntity.Id))
                {
                    loIdList.Add(loEntity.Id);
                }
            }

            if (loList.Count == 0)
            {
                throw new MaxException("Client Tool [" + lsTool + "] is not available.  It's either inactive or does not exist.");
            }
            else
            {
                SetIdListForProcess(loIdList);
            }
        }

        /// <summary>
        /// Gets a list of files for the provided location.
        /// </summary>
        /// <param name="lsLocation">Location the file should be on the page.</param>
        /// <returns>List of registered files for the location.</returns>
        public static List<MaxClientToolEntity> GetToolList(string lsLocation)
        {
            List<MaxClientToolEntity> loR = new List<MaxClientToolEntity>();
            MaxEntityList loList = Create().LoadAllCache();
            int lnLocation = GetLocation(lsLocation);
            List<Guid> loToolList = GetIdListForProcess();
            loToolList = AddRequired(loToolList);
            foreach (Guid loId in loToolList)
            {
                for (int lnE = 0; lnE < loList.Count; lnE++)
                {
                    MaxClientToolEntity loEntity = loList[lnE] as MaxClientToolEntity;
                    if (loEntity.Location == lnLocation && loEntity.Id == loId)
                    {
                        loR.Add(loEntity);
                    }
                }
            }

            return loR;
        }

        protected static List<Guid> GetIdListForProcess()
        {
            Guid[] laR = MaxConfigurationLibrary.GetValue(MaxEnumGroup.ScopeProcess, MaxClientToolLibraryKey) as Guid[];
            if (null == laR)
            {
                laR = new Guid[] { };
                MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxClientToolLibraryKey, laR);
            }

            return new List<Guid>(laR);
        }

        protected static void SetIdListForProcess(List<Guid> loList)
        {
            MaxConfigurationLibrary.SetValue(MaxEnumGroup.ScopeProcess, MaxClientToolLibraryKey, loList.ToArray());
        }

        protected static List<Guid> AddRequired(List<Guid> loList)
        {
            List<Guid> loR = new List<Guid>();
            bool lbChanged = false;
            MaxEntityList loEntityList = MaxClientToolEntity.Create().LoadAllCache();
            foreach (Guid loId in loList)
            {
                //// Add required tools that are not already in the list before adding each tool 
                MaxClientToolEntity loEntity = MaxClientToolEntity.Create();
                if (loEntity.LoadByIdCache(loId) && loEntity.IsActive)
                {
                    foreach (string lsRequired in loEntity.RequiredNameList)
                    {
                        MaxEntityList loRequiredList = MaxClientToolEntity.Create().LoadAllActiveMatchCache(lsRequired);
                        for (int lnE = 0; lnE < loRequiredList.Count; lnE++)
                        {
                            MaxClientToolEntity loRequiredEntity = loRequiredList[lnE] as MaxClientToolEntity;
                            if (!loList.Contains(loRequiredEntity.Id) && !loR.Contains(loRequiredEntity.Id))
                            {
                                loR.Add(loRequiredEntity.Id);
                                lbChanged = true;
                            }
                        }
                    }
                }

                if (!loR.Contains(loEntity.Id))
                {
                    loR.Add(loEntity.Id);
                }
            }

            if (lbChanged)
            {
                //// Make sure all required tools have been added
                return AddRequired(loR);
            }

            return loR;
        }

        public virtual MaxEntityList LoadAllActiveMatchCache(string lsTool)
        {
            string[] laTool = lsTool.Split(new char[] { ':' });
            string lsToolName = laTool[0];
            string lsToolVersion = string.Empty;
            if (laTool.Length == 2)
            {
                lsToolVersion = laTool[1];
            }

            MaxEntityList loR = new MaxEntityList(this.GetType());
            //// Get all that match the name
            MaxEntityList loList = this.LoadAllCache();
            //// Sort List by name and version
            SortedList<string, MaxClientToolEntity> loSortedList = new SortedList<string, MaxClientToolEntity>();
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxClientToolEntity loEntity = loList[lnE] as MaxClientToolEntity;
                if (loEntity.IsActive && loEntity.Name == lsToolName)
                {
                    loSortedList.Add(loEntity.GetDefaultSortString(), loEntity);
                }
            }

            if (loSortedList.Count > 0)
            {
                List<MaxClientToolEntity> loMatchList = new List<MaxClientToolEntity>(loSortedList.Values);
                loMatchList.Reverse();

                //// Check for exact match to version
                foreach (MaxClientToolEntity loEntity in loMatchList)
                {
                    if (loEntity.Version == lsToolVersion)
                    {
                        loR.Add(loEntity);
                    }
                }

                if (loR.Count == 0)
                {
                    //// Check for version match and version latest
                    string lsVersionMatch = string.Empty;
                    string lsVersionLatest = string.Empty;
                    foreach (MaxClientToolEntity loEntity in loMatchList)
                    {
                        if (string.Empty == lsVersionLatest)
                        {
                            lsVersionLatest = loEntity.Version;
                        }

                        if (string.Empty == lsVersionMatch && loEntity.IsVersionMatch(lsToolVersion))
                        {
                            lsVersionMatch = loEntity.Version;
                        }
                    }

                    //// Add those matching the version
                    if (string.Empty != lsVersionMatch)
                    {
                        foreach (MaxClientToolEntity loEntity in loMatchList)
                        {
                            if (loEntity.Version == lsVersionMatch)
                            {
                                loR.Add(loEntity);
                            }
                        }
                    }
                    else if (string.Empty != lsVersionLatest)
                    {
                        //// Add those that are the latest version
                        foreach (MaxClientToolEntity loEntity in loMatchList)
                        {
                            if (loEntity.Version == lsVersionLatest)
                            {
                                loR.Add(loEntity);
                            }
                        }
                    }
                }
            }

            return loR;
        }

        public static int GetLocation(string lsLocation)
        {
            int lnR = LocationHead;
            foreach (int lnKey in LocationIndex.Keys)
            {
                if (LocationIndex[lnKey] == lsLocation)
                {
                    lnR = lnKey;
                }
            }

            return lnR;
        }

        public bool IsVersionMatch(string lsToolVersion)
        {
            bool lbR = false;
            string[] laToolVersion = lsToolVersion.Split(new char[] { '.' });
            string[] laVersion = this.Version.Split(new char[] { '.' });
            if (laVersion.Length >= laToolVersion.Length)
            {
                lbR = true;
                for (int lnT = 0; lnT < laToolVersion.Length; lnT++)
                {
                    if (laVersion[lnT] != laToolVersion[lnT])
                    {
                        lbR = false;
                    }
                }
            }

            return lbR;
        }

        public override string GetDefaultSortString()
        {
            return this.Name + this.VersionSort + MaxConvertLibrary.ConvertToSortString(typeof(object), this.Location) + base.GetDefaultSortString();
        }

        public string GetUrl()
        {
            string lsR = this.CDNMinUrl;
            if (string.IsNullOrEmpty(lsR))
            {
                lsR = this.LocalMinUrl;
            }

            if (string.IsNullOrEmpty(lsR) || MaxFactryLibrary.Environment == MaxEnumGroup.EnvironmentDevelopment)
            {
                if (!string.IsNullOrEmpty(this.CDNUrl))
                {
                    lsR = this.CDNUrl;
                }
                else if (!string.IsNullOrEmpty(this.LocalUrl))
                {
                    lsR = this.LocalUrl;
                }
            }

            return lsR;
        }
    }
}
