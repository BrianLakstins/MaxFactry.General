// <copyright file="MaxMembershipProvider.cs" company="Lakstins Family, LLC">
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
// <change date="2/22/2014" author="Brian A. Lakstins" description="Initial Release">
// <change date="2/23/2014" author="Brian A. Lakstins" description="Moved getting a Guid from the Key to a library.">
// <change date="4/15/2014" author="Brian A. Lakstins" description="Differentiated exact and partial username and email matches.">
// <change date="5/6/2014" author="Brian A. Lakstins" description="Update to allow admins to reset password by sending null answer to question.">
// <change date="5/15/2014" author="Brian A. Lakstins" description="Remove local storage of application name and application Id.">
// <change date="6/27/2014" author="Brian A. Lakstins" description="Remove dependency on AppId.">
// <change date="10/17/2014" author="Brian A. Lakstins" description="Updated to not keep config in memory.">
// <change date="12/18/2014" author="Brian A. Lakstins" description="Updates to follow core data access patterns.">
// <change date="11/17/2020" author="Brian A. Lakstins" description="Move sign in and sign out to use library with a provider.">
// <change date="11/25/2020" author="Brian A. Lakstins" description="Handle users that don't have a password yet.">
// <change date="12/19/2020" author="Brian A. Lakstins" description="Handle multiple users sharing an email address so none are returned when looking for username by email">
// <change date="6/9/2021" author="Brian A. Lakstins" description="Only log user being online once per 5 minutes">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Add user related logging.  Updates to removal of an unneeded method.">
// </changelog>
#endregion

namespace System.Web.Security
{
	using System;
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;
	using MaxFactry.Core;
    using MaxFactry.Base.DataLayer;
	using MaxFactry.Base.BusinessLayer;
    using MaxFactry.General.BusinessLayer;

