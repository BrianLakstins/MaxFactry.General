// <copyright file="MaxUserAuthGrantViewModel.cs" company="Lakstins Family, LLC">
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
// <change date="11/3/2020" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.PresentationLayer
{
    using System;
    using MaxFactry.Core;
    using MaxFactry.Base.BusinessLayer;
    using MaxFactry.Base.PresentationLayer;
    using MaxFactry.General.BusinessLayer;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// View model base
    /// </summary>
    public class MaxUserAuthGrantViewModel : MaxBaseIdViewModel
    {
        /// <summary>
        /// Initializes a new instance of the MaxUserAuthGrantViewModel class
        /// </summary>
        public MaxUserAuthGrantViewModel()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthGrantViewModel class
        /// </summary>
        /// <param name="loEntity">Entity to use as data.</param>
        public MaxUserAuthGrantViewModel(MaxEntity loEntity)
            : base(loEntity)
        {
        }

        public MaxUserAuthGrantViewModel(string lsId) : base(lsId)
        {
        }

        protected override void CreateEntity()
        {
            this.Entity = MaxUserAuthGrantEntity.Create();
        }

        [Display(Name = "User Id")]
        public virtual string UserKey
        {
            get;
            set;
        }

        [Display(Name = "Id of User Auth")]
        public virtual string UserAuthId
        {
            get;
            set;
        }

        [Display(Name = "response_type")]
        public virtual string ResponseType
        {
            get;
            set;
        }

        [Display(Name = "access_type")]
        public virtual string AccessType
        {
            get;
            set;
        }

        [Display(Name = "approval_prompt")]
        public virtual string ApprovalPrompt
        {
            get;
            set;
        }

        [Display(Name = "include_granted_scopts")]
        public virtual string IncludeGrantedScopes
        {
            get;
            set;
        }

        [Display(Name = "prompt")]
        public virtual string Prompt
        {
            get;
            set;
        }

        [Display(Name = "response_mode")]
        public virtual string ResponseMode
        {
            get;
            set;
        }

        [Display(Name = "client_id")]
        public virtual string ClientId
        {
            get;
            set;
        }

        [Display(Name = "redirect_uri")]
        public virtual string RedirectUri
        {
            get;
            set;
        }

        [Display(Name = "scope")]
        public virtual string Scope
        {
            get;
            set;
        }

        [Display(Name = "state")]
        public virtual string State
        {
            get;
            set;
        }

        [Display(Name = "nonce")]
        public virtual string Nonce
        {
            get;
            set;
        }

        [Display(Name = "code")]
        public virtual string Code
        {
            get;
            set;
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
                MaxUserAuthGrantEntity loEntity = this.Entity as MaxUserAuthGrantEntity;
                if (null != loEntity)
                {
                    //// Updates only made to entity.  This is for viewing and approving only.
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
                MaxUserAuthGrantEntity loEntity = this.Entity as MaxUserAuthGrantEntity;
                if (null != loEntity)
                {
                    this.UserKey = loEntity.UserKey;
                    this.UserAuthId = loEntity.UserAuthId.ToString();
                    this.ResponseType = loEntity.ResponseType;
                    this.AccessType = loEntity.AccessType;
                    this.ApprovalPrompt = loEntity.ApprovalPrompt;
                    this.IncludeGrantedScopes = loEntity.IncludeGrantedScopes;
                    this.Prompt = loEntity.Prompt;
                    this.ResponseMode = loEntity.ResponseMode;
                    this.ClientId = loEntity.ClientId;
                    this.RedirectUri = loEntity.RedirectUri;
                    this.Scope = loEntity.Scope;
                    this.State = loEntity.State;
                    this.Nonce = loEntity.Nonce;
                    this.Code = loEntity.Code;
                    return true;
                }
            }

            return false;
        }
    }
}
