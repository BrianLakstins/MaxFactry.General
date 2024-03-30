// <copyright file="MaxUserAuthGrantEntity.cs" company="Lakstins Family, LLC">
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
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class. Use parent methods instead of repository.">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.BusinessLayer;
	using MaxFactry.Base.DataLayer;
    using MaxFactry.General.DataLayer;
    using MaxFactry.Base.DataLayer.Library;

    /// <summary>
    /// Entity used to manage information about UserAuthGrants for the MaxSecurityProvider.
    /// </summary>
    public class MaxUserAuthGrantEntity : MaxBaseGuidKeyEntity
	{
		/// <summary>
        /// Initializes a new instance of the MaxUserAuthGrantEntity class
		/// </summary>
        /// <param name="loData">object to hold data</param>
		public MaxUserAuthGrantEntity(MaxData loData)
			: base(loData)
		{
		}

        /// <summary>
        /// Initializes a new instance of the MaxUserAuthGrantEntity class
        /// </summary>
        /// <param name="loDataModelType">Type of data model.</param>
        public MaxUserAuthGrantEntity(Type loDataModelType)
            : base(loDataModelType)
        {
        }

        /// <summary>
        /// Gets or sets the UserKey.
        /// </summary>
		public string UserKey
        {
            get
            {
                return this.GetString(this.DataModel.UserKey);
            }

            set
            {
                this.Set(this.DataModel.UserKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the UserAuth Id
        /// </summary>
		public Guid UserAuthId
        {
            get
            {
                return this.GetGuid(this.DataModel.UserAuthId);
            }

            set
            {
                this.Set(this.DataModel.UserAuthId, value);
            }
        }

        /// <summary>
        /// Gets or sets the ResponseType.
        /// </summary>
		public string ResponseType
        {
			get
			{
				return this.GetString(this.DataModel.ResponseType);
			}

			set
			{
				this.Set(this.DataModel.ResponseType, value);
			}
		}

        public string AccessType
        {
            get
            {
                return this.GetString(this.DataModel.AccessType);
            }

            set
            {
                this.Set(this.DataModel.AccessType, value);
            }
        }

        public string ApprovalPrompt
        {
            get
            {
                return this.GetString(this.DataModel.ApprovalPrompt);
            }

            set
            {
                this.Set(this.DataModel.ApprovalPrompt, value);
            }
        }

        public string IncludeGrantedScopes
        {
            get
            {
                return this.GetString(this.DataModel.IncludeGrantedScopes);
            }

            set
            {
                this.Set(this.DataModel.IncludeGrantedScopes, value);
            }
        }

        public string Prompt
        {
            get
            {
                return this.GetString(this.DataModel.Prompt);
            }

            set
            {
                this.Set(this.DataModel.Prompt, value);
            }
        }

        public string ResponseMode
        {
            get
            {
                return this.GetString(this.DataModel.ResponseMode);
            }

            set
            {
                this.Set(this.DataModel.ResponseMode, value);
            }
        }

        public string ClientId
        {
			get
			{
				return this.GetString(this.DataModel.ClientId);
			}

			set
			{
				this.Set(this.DataModel.ClientId, value);
			}
		}

        public string RedirectUri
        {
            get
            {
                return this.GetString(this.DataModel.RedirectUri);
            }

            set
            {
                this.Set(this.DataModel.RedirectUri, value);
            }
        }

        public string Scope
        {
            get
            {
                return this.GetString(this.DataModel.Scope);
            }

            set
            {
                this.Set(this.DataModel.Scope, value);
            }
        }

        public string State
        {
            get
            {
                return this.GetString(this.DataModel.State);
            }

            set
            {
                this.Set(this.DataModel.State, value);
            }
        }

        public string Nonce
        {
            get
            {
                return this.GetString(this.DataModel.Nonce);
            }

            set
            {
                this.Set(this.DataModel.Nonce, value);
            }
        }

        public string Code
        {
            get
            {
                return MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), this.Id);
            }
        }

        public string FullUri
        {
            get
            {
                return this.GetString(this.DataModel.FullUri);
            }

            set
            {
                this.Set(this.DataModel.FullUri, value);
            }
        }

        /// <summary>
        /// Gets the Data Model for this entity
        /// </summary>
        protected MaxUserAuthGrantDataModel DataModel
        {
            get
            {
                return (MaxUserAuthGrantDataModel)MaxDataLibrary.GetDataModel(this.DataModelType);
            }
        }

        /// <summary>
        /// Creates a new entity initialized with the data.
        /// </summary>
        /// <returns>A new entity.</returns>
        public static MaxUserAuthGrantEntity Create()
        {
            return MaxBusinessLibrary.GetEntity(
                typeof(MaxUserAuthGrantEntity),
                typeof(MaxUserAuthGrantDataModel)) as MaxUserAuthGrantEntity;
        }

        public MaxEntityList LoadAllByUserKeyCache(string lsUserKey)
        {
            return this.LoadAllByPropertyCache(this.DataModel.UserKey, lsUserKey);
        }
	}
}