	/// <summary>
	/// Provide membership services using MaxFactry Framework.
	/// </summary>
	public class MaxMembershipProviderOverride : MembershipProvider
	{
		/// <summary>
		/// Gets or sets the name of the application.
		/// </summary>
		public override string ApplicationName
		{
			get
			{
                return this.Config.ApplicationName;
			}

			set
			{
                this.Config.ApplicationName = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the current membership provider is configured to allow users to retrieve their passwords.
		/// </summary>
		public override bool EnablePasswordRetrieval 
		{
			get
			{
				return this.Config.EnablePasswordRetrieval;
			}
		}
		
		/// <summary>
		/// Gets a value indicating whether the current membership provider is configured to allow users to reset their passwords.
		/// </summary>
		public override bool EnablePasswordReset
		{
			get
			{
				return this.Config.EnablePasswordReset;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the default membership provider requires the user to answer a password question for password reset and retrieval.
		/// </summary>
		public override bool RequiresQuestionAndAnswer
		{
			get
			{
				return this.Config.RequiresQuestionAndAnswer;
			}
		}
		
		/// <summary>
		/// Gets the number of invalid password or password-answer attempts allowed before the membership user is locked out.
		/// </summary>
		public override int MaxInvalidPasswordAttempts
		{
			get
			{
				return this.Config.MaxInvalidPasswordAttempts;
			}
		}

		/// <summary>
		/// Gets the time window between which consecutive failed attempts to provide a valid password or password answer are tracked.
		/// </summary>
		public override int PasswordAttemptWindow
		{
			get
			{
				return this.Config.PasswordAttemptWindow;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the membership provider is configured to require a unique e-mail address for each user name.
		/// </summary>
		public override bool RequiresUniqueEmail
		{
			get
			{
				return this.Config.RequiresUniqueEmail;
			}
		}

		/// <summary>
		/// Gets a value indicating the format for storing passwords in the membership data store.
		/// </summary>
		public override MembershipPasswordFormat PasswordFormat
		{
			get
			{
				int lnPasswordFormat = this.Config.MembershipPasswordFormat;
				if (lnPasswordFormat.Equals(MaxUserPasswordEntity.MembershipPasswordFormatClear))
				{
					return MembershipPasswordFormat.Clear;
				}
				else if (lnPasswordFormat.Equals(MaxUserPasswordEntity.MembershipPasswordFormatEncrypted))
				{
					return MembershipPasswordFormat.Encrypted;
				}
				else 
				{
					return MembershipPasswordFormat.Hashed;
				}
			}
		}

		/// <summary>
		/// Gets the minimum length required for a password.
		/// </summary>
		public override int MinRequiredPasswordLength
		{
			get
			{
				return this.Config.MinRequiredPasswordLength;
			}
		}
		
		/// <summary>
		/// Gets the minimum number of special characters that must be present in a valid password.
		/// </summary>
		public override int MinRequiredNonAlphanumericCharacters
		{
			get
			{
				return this.Config.MinRequiredNonAlphanumericCharacters;
			}
		}

		/// <summary>
		/// Gets the regular expression used to evaluate a password.
		/// </summary>
		public override string PasswordStrengthRegularExpression
		{
			get
			{
				return this.Config.PasswordStrengthRegularExpression;
			}
		}

        /// <summary>
        /// Gets the configuration information.
        /// </summary>
        protected MaxUserConfigurationEntity Config
        {
            get
            {
                return MaxUserConfigurationEntity.Create().GetCurrent();
            }
        }

		/// <summary>
		/// Adds a new user to the data store.
		/// </summary>
		/// <param name="username">The user name for the new user.</param>
		/// <param name="password">The password for the new user.</param>
		/// <param name="email">The e-mail address for the new user.</param>
		/// <param name="passwordQuestion">The password-question value for the membership user.</param>
		/// <param name="passwordAnswer">The password-answer value for the membership user.</param>
		/// <param name="isApproved">A Boolean that indicates whether the new user is approved to log on.</param>
		/// <param name="providerUserKey">The user identifier for the user that should be stored in the membership data store.</param>
		/// <param name="status">A MembershipCreateStatus indicating that the user was created successfully or the reason creation failed.</param>
		/// <returns>A MembershipUser object for the newly created user. If no user was created, this method returns a null reference (Nothing in Visual Basic).</returns>
        public override MembershipUser CreateUser(
            string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            object providerUserKey,
            out MembershipCreateStatus status)
        {
            status = MembershipCreateStatus.ProviderError;

            status = this.CheckUserData(providerUserKey, username, email, out Guid loKey);
            if (status != MembershipCreateStatus.Success)
            {
                return null;
            }

            status = this.CheckPasswordData(password, passwordQuestion, passwordAnswer);
            if (status != MembershipCreateStatus.Success)
            {
                return null;
            }

            ValidatePasswordEventArgs loEvent = new ValidatePasswordEventArgs(username, password, true);
            this.OnValidatingPassword(loEvent);

            if (loEvent.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            MaxUserEntity loMaxUserNew = MaxUserEntity.Create();
            loMaxUserNew.UserName = username;
            loMaxUserNew.Email = email;
            if (isApproved)
            {
                loMaxUserNew.IsActive = true;
            }

            if (loMaxUserNew.Insert())
            {
                MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create();
                loMaxPassword.PasswordQuestion = passwordQuestion;
                loMaxPassword.PasswordAnswer = passwordAnswer;
                if (loMaxPassword.Insert(loMaxUserNew.Id, this.GetMaxPasswordFormat(), password))
                {
                    return this.GetUserDetails(loMaxUserNew);
                }
            }

            return null;
        }

		/// <summary>
		/// Processes a request to update the password question and answer for a membership user.
		/// </summary>
		/// <param name="username">The user to change the password question and answer for.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <param name="newPasswordQuestion">The new password question for the specified user.</param>
		/// <param name="newPasswordAnswer">The new password answer for the specified user.</param>
		/// <returns>true if the password question and answer are updated successfully; otherwise, false.</returns>
		public override bool ChangePasswordQuestionAndAnswer(
			string username,
			string password,
			string newPasswordQuestion, 
			string newPasswordAnswer)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
			MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create().GetLatestByUserId(loMaxUser.Id);
			if (null == loMaxPassword || loMaxPassword.CheckPassword(password))
			{
				if (this.CheckPasswordData(password, newPasswordQuestion, newPasswordAnswer) == MembershipCreateStatus.Success)
				{
                    MaxUserPasswordEntity loMaxPasswordNew = MaxUserPasswordEntity.Create();
                    loMaxPasswordNew.PasswordQuestion = newPasswordQuestion;
                    loMaxPasswordNew.PasswordAnswer = newPasswordAnswer;
                    if (loMaxPasswordNew.Insert(loMaxUser.Id, this.GetMaxPasswordFormat(), password))
                    {
                        MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                        loMaxUserLog.Insert(
                            loMaxUser.Id,
                            MaxUserLogEntity.LogEntryTypePasswordChange,
                            this.GetType() + ".ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) - Question and answer updated for question [" + newPasswordQuestion + "]");

                        return true;
                    }
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the password for the specified user name from the data source.
		/// </summary>
		/// <param name="username">The user to retrieve the password for.</param>
		/// <param name="answer">The password answer for the user.</param>
		/// <returns>The password for the specified user name.</returns>
		public override string GetPassword(string username,	string answer)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
			MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create().GetLatestByUserId(loMaxUser.Id);
            if (null != loMaxPassword)
            {
                if (!this.EnablePasswordRetrieval)
                {
                    throw new MaxException("Password retrieval is disabled");
                }
                else if (loMaxPassword.PasswordAnswer != answer)
                {
                    throw new MaxException("Answer does not match");
                }

                return loMaxPassword.GetPassword();
            }

            return string.Empty;
        }

		/// <summary>
		/// Processes a request to update the password for a membership user.
		/// </summary>
		/// <param name="username">The user to update the password for.</param>
		/// <param name="oldPassword">The current password for the specified user.</param>
		/// <param name="newPassword">The new password for the specified user.</param>
		/// <returns>true if the password was updated successfully; otherwise, false.</returns>
		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
			MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create().GetLatestByUserId(loMaxUser.Id);
			if (null == loMaxPassword || loMaxPassword.CheckPassword(oldPassword))
			{
                string lsPasswordQuestion = string.Empty;
                string lsPasswordAnswer = string.Empty;
                if (null != loMaxPassword)
                {
                    lsPasswordQuestion = loMaxPassword.PasswordQuestion;
                    lsPasswordAnswer = loMaxPassword.PasswordAnswer;
                }

				MembershipCreateStatus loStatus = this.CheckPasswordData(newPassword, lsPasswordQuestion, lsPasswordAnswer);
				if (loStatus == MembershipCreateStatus.Success)
				{
					ValidatePasswordEventArgs loEvent = new ValidatePasswordEventArgs(username, newPassword, false);
					this.OnValidatingPassword(loEvent);
					if (loEvent.Cancel)
					{
						if (loEvent.FailureInformation != null)
						{
							throw loEvent.FailureInformation;
						}
						else
						{
							throw new ArgumentException("The custom password validation failed.");
						}
					}

					MaxUserPasswordEntity loMaxPasswordNew = MaxUserPasswordEntity.Create();
                    loMaxPasswordNew.PasswordQuestion = lsPasswordQuestion;
					loMaxPasswordNew.PasswordAnswer = lsPasswordAnswer;
                    if (loMaxPasswordNew.Insert(loMaxUser.Id, this.GetMaxPasswordFormat(), newPassword))
                    {
                        if (loMaxUser.IsPasswordResetNeeded)
                        {
                            loMaxUser.IsPasswordResetNeeded = false;
                            loMaxUser.Update();
                        }

                        MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                        loMaxUserLog.Insert(
                            loMaxUser.Id,
                            MaxUserLogEntity.LogEntryTypePasswordChange,
                            this.GetType() + ".ChangePassword(string username, string oldPassword, string newPassword)");
                        return true;
                    }
				}
			}

			return false;
		}

		/// <summary>
		/// Resets a user's password to a new, automatically generated password.
		/// </summary>
		/// <param name="username">The user to reset the password for.</param>
		/// <param name="answer">The password answer for the specified user.</param>
		/// <returns>The new password for the specified user.</returns>
		public override string ResetPassword(string username, string answer)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
			MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create().GetLatestByUserId(loMaxUser.Id);
			if (null != answer && null != loMaxPassword && loMaxPassword.PasswordAnswer != answer)
			{
                throw new MaxException("Answer does not match");
			}

            string lsPassword = MaxFactry.Core.MaxConvertLibrary.ConvertGuidToAlphabet64(typeof(object), Guid.NewGuid());
            string lsPasswordQuestion = string.Empty;
            string lsPasswordAnswer = string.Empty;
            if (null != loMaxPassword)
            {
                lsPasswordQuestion = loMaxPassword.PasswordQuestion;
                lsPasswordAnswer = loMaxPassword.PasswordAnswer;
            }

			MembershipCreateStatus loStatus = this.CheckPasswordData(lsPassword, lsPasswordQuestion, lsPasswordAnswer);
			if (loStatus == MembershipCreateStatus.Success)
			{
				ValidatePasswordEventArgs loEvent = new ValidatePasswordEventArgs(username, lsPassword, false);
				this.OnValidatingPassword(loEvent);

				if (loEvent.Cancel)
				{
					if (loEvent.FailureInformation != null)
					{
						throw loEvent.FailureInformation;
					}
					else
					{
						throw new ArgumentException("The custom password validation failed.");
					}
				}

				MaxUserPasswordEntity loMaxPasswordNew = MaxUserPasswordEntity.Create();
                loMaxPasswordNew.PasswordQuestion = lsPasswordQuestion;
				loMaxPasswordNew.PasswordAnswer = lsPasswordAnswer;
                if (loMaxPasswordNew.Insert(loMaxUser.Id, this.GetMaxPasswordFormat(), lsPassword)) 
                {
                    if (loMaxUser.IsPasswordResetNeeded)
                    {
                        loMaxUser.IsPasswordResetNeeded = false;
                        loMaxUser.Update();
                    }

                    MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                    loMaxUserLog.Insert(
                        loMaxUser.Id,
                        MaxUserLogEntity.LogEntryTypePasswordChange,
                        this.GetType() + ".ResetPassword(string username, string answer)");
                    return lsPassword;
                }
			}

			return string.Empty;
		}

		/// <summary>
		/// Updates the database with the information for the specified user.
		/// </summary>
		/// <param name="user">A MembershipUser object that represents the user to be updated and the updated information for the user.</param>
		public override void UpdateUser(MembershipUser user)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(user.ProviderUserKey);
            if (null == loMaxUser)
            {
                throw new MaxException("User with Key [" + user.ProviderUserKey.ToString() + "] cannot be found.");
            }

            if (loMaxUser.Email != user.Email)
            {
                if (!this.IsValidEmail(user.Email))
                {
                    throw new MaxException("Email of [" + user.Email + "] is not valid.");
                }
            }

            MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
			loMaxUserLog.Insert(
                loMaxUser.Id,
                MaxUserLogEntity.LogEntryTypeUserChange,
                this.GetType() + ".UpdateUser(MembershipUser user)");

			loMaxUser.Comment = user.Comment;
			loMaxUser.Email = user.Email;
			loMaxUser.IsActive = user.IsApproved;
            if (user is MaxMembershipUser)
            {
                if (loMaxUser.IsPasswordResetNeeded != ((MaxMembershipUser)user).IsPasswordResetNeeded)
                {
                    loMaxUser.IsPasswordResetNeeded = ((MaxMembershipUser)user).IsPasswordResetNeeded;
                }
            }

			loMaxUser.Update();	
		}

		/// <summary>
		/// Verifies that the supplied user name and password are valid.
		/// </summary>
		/// <param name="username">The name of the user to be validated.</param>
		/// <param name="password">The password for the specified user.</param>
		/// <returns>true if the supplied user name and password are valid; otherwise, false.</returns>
		public override bool ValidateUser(string username, string password)
		{
            bool lbR = false;
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
            if (null != loMaxUser)
            {
                MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create().GetLatestByUserId(loMaxUser.Id);
                if (null != loMaxPassword)
                {
                    if (!string.IsNullOrEmpty(loMaxPassword.Password))
                    {
                        lbR = loMaxPassword.CheckPassword(password);
                    }
                    else
                    {
                        lbR = loMaxUser.ValidateUserExternal(password);
                    }
                }
            }
            else
            {
                loMaxUser = MaxUserEntity.Create();
                lbR = loMaxUser.ValidateUserExternal(username, password);
            }

            MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
            if (lbR)
            {
                loMaxUserLog.Insert(
                    loMaxUser.Id,
                    MaxUserLogEntity.LogEntryTypeLogin,
                    this.GetType() + ".ValidateUser(string username, string password) - succeeded");
            }

            return lbR;
		}

		/// <summary>
		/// Clears a lock so that the membership user can be validated.
		/// </summary>
		/// <param name="userName">The membership user whose lock status you want to clear.</param>
		/// <returns>true if the membership user was successfully unlocked; otherwise, false.</returns>
		public override bool UnlockUser(string userName)
		{
			try
			{
				MaxUserEntity loMaxUser = this.GetMaxUser(userName);
                MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
				loMaxUserLog.Insert(
                    loMaxUser.Id,
                    MaxUserLogEntity.LogEntryTypeUnlockout,
                    this.GetType() + ".UnlockUser(string userName)");
			}
			catch (Exception loE)
			{
                MaxException loMaxException = new MaxException("Error unlocking user", loE);
				return false;
			}

			return true;
		}

		/// <summary>
		/// Gets the information from the data source for the membership user associated with the specified unique identifier. Updates the last-activity date/time stamp for the user, if specified.
		/// </summary>
		/// <param name="providerUserKey">The unique user identifier from the membership data source for the user.</param>
		/// <param name="userIsOnline">If true, updates the last-activity date/time stamp for the specified user.</param>
		/// <returns>A MembershipUser object representing the user associated with the specified unique identifier.</returns>
		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(providerUserKey);
			if (null != loMaxUser)
			{
                if (userIsOnline)
                {
                    string lsKey = loMaxUser.Id.ToString();
                    string lsLastLog = MaxCacheRepository.Get(this.GetType(), lsKey, typeof(string)) as string;
                    DateTime ldLastLog = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(lsLastLog))
                    {
                        ldLastLog = new DateTime(Convert.ToInt64(lsLastLog));
                    }

                    if (DateTime.UtcNow > ldLastLog.AddMinutes(5))
                    {
                        MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                        loMaxUserLog.Insert(
                            loMaxUser.Id,
                            MaxUserLogEntity.LogEntryTypeActivity,
                            this.GetType() + ".GetUser(object providerUserKey, bool userIsOnline)");
                        MaxCacheRepository.Set(this.GetType(), lsKey, DateTime.UtcNow.Ticks.ToString());
                    }
                }

				return this.GetUserDetails(loMaxUser);
			}

			return null;
		}

		/// <summary>
		/// Gets the information from the data source for the specified membership user. Updates the last-activity date/time stamp for the user, if specified.
		/// </summary>
		/// <param name="username">The name of the user to retrieve.</param>
		/// <param name="userIsOnline">If true, updates the last-activity date/time stamp for the specified user.</param>
		/// <returns>A MembershipUser object representing the specified user.</returns>
		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
			if (null != loMaxUser)
			{
				if (userIsOnline)
				{
                    string lsKey = this.GetType().ToString() + "/GetUser/" + loMaxUser.Id.ToString();
                    string lsLastLog = MaxCacheRepository.Get(this.GetType(), lsKey, typeof(string)) as string;
                    DateTime ldLastLog = DateTime.MinValue;
                    if (!string.IsNullOrEmpty(lsLastLog))
                    {
                        ldLastLog = new DateTime(Convert.ToInt64(lsLastLog), DateTimeKind.Utc);
                    }

                    if (DateTime.UtcNow > ldLastLog.AddMinutes(5))
                    { 
                        MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                        loMaxUserLog.Insert(
                            loMaxUser.Id,
                            MaxUserLogEntity.LogEntryTypeActivity,
                            this.GetType() + ".GetUser(string username, bool userIsOnline)");
                        MaxCacheRepository.Set(this.GetType(), lsKey, DateTime.UtcNow.Ticks.ToString());
                    }
				}

				return this.GetUserDetails(loMaxUser);
			}

			return null;
		}

		/// <summary>
		/// Gets a user name where the e-mail address for the user matches the specified e-mail address.
		/// </summary>
		/// <param name="email">The e-mail address to search for.</param>
		/// <returns>The user name where the e-mail address for the user matches the specified e-mail address. If no match is found, a null reference (Nothing in Visual Basic) is returned.</returns>
		public override string GetUserNameByEmail(string lsEmail)
		{
			if (this.RequiresUniqueEmail)
			{
				try
				{
					MaxEntityList loList = MaxUserEntity.Create().LoadAllByEmailCache(lsEmail);
                    int lnMatchCount = 0;
                    string lsUserName = null;
					for (int lnL = 0; lnL < loList.Count; lnL++)
					{
						if (((MaxUserEntity)loList[lnL]).Email.Equals(lsEmail, StringComparison.InvariantCultureIgnoreCase))
						{
                            lsUserName = ((MaxUserEntity)loList[lnL]).UserName;
                            lnMatchCount++;
						}
					}

                    if (lnMatchCount == 1)
                    {
                        return lsUserName;
                    }
                    else
                    {
                        MaxLogLibrary.Log(new MaxLogEntryStructure("GetUserNameByEmail", MaxEnumGroup.LogError, "Error getting user name by email.  There were {lnMatchCount} users with this email {email}", lnMatchCount, lsEmail));
                    }
				}
				catch (Exception loE)
				{
                    MaxLogLibrary.Log(new MaxLogEntryStructure("GetUserNameByEmail", MaxEnumGroup.LogError, "Error getting user name by email", loE));
					return null;
				}
			}

			return null;
		}

		/// <summary>
		/// Removes a user from the membership data source.
		/// </summary>
		/// <param name="username">The name of the user to delete.</param>
		/// <param name="deleteAllRelatedData">true to delete data related to the user from the database; false to leave data related to the user in the database.</param>
		/// <returns>true if the user was deleted; otherwise, false.</returns>
		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			MaxUserEntity loMaxUser = this.GetMaxUser(username);
			if (deleteAllRelatedData)
			{
				//// Other data doesn't really need deleted...
			}

            MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
			loMaxUserLog.Insert(
                loMaxUser.Id,
                MaxUserLogEntity.LogEntryTypeUserDelete,
                this.GetType() + ".DeleteUser(string username, bool deleteAllRelatedData)");

            loMaxUser.Delete();
			return true;
		}

        /// <summary>
        /// Gets a collection of all the users in the data source in pages of data.
        /// </summary>
        /// <param name="pageIndex">The index of the page of results to return. Use 0 to indicate the first page.</param>
        /// <param name="pageSize">The size of the page of results to return. pageIndex is zero-based.</param>
        /// <param name="totalRecords">The total number of users.</param>
        /// <returns>A MembershipUserCollection of MembershipUser objects representing all the users in the database for the configured applicationName.</returns>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
            MaxEntityList loList = MaxUserEntity.Create().LoadAllByPage(pageIndex, pageSize, string.Empty);
            totalRecords = loList.Total;
            MembershipUserCollection loCollection = new MembershipUserCollection();
			for (int lnL = 0; lnL < loList.Count; lnL++)
			{
				loCollection.Add(this.GetUser((MaxUserEntity)loList[lnL]));
			}

			return loCollection;
		}

