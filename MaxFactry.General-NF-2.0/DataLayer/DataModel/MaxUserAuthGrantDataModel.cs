// <copyright file="MaxUserAuthGrantDataModel.cs" company="Lakstins Family, LLC">
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

namespace MaxFactry.General.DataLayer
{
	using System;
	using MaxFactry.Core;
	using MaxFactry.Base.DataLayer;

	/// <summary>
	/// Data model for the Auth Grant process
	/// </summary>
    public class MaxUserAuthGrantDataModel : MaxBaseIdDataModel
	{
		/// <summary>
		/// Key of the user associated with this grant
		/// </summary>
		public readonly string UserKey = "UserKey";

		/// <summary>
		/// Id of User Auth associated with this grant
		/// </summary>
		public readonly string UserAuthId = "UserAuthId";

		/// <summary>
		/// Type of response expected for this Grant
		/// </summary>
		public readonly string ResponseType = "ResponseType";

		/// <summary>
		/// Indicates whether your application can refresh access tokens when the user is not present at the browser. Valid parameter values are online, which is the default value, and offline.
		/// access_type=online
		/// access_type=offline
		/// </summary>
		public readonly string AccessType = "AccessType";

		/// <summary>
		/// When access_type=online you are also allowed to specify a value for approval_prompt. If it is set to approval_prompt=force, your user will always be prompted, even if they have already granted.
		/// approval_prompt=force
		/// </summary>
		public readonly string ApprovalPrompt = "ApprovalPrompt";

		/// <summary>
		/// Enables applications to use incremental authorization to request access to additional scopes in context. 
		/// If you set this parameter's value to true and the authorization request is granted, then the new access token will also cover any scopes to which the user previously granted the application access.
		/// include_granted_scopes=ture
		/// </summary>
		public readonly string IncludeGrantedScopes = "IncludeGrantedScopes";

		/// <summary>
		/// A space-delimited, case-sensitive list of prompts to present the user. If you don't specify this parameter, the user will be prompted only the first time your project requests access.
		/// prompt=none
		/// prompt=consent
		/// prompt=select_account
		/// </summary>
		public readonly string Prompt = "Prompt";

		/// <summary>
		/// Mode of response expected for this Grant
		/// https://auth0.com/docs/protocols/protocol-oauth2
		/// https://openid.net/specs/oauth-v2-multiple-response-types-1_0.html
		/// </summary>
		public readonly string ResponseMode = "ResponseMode";

		/// <summary>
		/// The client ID for your application.
		/// client_id
		/// </summary>
		public readonly string ClientId = "ClientId";

		/// <summary>
		/// Determines where the API server redirects the user after the user completes the authorization flow.
		/// redirect_uri=https://somewebpage.com/oath2
		/// </summary>
		public readonly string RedirectUri = "RedirectUri";

		/// <summary>
		/// A space-delimited list of scopes that identify the resources that your application could access on the user's behalf.
		/// scope=profile email
		/// </summary>
		public readonly string Scope = "Scope";

		/// <summary>
		/// Specifies any string value that your application uses to maintain state between your authorization request and the authorization server's response. 
		/// The server returns the exact value that you send as a name=value pair in the URL query component (?) of the redirect_uri after the user consents to or denies your application's access request.
		/// state=randomtext
		/// </summary>
		public readonly string State = "State";

		/// <summary>
		/// Nonce number passed to only be allowed once
		/// </summary>
		public readonly string Nonce = "Nonce";

		/// <summary>
		/// Full Uri used to request grant
		/// </summary>
		public readonly string FullUri = "FullUri";

		/// <summary>
		/// Initializes a new instance of the MaxUserAuthGrantDataModel class.
		/// </summary>
		public MaxUserAuthGrantDataModel() : base()
		{
            this.SetDataStorageName("MaxSecurityUserAuthGrant");
			this.AddNullable(this.UserKey, typeof(string));
			this.AddNullable(this.UserAuthId, typeof(Guid));
			this.AddType(this.ResponseType, typeof(string));
			this.AddNullable(this.AccessType, typeof(string));
			this.AddNullable(this.ApprovalPrompt, typeof(string));
			this.AddNullable(this.IncludeGrantedScopes, typeof(string));
			this.AddNullable(this.Prompt, typeof(string));
			this.AddNullable(this.ResponseMode, typeof(string));
			this.AddType(this.ClientId, typeof(string));
			this.AddType(this.RedirectUri, typeof(string));
			this.AddNullable(this.Scope, typeof(MaxLongString));
			this.AddNullable(this.State, typeof(MaxLongString));
			this.AddNullable(this.Nonce, typeof(MaxLongString));
			this.AddType(this.FullUri, typeof(MaxLongString));

			this.RepositoryProviderType = typeof(MaxFactry.General.DataLayer.Provider.MaxSecurityRepositoryDefaultProvider);
            this.RepositoryType = typeof(MaxSecurityRepository);
		}
	}
}
