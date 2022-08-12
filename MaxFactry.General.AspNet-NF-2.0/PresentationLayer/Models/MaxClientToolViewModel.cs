// <copyright file="MaxClientToolViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="6/4/2015" author="Brian A. Lakstins" description="Initial creation">
// <change date="9/4/2020" author="Brian A. Lakstins" description="Update fields">
// <change date="9/6/2020" author="Brian A. Lakstins" description="Integrated with version handling updates and ability to use different url depending on what is available">
// <change date="9/14/2020" author="Brian A. Lakstins" description="Fixed code that required .NET 4.52">
// </changelog>
#endregion

namespace MaxFactry.General.AspNet.PresentationLayer
{
	using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Web;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer ;
    using MaxFactry.General.AspNet.BusinessLayer;

	/// <summary>
	/// View model for content.
	/// </summary>
    public class MaxClientToolViewModel : MaxFactry.Base.PresentationLayer.MaxBaseIdViewModel
	{
        private List<MaxClientToolViewModel> _oSortedList = null;

        /// <summary>
        /// Internal storage of an index for bundling together client tools
        /// </summary>
        private static MaxIndex _oBundleIndex = new MaxIndex();

        /// <summary>
        /// Initializes a new instance of the MaxClientToolViewModel class
        /// </summary>
        public MaxClientToolViewModel()
            : base()
        {
            
        }

        /// <summary>
        /// Initializes a new instance of the MaxClientToolViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxClientToolViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxClientToolViewModel(string lsId) : base(lsId)
        {
        }

        protected override void CreateEntity()
        {
            this.Entity = MaxClientToolEntity.Create();
        }

        public string Name
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }

        public string Version
        {
            get;
            set;
        }

        public string Location
        {
            get;
            set;
        }

        public string IncludeFilter
        {
            get;
            set;
        }

        public string LocalUrl
        {
            get;
            set;
        }

        public string LocalMinUrl
        {
            get;
            set;
        }

        public string CDNUrl
        {
            get;
            set;
        }

        public string CDNMinUrl
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public List<string> RequiredNameList
        {
            get
            {
                List<string> loR = this.Get("RequiredNameList") as List<string>;
                if (null == loR)
                {
                    loR = new List<string>();
                    this.Set("RequiredNameList", loR);
                }

                return loR;
            }

            set
            {
                this.Set("RequiredNameList", value);
            }
        }