		/// <summary>
		/// Gets the number of users currently accessing an application.
		/// </summary>
		/// <returns>The number of users currently accessing an application.</returns>
		public override int GetNumberOfUsersOnline()
		{
            return MaxUserLogEntity.Create().GetCountActivityByCreatedDate(DateTime.Now.AddMinutes(-1 * this.Config.OnlineWindowDuration));
		}

		/// <summary>
		/// Gets a collection of membership users where the user name contains the specified user name to match.
		/// </summary>
		/// <param name="usernameToMatch">The user name to search for.</param>
		/// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>A MembershipUserCollection collection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
            MaxEntityList loList = MaxUserEntity.Create().LoadAllByUsernamePartial(usernameToMatch, pageIndex, pageSize, string.Empty);
            totalRecords = loList.Total;
			MembershipUserCollection loCollection = new MembershipUserCollection();
			for (int lnL = 0; lnL < loList.Count; lnL++)
			{
				loCollection.Add(this.GetUser((MaxUserEntity)loList[lnL]));
			}

			return loCollection;
		}
		
		/// <summary>
		/// Gets a collection of membership users, in a page of data, where the e-mail address contains the specified e-mail address to match.
		/// </summary>
		/// <param name="emailToMatch">The e-mail address to search for.</param>
		/// <param name="pageIndex">The index of the page of results to return. pageIndex is zero-based.</param>
		/// <param name="pageSize">The size of the page of results to return.</param>
		/// <param name="totalRecords">The total number of matched users.</param>
		/// <returns>A MembershipUserCollection that contains a page of pageSizeMembershipUser objects beginning at the page specified by pageIndex.</returns>
		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
            MaxEntityList loList = MaxUserEntity.Create().LoadAllByEmailPartial(emailToMatch, pageIndex, pageSize, string.Empty);
            totalRecords = loList.Total;
			MembershipUserCollection loCollection = new MembershipUserCollection();
			for (int lnL = 0; lnL < loList.Count; lnL++)
			{
				loCollection.Add(this.GetUser((MaxUserEntity)loList[lnL]));
			}

