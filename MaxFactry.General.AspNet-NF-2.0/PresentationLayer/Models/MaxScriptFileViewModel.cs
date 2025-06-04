// <copyright file="MaxScriptFileViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="7/14/2016" author="Brian A. Lakstins" description="Initial creation">
// <change date="7/22/2016" author="Brian A. Lakstins" description="Added global script for facebook">
// <change date="6/4/2025" author="Brian A. Lakstins" description="Change base class to remove versioning">
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
    public class MaxScriptFileViewModel : MaxFactry.Base.PresentationLayer.MaxBaseGuidKeyViewModel
	{
        private List<MaxScriptFileViewModel> _oSortedList = null;

        /// <summary>
        /// Initializes a new instance of the MaxScriptFileViewModel class
        /// </summary>
        public MaxScriptFileViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxScriptFileViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxScriptFileViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxScriptFileViewModel(string lsId) : base(lsId)
        {
        }

        protected override void CreateEntity()
        {
            this.Entity = MaxScriptFileEntity.Create();
        }

        public virtual bool LoadFromName(string lsName)
        {
            MaxEntityList loList = MaxScriptFileEntity.Create().LoadAllCache();
            bool lbFound = false;
            for (int lnE = 0; lnE < loList.Count; lnE++)
            {
                MaxScriptFileEntity loEntity = loList[lnE] as MaxScriptFileEntity;
                if (loEntity.Name.Equals(lsName, StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Entity = loEntity;
                    lbFound = true;
                }
            }

            if (lbFound)
            {
                return this.MapFromEntity();
            }
            else
            {
                //// Global scripts
                if (lsName.Equals("facebook.js", StringComparison.InvariantCultureIgnoreCase))
                {
                    this.Content = "  window.fbAsyncInit = function() {\r\n" +
                                    "    FB.init({\r\n" +
                                    "      appId      : '430907323707718',\r\n" +
                                    "      xfbml      : true,\r\n" +
                                    "      version    : 'v2.7'\r\n" +
                                    "    });\r\n" +
                                    "  };\r\n" +
                                    "  (function(d, s, id){\r\n" +
                                    "     var js, fjs = d.getElementsByTagName(s)[0];\r\n" +
                                    "     if (d.getElementById(id)) {return;}\r\n" +
                                    "     js = d.createElement(s); js.id = id;\r\n" +
                                    "     js.src = \"//connect.facebook.net/en_US/sdk.js\";\r\n" +
                                    "     fjs.parentNode.insertBefore(js, fjs);\r\n" +
                                    "   }(document, 'script', 'facebook-jssdk'));\";\r\n";
                }
            }

            return false;
        }

        public string Name
        {
            get;
            set;
        }


        public string Content
        {
            get;
            set;
        }

        public string ContentMin
        {
            get;
            set;
        }

        public string ContentName
        {
            get;
            set;
        }

        public string ScriptType
        {
            get;
            set;
        }

        public List<MaxScriptFileViewModel> GetSortedList()
        {
            if (null == this._oSortedList)
            {
                this._oSortedList = new List<MaxScriptFileViewModel>();
                SortedList<string, MaxScriptFileViewModel> loSortedList = new SortedList<string, MaxScriptFileViewModel>();
                string[] laKey = this.EntityIndex.GetSortedKeyList();
                for (int lnK = 0; lnK < laKey.Length; lnK++)
                {
                    MaxScriptFileViewModel loViewModel = new MaxScriptFileViewModel(this.EntityIndex[laKey[lnK]] as MaxScriptFileEntity);
                    loViewModel.Load();
                    string lsKey = loViewModel.Name.ToLowerInvariant();
                    
                    if (loSortedList.ContainsKey(lsKey))
                    {
                        DateTime ldCheck = MaxConvertLibrary.ConvertToDateTime(typeof(object), loViewModel.CreatedDate);
                        DateTime ldCurrent = MaxConvertLibrary.ConvertToDateTime(typeof(object), loSortedList[lsKey].CreatedDate);
                        if (ldCheck > ldCurrent)
                        {
                            loSortedList[lsKey] = loViewModel;
                        }
                    }
                    else
                    {
                        loSortedList.Add(lsKey, loViewModel);
                    }
                }

                this._oSortedList = new List<MaxScriptFileViewModel>(loSortedList.Values);
            }

            return this._oSortedList;
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
                MaxScriptFileEntity loEntity = this.Entity as MaxScriptFileEntity;
                if (null != loEntity)
                {
                    string lsName = this.Name.ToLowerInvariant();
                    loEntity.Name = lsName;
                    loEntity.Content = this.Content;
                    loEntity.ContentMin = this.ContentMin;
                    loEntity.ContentName = lsName;
                    loEntity.ScriptType = this.ScriptType;
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
            MaxScriptFileEntity loEntity = this.Entity as MaxScriptFileEntity;
            if (null != loEntity)
            {
                if (base.MapFromEntity())
                {
                    this.Name = loEntity.Name;
                    this.Content = loEntity.Content;
                    this.ContentMin = loEntity.ContentMin;
                    this.ContentName = loEntity.ContentName;
                    this.ScriptType = loEntity.ScriptType;
                    return true;
                }
            }

            return false;
        }

        public override bool Save()
        {
            bool lbR = false;
            this.Id = null;
            this.Entity = MaxScriptFileEntity.Create();
            lbR = base.Save();
            return lbR;
        }
    }
}