        public List<MaxClientToolViewModel> GetSortedList()
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxClientToolViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxClientToolViewModel loViewModel = new MaxClientToolViewModel(this.EntityIndex[laKey[lnK]] as MaxClientToolEntity);
                    loViewModel.Load();
                    this._oSortedList.Add(loViewModel);
                }
            }

            return this._oSortedList;
        }

        /// <summary>
        /// Gets raw Html for the location
        /// </summary>
        /// <param name="lsLocation">Location that the Html should appear.</param>
        /// <returns>Html for the client tools.</returns>
        public virtual string GetHtml(string lsLocation)
        {
            string lsR = string.Empty;
            List<MaxClientToolEntity> loList = this.GetToolList(lsLocation);

            string lsStyleKey = string.Empty;
            string lsScriptKey = string.Empty;
            //// Generate two unique keys for this location.  One for scripts and one for styles.
            for (int lnL = 0; lnL < loList.Count; lnL++)
            {
                MaxClientToolEntity loEntity = loList[lnL] as MaxClientToolEntity;
                string lsUrl = loEntity.GetUrl();
                if (this.IsStyle(lsUrl))
                {
                    lsStyleKey += lsUrl;
                }
                else if (this.IsScript(lsUrl))
                {
                    lsScriptKey += lsUrl;
                }
            }

            bool lbStyleFound = false;
            if (_oBundleIndex.Contains(lsStyleKey))
            {
                lsR += (string)_oBundleIndex[lsStyleKey];
                lbStyleFound = true;
            }

            bool lbScriptFound = false;
            if (_oBundleIndex.Contains(lsScriptKey))
            {
                lsR += (string)_oBundleIndex[lsScriptKey];
                lbScriptFound = true;
            }

            //// Generate HTML for the script and style tags and save them for future use.
            if (!lbStyleFound || !lbScriptFound)
            {
                string lsStyleBundle = string.Empty;
                string lsScriptBundle = string.Empty;
                for (int lnL = 0; lnL < loList.Count; lnL++)
                {
                    string lsUrl = loList[lnL].GetUrl();
                    if ((!lsUrl.StartsWith("//") && !lsUrl.StartsWith("http")) || lsUrl.StartsWith("ct"))
                    {
                        if (MaxFactry.Core.MaxFactryLibrary.Environment == MaxEnumGroup.EnvironmentProduction)
                        {
                            lsUrl = "https://dns9s.azureedge.net/" + lsUrl;
                        }
                        else
                        {
                            lsUrl = "https://s.dns9.co/" + lsUrl;
                        }
                    }

                    if (this.IsStyle(lsUrl))
                    {
                        lsStyleBundle += string.Format("<link href='{0}' rel='stylesheet' type='text/css' />\r\n", lsUrl);
                    }
                    else if (this.IsScript(lsUrl))
                    {
                        if (!string.IsNullOrEmpty(loList[lnL].IncludeFilter))
                        {
                            lsScriptBundle += "<!--[if " + loList[lnL].IncludeFilter + "]>\r\n";
                        }

                        lsScriptBundle += string.Format("<script type='text/javascript' src='{0}'></script>\r\n", lsUrl);
                        if (!string.IsNullOrEmpty(loList[lnL].IncludeFilter))
                        {
                            lsScriptBundle += "<![endif]-->\r\n";
                        }
                    }
                }

                if (!lbStyleFound)
                {
                    _oBundleIndex.Add(lsStyleKey, lsStyleBundle);
                    lsR += lsStyleBundle;
                }

                if (!lbScriptFound)
                {
                    _oBundleIndex.Add(lsScriptKey, lsScriptBundle);
                    lsR += lsScriptBundle;
                }
            }

            return lsR;
        }

        protected virtual bool IsScript(string lsUrl)
        {
            if (lsUrl.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase) ||
                    lsUrl.ToLower().Contains(".js?") ||
                    lsUrl.ToLower().Contains("/js?"))
            {
                return true;
            }

            return false;
        }

        protected virtual bool IsStyle(string lsUrl)
        {
            if (lsUrl.EndsWith(".css", StringComparison.InvariantCultureIgnoreCase) ||
                lsUrl.ToLower().Contains(".css?") ||
                lsUrl.ToLower().Contains("/css?"))
            {
                return true;
            }

            return false;
        }


        protected virtual List<MaxClientToolEntity> GetToolList(string lsLocation)
        {
            return MaxClientToolEntity.GetToolList(lsLocation);
        }

        /// <summary>
        /// Loads the entity based on the Id property.
        /// Maps the current values of properties in the ViewModel to the Entity.
        /// </summary>
        /// <returns>True if successful. False if it cannot be mapped.</returns>
        protected override bool MapToEntity()
        {
            if (base.MapToEntity())
            {
                MaxClientToolEntity loEntity = this.Entity as MaxClientToolEntity;
                if (null != loEntity)
                {
                    loEntity.Name = this.Name;
                    loEntity.Description = this.Description;
                    loEntity.Version = this.Version;
                    loEntity.Location = MaxClientToolEntity.GetLocation(this.Location);
                    loEntity.IncludeFilter = this.IncludeFilter;
                    loEntity.LocalUrl = this.LocalUrl;
                    loEntity.LocalMinUrl = this.LocalMinUrl;
                    loEntity.CDNUrl = this.CDNUrl;
                    loEntity.CDNMinUrl = this.CDNMinUrl;
                    loEntity.RequiredNameList = this.RequiredNameList.ToArray();
                    loEntity.Content = this.Content;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Maps the properties of the Entity to the properties of the ViewModel.
        /// </summary>
        /// <returns>True if the entity exists.</returns>
        protected override bool MapFromEntity()
        {
            if (base.MapFromEntity())
            {
                MaxClientToolEntity loEntity = this.Entity as MaxClientToolEntity;
                if (null != loEntity)
                {
                    this.Name = loEntity.Name;
                    this.Description = loEntity.Description;
                    this.Version = loEntity.Version;
                    this.Location = MaxClientToolEntity.LocationIndex[loEntity.Location];

                    this.IncludeFilter = loEntity.IncludeFilter;
                    this.LocalUrl = loEntity.LocalUrl;
                    this.LocalMinUrl = loEntity.LocalMinUrl;
                    this.CDNUrl = loEntity.CDNUrl;
                    this.CDNMinUrl = loEntity.CDNMinUrl;
                    this.RequiredNameList = new List<string>(loEntity.RequiredNameList);
                    this.Content = loEntity.Content;
                    return true;
                }
            }

            return false;
        }
    }
}