			return loCollection;
		}

        /// <summary>
        /// Checks to make sure there is not another user with this same username.
        /// </summary>
        /// <param name="loId">Id of the user being checked.</param>
        /// <param name="lsUserName">username to check for duplicates.</param>
        /// <returns>true if there is a user with a different id and this username, false otherwise.</returns>
        protected bool IsDuplicateUserName(Guid loId, string lsUserName)
        {
            MaxEntityList loUserEntityList = MaxUserEntity.Create().LoadAllByUsernameCache(lsUserName);
            if (loUserEntityList.Count > 0)
            {
                for (int lnU = 0; lnU < loUserEntityList.Count; lnU++)
                {
                    MaxUserEntity loUserEntity = (MaxUserEntity)loUserEntityList[lnU];
                    if (loUserEntity.Id != loId && loUserEntity.UserName.Equals(lsUserName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to make sure there is not another user with this same email.
        /// </summary>
        /// <param name="loId">Id of the user being checked.</param>
        /// <param name="lsEmail">email to check for duplicates.</param>
        /// <returns>true if there is a user with a different id and this email, false otherwise.</returns>
        protected bool IsDuplicateEmail(Guid loId, string lsEmail)
        {
            MaxEntityList loUserEntityList = MaxUserEntity.Create().LoadAllByEmailCache(lsEmail);
            if (loUserEntityList.Count > 0)
            {
                for (int lnU = 0; lnU < loUserEntityList.Count; lnU++)
                {
                    MaxUserEntity loUserEntity = (MaxUserEntity)loUserEntityList[lnU];
                    if (loUserEntity.Id != loId && loUserEntity.Email.Equals(lsEmail, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Checks to see if the format of the email address is valid.
        /// </summary>
        /// <param name="lsEmail">The address to check.</param>
        /// <returns>true if valid.  False if not.</returns>
        protected bool IsValidEmail(string lsEmail)
        {
            if (string.IsNullOrEmpty(lsEmail))
            {
                return false;
            }
            else if (lsEmail.Length > 254)
            {
                return false;
            }
            else
            {
                Regex loRegexIsInternetEmailAddressSimple = new Regex(@"(@)(.+)$", RegexOptions.IgnoreCase);
                if (!loRegexIsInternetEmailAddressSimple.IsMatch(lsEmail))
                {
                    return false;
                }

                string lsEmailFormat = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*";
                //// http://data.iana.org/TLD/tlds-alpha-by-domain.txt
                //// # Version 2020111700, Last Updated Tue Nov 17 07:07:01 2020 UTC
                string lsTLDList = "(?:AAA|AARP|ABARTH|ABB|ABBOTT|ABBVIE|ABC|ABLE|ABOGADO|ABUDHABI|AC|ACADEMY|ACCENTURE|ACCOUNTANT|ACCOUNTANTS|ACO|ACTOR|AD|ADAC|ADS|ADULT|AE|AEG|AERO|AETNA|AF|AFAMILYCOMPANY|AFL|AFRICA|AG|AGAKHAN|AGENCY|AI|AIG|AIRBUS|AIRFORCE|AIRTEL|AKDN|AL|ALFAROMEO|ALIBABA|ALIPAY|ALLFINANZ|ALLSTATE|ALLY|ALSACE|ALSTOM|AM|AMAZON|AMERICANEXPRESS|AMERICANFAMILY|AMEX|AMFAM|AMICA|AMSTERDAM|ANALYTICS|ANDROID|ANQUAN|ANZ|AO|AOL|APARTMENTS|APP|APPLE|AQ|AQUARELLE|AR|ARAB|ARAMCO|ARCHI|ARMY|ARPA|ART|ARTE|AS|ASDA|ASIA|ASSOCIATES|AT|ATHLETA|ATTORNEY|AU|AUCTION|AUDI|AUDIBLE|AUDIO|AUSPOST|AUTHOR|AUTO|AUTOS|AVIANCA|AW|AWS|AX|AXA|AZ|AZURE|BA|BABY|BAIDU|BANAMEX|BANANAREPUBLIC|BAND|BANK|BAR|BARCELONA|BARCLAYCARD|BARCLAYS|BAREFOOT|BARGAINS|BASEBALL|BASKETBALL|BAUHAUS|BAYERN|BB|BBC|BBT|BBVA|BCG|BCN|BD|BE|BEATS|BEAUTY|BEER|BENTLEY|BERLIN|BEST|BESTBUY|BET|BF|BG|BH|BHARTI|BI|BIBLE|BID|BIKE|BING|BINGO|BIO|BIZ|BJ|BLACK|BLACKFRIDAY|BLOCKBUSTER|BLOG|BLOOMBERG|BLUE|BM|BMS|BMW|BN|BNPPARIBAS|BO|BOATS|BOEHRINGER|BOFA|BOM|BOND|BOO|BOOK|BOOKING|BOSCH|BOSTIK|BOSTON|BOT|BOUTIQUE|BOX|BR|BRADESCO|BRIDGESTONE|BROADWAY|BROKER|BROTHER|BRUSSELS|BS|BT|BUDAPEST|BUGATTI|BUILD|BUILDERS|BUSINESS|BUY|BUZZ|BV|BW|BY|BZ|BZH|CA|CAB|CAFE|CAL|CALL|CALVINKLEIN|CAM|CAMERA|CAMP|CANCERRESEARCH|CANON|CAPETOWN|CAPITAL|CAPITALONE|CAR|CARAVAN|CARDS|CARE|CAREER|CAREERS|CARS|CASA|CASE|CASEIH|CASH|CASINO|CAT|CATERING|CATHOLIC|CBA|CBN|CBRE|CBS|CC|CD|CEB|CENTER|CEO|CERN|CF|CFA|CFD|CG|CH|CHANEL|CHANNEL|CHARITY|CHASE|CHAT|CHEAP|CHINTAI|CHRISTMAS|CHROME|CHURCH|CI|CIPRIANI|CIRCLE|CISCO|CITADEL|CITI|CITIC|CITY|CITYEATS|CK|CL|CLAIMS|CLEANING|CLICK|CLINIC|CLINIQUE|CLOTHING|CLOUD|CLUB|CLUBMED|CM|CN|CO|COACH|CODES|COFFEE|COLLEGE|COLOGNE|COM|COMCAST|COMMBANK|COMMUNITY|COMPANY|COMPARE|COMPUTER|COMSEC|CONDOS|CONSTRUCTION|CONSULTING|CONTACT|CONTRACTORS|COOKING|COOKINGCHANNEL|COOL|COOP|CORSICA|COUNTRY|COUPON|COUPONS|COURSES|CPA|CR|CREDIT|CREDITCARD|CREDITUNION|CRICKET|CROWN|CRS|CRUISE|CRUISES|CSC|CU|CUISINELLA|CV|CW|CX|CY|CYMRU|CYOU|CZ|DABUR|DAD|DANCE|DATA|DATE|DATING|DATSUN|DAY|DCLK|DDS|DE|DEAL|DEALER|DEALS|DEGREE|DELIVERY|DELL|DELOITTE|DELTA|DEMOCRAT|DENTAL|DENTIST|DESI|DESIGN|DEV|DHL|DIAMONDS|DIET|DIGITAL|DIRECT|DIRECTORY|DISCOUNT|DISCOVER|DISH|DIY|DJ|DK|DM|DNP|DO|DOCS|DOCTOR|DOG|DOMAINS|DOT|DOWNLOAD|DRIVE|DTV|DUBAI|DUCK|DUNLOP|DUPONT|DURBAN|DVAG|DVR|DZ|EARTH|EAT|EC|ECO|EDEKA|EDU|EDUCATION|EE|EG|EMAIL|EMERCK|ENERGY|ENGINEER|ENGINEERING|ENTERPRISES|EPSON|EQUIPMENT|ER|ERICSSON|ERNI|ES|ESQ|ESTATE|ET|ETISALAT|EU|EUROVISION|EUS|EVENTS|EXCHANGE|EXPERT|EXPOSED|EXPRESS|EXTRASPACE|FAGE|FAIL|FAIRWINDS|FAITH|FAMILY|FAN|FANS|FARM|FARMERS|FASHION|FAST|FEDEX|FEEDBACK|FERRARI|FERRERO|FI|FIAT|FIDELITY|FIDO|FILM|FINAL|FINANCE|FINANCIAL|FIRE|FIRESTONE|FIRMDALE|FISH|FISHING|FIT|FITNESS|FJ|FK|FLICKR|FLIGHTS|FLIR|FLORIST|FLOWERS|FLY|FM|FO|FOO|FOOD|FOODNETWORK|FOOTBALL|FORD|FOREX|FORSALE|FORUM|FOUNDATION|FOX|FR|FREE|FRESENIUS|FRL|FROGANS|FRONTDOOR|FRONTIER|FTR|FUJITSU|FUJIXEROX|FUN|FUND|FURNITURE|FUTBOL|FYI|GA|GAL|GALLERY|GALLO|GALLUP|GAME|GAMES|GAP|GARDEN|GAY|GB|GBIZ|GD|GDN|GE|GEA|GENT|GENTING|GEORGE|GF|GG|GGEE|GH|GI|GIFT|GIFTS|GIVES|GIVING|GL|GLADE|GLASS|GLE|GLOBAL|GLOBO|GM|GMAIL|GMBH|GMO|GMX|GN|GODADDY|GOLD|GOLDPOINT|GOLF|GOO|GOODYEAR|GOOG|GOOGLE|GOP|GOT|GOV|GP|GQ|GR|GRAINGER|GRAPHICS|GRATIS|GREEN|GRIPE|GROCERY|GROUP|GS|GT|GU|GUARDIAN|GUCCI|GUGE|GUIDE|GUITARS|GURU|GW|GY|HAIR|HAMBURG|HANGOUT|HAUS|HBO|HDFC|HDFCBANK|HEALTH|HEALTHCARE|HELP|HELSINKI|HERE|HERMES|HGTV|HIPHOP|HISAMITSU|HITACHI|HIV|HK|HKT|HM|HN|HOCKEY|HOLDINGS|HOLIDAY|HOMEDEPOT|HOMEGOODS|HOMES|HOMESENSE|HONDA|HORSE|HOSPITAL|HOST|HOSTING|HOT|HOTELES|HOTELS|HOTMAIL|HOUSE|HOW|HR|HSBC|HT|HU|HUGHES|HYATT|HYUNDAI|IBM|ICBC|ICE|ICU|ID|IE|IEEE|IFM|IKANO|IL|IM|IMAMAT|IMDB|IMMO|IMMOBILIEN|IN|INC|INDUSTRIES|INFINITI|INFO|ING|INK|INSTITUTE|INSURANCE|INSURE|INT|INTERNATIONAL|INTUIT|INVESTMENTS|IO|IPIRANGA|IQ|IR|IRISH|IS|ISMAILI|IST|ISTANBUL|IT|ITAU|ITV|IVECO|JAGUAR|JAVA|JCB|JCP|JE|JEEP|JETZT|JEWELRY|JIO|JLL|JM|JMP|JNJ|JO|JOBS|JOBURG|JOT|JOY|JP|JPMORGAN|JPRS|JUEGOS|JUNIPER|KAUFEN|KDDI|KE|KERRYHOTELS|KERRYLOGISTICS|KERRYPROPERTIES|KFH|KG|KH|KI|KIA|KIM|KINDER|KINDLE|KITCHEN|KIWI|KM|KN|KOELN|KOMATSU|KOSHER|KP|KPMG|KPN|KR|KRD|KRED|KUOKGROUP|KW|KY|KYOTO|KZ|LA|LACAIXA|LAMBORGHINI|LAMER|LANCASTER|LANCIA|LAND|LANDROVER|LANXESS|LASALLE|LAT|LATINO|LATROBE|LAW|LAWYER|LB|LC|LDS|LEASE|LECLERC|LEFRAK|LEGAL|LEGO|LEXUS|LGBT|LI|LIDL|LIFE|LIFEINSURANCE|LIFESTYLE|LIGHTING|LIKE|LILLY|LIMITED|LIMO|LINCOLN|LINDE|LINK|LIPSY|LIVE|LIVING|LIXIL|LK|LLC|LLP|LOAN|LOANS|LOCKER|LOCUS|LOFT|LOL|LONDON|LOTTE|LOTTO|LOVE|LPL|LPLFINANCIAL|LR|LS|LT|LTD|LTDA|LU|LUNDBECK|LUPIN|LUXE|LUXURY|LV|LY|MA|MACYS|MADRID|MAIF|MAISON|MAKEUP|MAN|MANAGEMENT|MANGO|MAP|MARKET|MARKETING|MARKETS|MARRIOTT|MARSHALLS|MASERATI|MATTEL|MBA|MC|MCKINSEY|MD|ME|MED|MEDIA|MEET|MELBOURNE|MEME|MEMORIAL|MEN|MENU|MERCKMSD|MG|MH|MIAMI|MICROSOFT|MIL|MINI|MINT|MIT|MITSUBISHI|MK|ML|MLB|MLS|MM|MMA|MN|MO|MOBI|MOBILE|MODA|MOE|MOI|MOM|MONASH|MONEY|MONSTER|MORMON|MORTGAGE|MOSCOW|MOTO|MOTORCYCLES|MOV|MOVIE|MP|MQ|MR|MS|MSD|MT|MTN|MTR|MU|MUSEUM|MUTUAL|MV|MW|MX|MY|MZ|NA|NAB|NAGOYA|NAME|NATIONWIDE|NATURA|NAVY|NBA|NC|NE|NEC|NET|NETBANK|NETFLIX|NETWORK|NEUSTAR|NEW|NEWHOLLAND|NEWS|NEXT|NEXTDIRECT|NEXUS|NF|NFL|NG|NGO|NHK|NI|NICO|NIKE|NIKON|NINJA|NISSAN|NISSAY|NL|NO|NOKIA|NORTHWESTERNMUTUAL|NORTON|NOW|NOWRUZ|NOWTV|NP|NR|NRA|NRW|NTT|NU|NYC|NZ|OBI|OBSERVER|OFF|OFFICE|OKINAWA|OLAYAN|OLAYANGROUP|OLDNAVY|OLLO|OM|OMEGA|ONE|ONG|ONL|ONLINE|ONYOURSIDE|OOO|OPEN|ORACLE|ORANGE|ORG|ORGANIC|ORIGINS|OSAKA|OTSUKA|OTT|OVH|PA|PAGE|PANASONIC|PARIS|PARS|PARTNERS|PARTS|PARTY|PASSAGENS|PAY|PCCW|PE|PET|PF|PFIZER|PG|PH|PHARMACY|PHD|PHILIPS|PHONE|PHOTO|PHOTOGRAPHY|PHOTOS|PHYSIO|PICS|PICTET|PICTURES|PID|PIN|PING|PINK|PIONEER|PIZZA|PK|PL|PLACE|PLAY|PLAYSTATION|PLUMBING|PLUS|PM|PN|PNC|POHL|POKER|POLITIE|PORN|POST|PR|PRAMERICA|PRAXI|PRESS|PRIME|PRO|PROD|PRODUCTIONS|PROF|PROGRESSIVE|PROMO|PROPERTIES|PROPERTY|PROTECTION|PRU|PRUDENTIAL|PS|PT|PUB|PW|PWC|PY|QA|QPON|QUEBEC|QUEST|QVC|RACING|RADIO|RAID|RE|READ|REALESTATE|REALTOR|REALTY|RECIPES|RED|REDSTONE|REDUMBRELLA|REHAB|REISE|REISEN|REIT|RELIANCE|REN|RENT|RENTALS|REPAIR|REPORT|REPUBLICAN|REST|RESTAURANT|REVIEW|REVIEWS|REXROTH|RICH|RICHARDLI|RICOH|RIL|RIO|RIP|RMIT|RO|ROCHER|ROCKS|RODEO|ROGERS|ROOM|RS|RSVP|RU|RUGBY|RUHR|RUN|RW|RWE|RYUKYU|SA|SAARLAND|SAFE|SAFETY|SAKURA|SALE|SALON|SAMSCLUB|SAMSUNG|SANDVIK|SANDVIKCOROMANT|SANOFI|SAP|SARL|SAS|SAVE|SAXO|SB|SBI|SBS|SC|SCA|SCB|SCHAEFFLER|SCHMIDT|SCHOLARSHIPS|SCHOOL|SCHULE|SCHWARZ|SCIENCE|SCJOHNSON|SCOT|SD|SE|SEARCH|SEAT|SECURE|SECURITY|SEEK|SELECT|SENER|SERVICES|SES|SEVEN|SEW|SEX|SEXY|SFR|SG|SH|SHANGRILA|SHARP|SHAW|SHELL|SHIA|SHIKSHA|SHOES|SHOP|SHOPPING|SHOUJI|SHOW|SHOWTIME|SHRIRAM|SI|SILK|SINA|SINGLES|SITE|SJ|SK|SKI|SKIN|SKY|SKYPE|SL|SLING|SM|SMART|SMILE|SN|SNCF|SO|SOCCER|SOCIAL|SOFTBANK|SOFTWARE|SOHU|SOLAR|SOLUTIONS|SONG|SONY|SOY|SPA|SPACE|SPORT|SPOT|SPREADBETTING|SR|SRL|SS|ST|STADA|STAPLES|STAR|STATEBANK|STATEFARM|STC|STCGROUP|STOCKHOLM|STORAGE|STORE|STREAM|STUDIO|STUDY|STYLE|SU|SUCKS|SUPPLIES|SUPPLY|SUPPORT|SURF|SURGERY|SUZUKI|SV|SWATCH|SWIFTCOVER|SWISS|SX|SY|SYDNEY|SYSTEMS|SZ|TAB|TAIPEI|TALK|TAOBAO|TARGET|TATAMOTORS|TATAR|TATTOO|TAX|TAXI|TC|TCI|TD|TDK|TEAM|TECH|TECHNOLOGY|TEL|TEMASEK|TENNIS|TEVA|TF|TG|TH|THD|THEATER|THEATRE|TIAA|TICKETS|TIENDA|TIFFANY|TIPS|TIRES|TIROL|TJ|TJMAXX|TJX|TK|TKMAXX|TL|TM|TMALL|TN|TO|TODAY|TOKYO|TOOLS|TOP|TORAY|TOSHIBA|TOTAL|TOURS|TOWN|TOYOTA|TOYS|TR|TRADE|TRADING|TRAINING|TRAVEL|TRAVELCHANNEL|TRAVELERS|TRAVELERSINSURANCE|TRUST|TRV|TT|TUBE|TUI|TUNES|TUSHU|TV|TVS|TW|TZ|UA|UBANK|UBS|UG|UK|UNICOM|UNIVERSITY|UNO|UOL|UPS|US|UY|UZ|VA|VACATIONS|VANA|VANGUARD|VC|VE|VEGAS|VENTURES|VERISIGN|VERSICHERUNG|VET|VG|VI|VIAJES|VIDEO|VIG|VIKING|VILLAS|VIN|VIP|VIRGIN|VISA|VISION|VIVA|VIVO|VLAANDEREN|VN|VODKA|VOLKSWAGEN|VOLVO|VOTE|VOTING|VOTO|VOYAGE|VU|VUELOS|WALES|WALMART|WALTER|WANG|WANGGOU|WATCH|WATCHES|WEATHER|WEATHERCHANNEL|WEBCAM|WEBER|WEBSITE|WED|WEDDING|WEIBO|WEIR|WF|WHOSWHO|WIEN|WIKI|WILLIAMHILL|WIN|WINDOWS|WINE|WINNERS|WME|WOLTERSKLUWER|WOODSIDE|WORK|WORKS|WORLD|WOW|WS|WTC|WTF|XBOX|XEROX|XFINITY|XIHUAN|XIN|XN--11B4C3D|XN--1CK2E1B|XN--1QQW23A|XN--2SCRJ9C|XN--30RR7Y|XN--3BST00M|XN--3DS443G|XN--3E0B707E|XN--3HCRJ9C|XN--3OQ18VL8PN36A|XN--3PXU8K|XN--42C2D9A|XN--45BR5CYL|XN--45BRJ9C|XN--45Q11C|XN--4GBRIM|XN--54B7FTA0CC|XN--55QW42G|XN--55QX5D|XN--5SU34J936BGSG|XN--5TZM5G|XN--6FRZ82G|XN--6QQ986B3XL|XN--80ADXHKS|XN--80AO21A|XN--80AQECDR1A|XN--80ASEHDB|XN--80ASWG|XN--8Y0A063A|XN--90A3AC|XN--90AE|XN--90AIS|XN--9DBQ2A|XN--9ET52U|XN--9KRT00A|XN--B4W605FERD|XN--BCK1B9A5DRE4C|XN--C1AVG|XN--C2BR7G|XN--CCK2B3B|XN--CCKWCXETD|XN--CG4BKI|XN--CLCHC0EA0B2G2A9GCD|XN--CZR694B|XN--CZRS0T|XN--CZRU2D|XN--D1ACJ3B|XN--D1ALF|XN--E1A4C|XN--ECKVDTC9D|XN--EFVY88H|XN--FCT429K|XN--FHBEI|XN--FIQ228C5HS|XN--FIQ64B|XN--FIQS8S|XN--FIQZ9S|XN--FJQ720A|XN--FLW351E|XN--FPCRJ9C3D|XN--FZC2C9E2C|XN--FZYS8D69UVGM|XN--G2XX48C|XN--GCKR3F0F|XN--GECRJ9C|XN--GK3AT1E|XN--H2BREG3EVE|XN--H2BRJ9C|XN--H2BRJ9C8C|XN--HXT814E|XN--I1B6B1A6A2E|XN--IMR513N|XN--IO0A7I|XN--J1AEF|XN--J1AMH|XN--J6W193G|XN--JLQ480N2RG|XN--JLQ61U9W7B|XN--JVR189M|XN--KCRX77D1X4A|XN--KPRW13D|XN--KPRY57D|XN--KPUT3I|XN--L1ACC|XN--LGBBAT1AD8J|XN--MGB9AWBF|XN--MGBA3A3EJT|XN--MGBA3A4F16A|XN--MGBA7C0BBN0A|XN--MGBAAKC7DVF|XN--MGBAAM7A8H|XN--MGBAB2BD|XN--MGBAH1A3HJKRD|XN--MGBAI9AZGQP6J|XN--MGBAYH7GPA|XN--MGBBH1A|XN--MGBBH1A71E|XN--MGBC0A9AZCG|XN--MGBCA7DZDO|XN--MGBCPQ6GPA1A|XN--MGBERP4A5D4AR|XN--MGBGU82A|XN--MGBI4ECEXP|XN--MGBPL2FH|XN--MGBT3DHD|XN--MGBTX2B|XN--MGBX4CD0AB|XN--MIX891F|XN--MK1BU44C|XN--MXTQ1M|XN--NGBC5AZD|XN--NGBE9E0A|XN--NGBRX|XN--NODE|XN--NQV7F|XN--NQV7FS00EMA|XN--NYQY26A|XN--O3CW4H|XN--OGBPF8FL|XN--OTU796D|XN--P1ACF|XN--P1AI|XN--PGBS0DH|XN--PSSY2U|XN--Q7CE6A|XN--Q9JYB4C|XN--QCKA1PMC|XN--QXA6A|XN--QXAM|XN--RHQV96G|XN--ROVU88B|XN--RVC1E0AM3E|XN--S9BRJ9C|XN--SES554G|XN--T60B56A|XN--TCKWE|XN--TIQ49XQYJ|XN--UNUP4Y|XN--VERMGENSBERATER-CTB|XN--VERMGENSBERATUNG-PWB|XN--VHQUV|XN--VUQ861B|XN--W4R85EL8FHU5DNRA|XN--W4RS40L|XN--WGBH1C|XN--WGBL6A|XN--XHQ521B|XN--XKC2AL3HYE2A|XN--XKC2DL3A5EE0H|XN--Y9A3AQ|XN--YFRO4I67O|XN--YGBI2AMMX|XN--ZFR164B|XXX|XYZ|YACHTS|YAHOO|YAMAXUN|YANDEX|YE|YODOBASHI|YOGA|YOKOHAMA|YOU|YOUTUBE|YT|YUN|ZA|ZAPPOS|ZARA|ZERO|ZIP|ZM|ZONE|ZUERICH|ZW)";
                Regex loRegexIsInternetEmailAddressAtTLD = new Regex("^" + lsEmailFormat + "@" + lsTLDList + @"\b$", RegexOptions.IgnoreCase);

                if (!loRegexIsInternetEmailAddressAtTLD.IsMatch(lsEmail))
                {
                    string lsServerFormat = @"(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)";
                    Regex loRegexIsInternetEmailAddress = new Regex("^" + lsEmailFormat + "@" + lsServerFormat + "+" + lsTLDList + @"\b$", RegexOptions.IgnoreCase);
                    if (!loRegexIsInternetEmailAddress.IsMatch(lsEmail))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Checks user data for a new user to prevent duplicates.
        /// </summary>
        /// <param name="loProviderKey">The Id of the new user.</param>
        /// <param name="lsUsername">The username for the new user.</param>
        /// <param name="lsEmail">The email address for the new user.</param>
        /// <param name="loKey">Key created for the user.</param>
        /// <returns>MembershipCreateStatus.Success if all checks pass.</returns>
        protected MembershipCreateStatus CheckUserData(object loProviderKey, string lsUsername, string lsEmail, out Guid loKey)
        {
            if (null == loProviderKey)
            {
                loKey = Guid.NewGuid();
            }
            else
            {
                loKey = MaxConvertLibrary.ConvertToGuid(typeof(object), loProviderKey);
            }

            if (Guid.Empty.Equals(loKey))
            {
                return MembershipCreateStatus.InvalidProviderUserKey;
            }

            //// Check for duplicate provider user key
            MaxUserEntity loEntity = MaxUserEntity.Create();
            if (loEntity.LoadByIdCache(loKey))
            {
                return MembershipCreateStatus.DuplicateProviderUserKey;
            }

            if (string.IsNullOrEmpty(lsUsername))
            {
                return MembershipCreateStatus.InvalidUserName;
            }

            if (this.IsDuplicateUserName(loKey, lsUsername))
            {
                return MembershipCreateStatus.DuplicateUserName;
            }

            if (!this.IsValidEmail(lsEmail))
            {
                return MembershipCreateStatus.InvalidEmail;
            }

            //// Check for duplicate email
            if (this.RequiresUniqueEmail)
            {
                if (this.IsDuplicateEmail(loKey, lsEmail))
                {
                    return MembershipCreateStatus.DuplicateEmail;
                }
            }

            return MembershipCreateStatus.Success;
        }

        /// <summary>
        /// Checks the password data.
        /// </summary>
        /// <param name="lsPassword">The password or null if not checking the password.</param>
        /// <param name="lsQuestion">Question to reset the password.</param>
        /// <param name="lsAnswer">Answer for the password question.</param>
        /// <returns>MembershipCreateStatus.Success if all checks pass.</returns>
        protected MembershipCreateStatus CheckPasswordData(string lsPassword, string lsQuestion, string lsAnswer)
        {
            if (this.RequiresQuestionAndAnswer)
            {
                if (string.IsNullOrEmpty(lsQuestion))
                {
                    return MembershipCreateStatus.InvalidQuestion;
                }
                else if (string.IsNullOrEmpty(lsAnswer))
                {
                    return MembershipCreateStatus.InvalidAnswer;
                }
            }

            if (null != lsPassword)
            {
                if (this.MinRequiredPasswordLength > 0)
                {
                    if (lsPassword.Length < this.MinRequiredPasswordLength)
                    {
                        return MembershipCreateStatus.InvalidPassword;
                    }
                }

                if (this.MinRequiredNonAlphanumericCharacters > 0)
                {
                    int lnNonAlpha = 0;
                    for (int lnC = 0; lnC < lsPassword.Length; lnC++)
                    {
                        if (!char.IsLetterOrDigit(lsPassword.ToCharArray()[lnC]))
                        {
                            lnNonAlpha++;
                        }
                    }

                    if (lnNonAlpha < this.MinRequiredNonAlphanumericCharacters)
                    {
                        return MembershipCreateStatus.InvalidPassword;
                    }
                }

                if (!string.IsNullOrEmpty(this.PasswordStrengthRegularExpression))
                {
                    Regex loRegex = new Regex(this.PasswordStrengthRegularExpression);
                    if (!loRegex.IsMatch(lsPassword))
                    {
                        return MembershipCreateStatus.InvalidPassword;
                    }
                }
            }

            return MembershipCreateStatus.Success;
        }

        /// <summary>
        /// Gets the Max password format that relates to the configured password format.
        /// </summary>
        /// <returns>Value matching password format.</returns>
        protected int GetMaxPasswordFormat()
        {
            if (this.PasswordFormat == MembershipPasswordFormat.Clear)
            {
                return MaxUserPasswordEntity.MembershipPasswordFormatClear;
            }
            else if (this.PasswordFormat == MembershipPasswordFormat.Encrypted)
            {
                return MaxUserPasswordEntity.MembershipPasswordFormatEncrypted;
            }
            else if (this.PasswordFormat == MembershipPasswordFormat.Hashed)
            {
                return MaxUserPasswordEntity.MembershipPasswordFormatHashed;
            }

            throw new MaxException("Password format not supported");
        }

        /// <summary>
        /// Gets the entity containing the user information for the specified user.
        /// </summary>
        /// <param name="lsUserName">Username to look up.</param>
        /// <returns>entity containing user information.</returns>
        protected MaxUserEntity GetMaxUser(string lsUserName)
        {
            if (string.IsNullOrEmpty(lsUserName))
            {
                return null;
            }

            if (lsUserName.Trim().Length.Equals(0))
            {
                return null;
            }

            MaxEntityList loMaxUserList = MaxUserEntity.Create().LoadAllByUsernameCache(lsUserName);
            if (loMaxUserList.Count.Equals(1))
            {
                return (MaxUserEntity)loMaxUserList[0];
            }
            else if (loMaxUserList.Count.Equals(0))
            {
                // throw new MaxException("User [" + lsUserName + "] cannot be found.");
                return null;
            }

            throw new MaxException("More than one user matches [" + lsUserName + "].");
        }

        /// <summary>
        /// Gets the entity with the supplied ID
        /// </summary>
        /// <param name="loProviderKey">The object that maps to the unique id</param>
        /// <returns>An entity with the user information</returns>
        protected MaxUserEntity GetMaxUser(object loProviderKey)
        {
            MaxUserEntity loR = MaxUserEntity.Create();
            if (!loR.LoadByDataKeyCache(loProviderKey.ToString()))
            {
                return null;
            }

            return loR;
        }

        /// <summary>
        /// Gets the mapped MembershipUser based on the MaxUserEntity.
        /// Only gets part of the properties.  For use with lists of users.
        /// </summary>
        /// <param name="loMaxUser">The user to map.</param>
        /// <returns>The matching MembershipUser.</returns>
        protected MembershipUser GetUser(MaxUserEntity loMaxUser)
        {
            DateTime ldLastLoginFailure = DateTime.MinValue;
            DateTime ldLastLockout = DateTime.MinValue;
            DateTime ldLastUnlock = DateTime.MinValue;
            DateTime ldLastLogin = DateTime.MinValue;
            DateTime ldLastActivity = DateTime.MinValue;
            DateTime ldLastPasswordChange = DateTime.MinValue;
            bool lbIsLockedOut = false;
            string lsPasswordQuestion = string.Empty;

            MembershipUser loUser = new MembershipUser(
                this.Name,
                loMaxUser.UserName,
                loMaxUser.Id,
                loMaxUser.Email,
                lsPasswordQuestion,
                loMaxUser.Comment,
                loMaxUser.IsActive,
                lbIsLockedOut,
                loMaxUser.CreatedDate,
                ldLastLogin,
                ldLastActivity,
                ldLastPasswordChange,
                ldLastLockout);
            return loUser;
        }

        /// <summary>
        /// Gets the mapped MembershipUser based on the MaxUserEntity.
        /// </summary>
        /// <param name="loMaxUser">The user to map.</param>
        /// <returns>The matching MembershipUser.</returns>
        protected MembershipUser GetUserDetails(MaxUserEntity loMaxUser)
        {
            return new MaxMembershipUser(loMaxUser, this);
        }

        /// <summary>
        /// Gets a text error message based on the creation status.
        /// </summary>
        /// <param name="loStatus">The failure status.</param>
        /// <returns>Error message based on the status.</returns>
        public static string GetCreateErrorMessage(MembershipCreateStatus loStatus)
        {
            string lsR = loStatus.ToString();
            if (loStatus.Equals(MembershipCreateStatus.DuplicateEmail))
            {
                lsR = "Another account already exists with this email address. If you have forgotten your account information, you can have information emailed to you using the forgot password functionality.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.DuplicateProviderUserKey))
            {
                lsR = "Another account already exists with this Id. This should never happen, so something must be broken.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.DuplicateUserName))
            {
                lsR = "Another account already exists with this user name. If you have forgotten your account information, you can have information emailed to you using the forgot password functionality.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.InvalidAnswer))
            {
                lsR = "The answer to the password reset question is not valid. It is required to be something you can type in if you forget your password.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.InvalidEmail))
            {
                lsR = "The email address format is not valid. It should be something like 'myemail@mydomain.com'.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.InvalidPassword))
            {
                lsR = "The password is not valid.";
                if (Membership.MinRequiredPasswordLength > 0)
                {
                    lsR += string.Format(" The password needs to be at least {0} characters.", Membership.MinRequiredPasswordLength);
                }

                if (Membership.MinRequiredNonAlphanumericCharacters > 0)
                {
                    lsR += string.Format(" The password needs to have at least {0} characters that are not numbers or letters.", Membership.MinRequiredNonAlphanumericCharacters);
                }

                if (Membership.PasswordStrengthRegularExpression.Length > 0)
                {
                    lsR += string.Format(" The password needs to be complex [{0}].", Membership.PasswordStrengthRegularExpression);
                }
            }
            else if (loStatus.Equals(MembershipCreateStatus.InvalidProviderUserKey))
            {
                lsR = "The account key is not valid.  This should never happen, so something must be broken.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.InvalidQuestion))
            {
                lsR = "The password reset question needs to be provided.";
            }
            else if (loStatus.Equals(MembershipCreateStatus.InvalidUserName))
            {
                lsR = "The user name needs to be provided.";
            }

            return lsR;
        }
    }
}
