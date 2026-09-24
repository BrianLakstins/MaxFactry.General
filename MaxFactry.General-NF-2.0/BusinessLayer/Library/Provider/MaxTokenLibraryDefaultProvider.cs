// <copyright file="MaxTokenLibraryDefaultProvider.cs" company="Lakstins Family, LLC">
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
// <change date="9/24/2026" author="Brian A. Lakstins" description="Initial creation">
// </changelog>
#endregion

namespace MaxFactry.General.BusinessLayer.Provider
{
    using MaxFactry.Core;
    using System;
    using System.Text.RegularExpressions;

    /// <summary>
    /// Provides methods to handle business layer security
    /// </summary>
    public class MaxTokenLibraryDefaultProvider : MaxProvider, IMaxTokenLibraryProvider
    {
        public virtual MaxIndex ParseToken(string lsToken)
        {
            MaxIndex loR = new MaxIndex();
            loR.Add("TokenText", lsToken);
            Regex loJWTRegex = new Regex(@"^[A-Za-z0-9-_]+\.[A-Za-z0-9-_]+\.[A-Za-z0-9-_]*$");
            if (loJWTRegex.IsMatch(lsToken))
            {
                try
                {
                    //// Need a provider library to handle parsing JWT token
                }
                catch (Exception loE)
                {
                    MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxTokenLibrary), "ParseToken", MaxEnumGroup.LogError, "Exception parsing toaken {lsToken}", loE, lsToken));
                }
            }
            else
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxTokenLibrary), "ParseToken", MaxEnumGroup.LogStatic, "Token does not match JWT format {lsIdToken}", lsToken));
            }

            return loR;
        }

        public virtual bool IsValidToken(MaxIndex loToken)
        {
            bool lbR = false;
            try
            {
                //// Need a provider library to handle validating token
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxTokenLibrary), "IsValidToken", MaxEnumGroup.LogError, "Error validating token {loToken}", loE, loToken));
            }

            return lbR;
        }

        public virtual bool ValidateTokenSignature(MaxIndex loToken)
        {
            bool lbR = false;
            try
            {
                //// Need a provider library to handle validating token signature
            }
            catch (Exception loE)
            {
                MaxLogLibrary.Log(new MaxLogEntryStructure(typeof(MaxTokenLibrary), "ValidateTokenSignature", MaxEnumGroup.LogError, "Error validating token signature {loToken}", loE, loToken));
            }

            return lbR;
        }

        public virtual string GetUserName(MaxIndex loToken)
        {
            Guid loObjectId = new Guid(loToken["oid"] as string);
            //// The immutable identifier for an object, in this case, a user account. 
            ////  This ID uniquely identifies the user across applications - two different applications signing in the same user receives the same value in the oid claim. 
            ////  Microsoft Graph returns this ID as the id property for a user account. 
            ////  Because the oid allows multiple apps to correlate users, the profile scope is required to receive this claim. 
            ////  If a single user exists in multiple tenants, the user contains a different object ID in each tenant - they're considered different accounts, even though the user logs into each account with the same credentials.
            ////  The oid claim is a GUID and can't be reused.
            string lsObjectId = MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), loObjectId);
            string lsEmail = GetEmail(loToken);
            string lsR = lsEmail.Replace("@", "+OAuth2" + lsObjectId + "@");
            return lsR;
        }

        public virtual string GetEmail(MaxIndex loToken)
        {
            string lsR = string.Empty;
            string lsEmail = string.Empty;
            if (loToken.Contains("unique_name"))
            {
                lsEmail = loToken["unique_name"] as string;
            }
            else if (loToken.Contains("email"))
            {
                lsEmail = loToken["email"] as string;
            }
            else if (loToken.Contains("preferred_username"))
            {
                lsEmail = loToken["preferred_username"] as string;
            }

            if (MaxEmailEntity.IsValidEmail(lsEmail))
            {
                lsR = lsEmail;
            }

            return lsR;
        }

    }
}
