// <copyright file="MaxMembershipUser.cs" company="Lakstins Family, LLC">
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
// <change date="2/22/2014" author="Brian A. Lakstins" description="Initial creation">
// <change date="2/24/2021" author="Brian A. Lakstins" description="Add password reset method.">
// <change date="3/30/2024" author="Brian A. Lakstins" description="Update for change to dependent class.">
// <change date="6/19/2024" author="Brian A. Lakstins" description="Update user related logging.">
// </changelog>
#endregion

namespace System.Web.Security
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using MaxFactry.General.BusinessLayer;
    using MaxFactry.Base.BusinessLayer;

    public class MaxMembershipUser : MembershipUser
    {
        private MaxUserEntity _oMaxUser = null;

        private MembershipProvider _oMembership = null;

        private MaxEntityList _oUserLogList = null;

        public MaxMembershipUser(MaxUserEntity loMaxUser, MembershipProvider loMembership)
        {
            this._oMaxUser = loMaxUser;
            this._oMembership = loMembership;
        }

        protected MaxEntityList UserLogList
        {
            get
            {
                if (null == this._oUserLogList)
                {
                    this._oUserLogList = MaxUserLogEntity.Create().LoadAllByUserIdCache(this._oMaxUser.Id);
                }

                return this._oUserLogList;
            }
        }

        public override string Comment
        {
            get
            {
                return this._oMaxUser.Comment;
            }

            set
            {
                this._oMaxUser.Comment = value;
            }
        }

        public override string Email
        {
            get
            {
                return this._oMaxUser.Email;
            }

            set
            {
                this._oMaxUser.Email = value;
            }
        }

        public override DateTime CreationDate
        {
            get
            {
                return this._oMaxUser.CreatedDate;
            }
        }

        public override bool IsApproved
        {
            get
            {
                return this._oMaxUser.IsActive;
            }

            set
            {
                this._oMaxUser.IsActive = value;
            }
        }


        public bool IsPasswordResetNeeded
        {
            get
            {
                return this._oMaxUser.IsPasswordResetNeeded;
            }

            set
            {
                this._oMaxUser.IsPasswordResetNeeded = value;
            }
        }

        public override bool IsLockedOut
        {
            get
            {
                bool lbR = false;
                DateTime ldPasswordWindow = DateTime.Now.AddMinutes(-1 * this._oMembership.PasswordAttemptWindow);
                DateTime ldLastLockout = DateTime.MinValue;
                DateTime ldLastLoginFailure = DateTime.MinValue;
                DateTime ldLastUnlock = DateTime.MinValue;
                int lnLoginFailureCount = 0;
                for (int lnL = 0; lnL < this.UserLogList.Count; lnL++)
                {
                    MaxUserLogEntity loMaxUserLog = (MaxUserLogEntity)this.UserLogList[lnL];
                    if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypePasswordFail &&
                            loMaxUserLog.CreatedDate > ldPasswordWindow)
                    {
                        if (loMaxUserLog.CreatedDate > ldLastLoginFailure)
                        {
                            ldLastLoginFailure = loMaxUserLog.CreatedDate;
                        }

                        lnLoginFailureCount++;
                    }
                    else if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypeUnlockout)
                    {
                        if (loMaxUserLog.CreatedDate > ldLastUnlock)
                        {
                            ldLastUnlock = loMaxUserLog.CreatedDate;
                        }
                    }
                    else if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypeLockout)
                    {
                        if (loMaxUserLog.CreatedDate > ldLastLockout)
                        {
                            ldLastLockout = loMaxUserLog.CreatedDate;
                        }
                    }
                }

                if (ldLastLockout > ldPasswordWindow)
                {
                    lbR = true;
                }

                if (lnLoginFailureCount >= this._oMembership.MaxInvalidPasswordAttempts &&
                        !lbR &&
                        ldLastUnlock < ldLastLoginFailure)
                {
                    MaxUserLogEntity loMaxUserLog = MaxUserLogEntity.Create();
                    loMaxUserLog.Insert(
                        this._oMaxUser.Id,
                        MaxUserLogEntity.LogEntryTypeLockout,
                        this.GetType() + ".IsLockedOut - " + lnLoginFailureCount.ToString() + " failures since " + ldPasswordWindow.ToString());
                    lbR = true;
                }
                else if (lbR && ldLastUnlock > ldLastLoginFailure)
                {
                    lbR = false;
                }

                return lbR;
            }
        }

#if net4_52
        public override bool IsOnline
        {
            get
            {
                return base.IsOnline;
            }
        }
#endif

        public override DateTime LastActivityDate
        {
            get
            {
                DateTime ldR = DateTime.MinValue;
                for (int lnL = 0; lnL < this.UserLogList.Count; lnL++)
                {
                    MaxUserLogEntity loMaxUserLog = (MaxUserLogEntity)this.UserLogList[lnL];
                    if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypeActivity)
                    {
                        if (loMaxUserLog.CreatedDate > ldR)
                        {
                            ldR = loMaxUserLog.CreatedDate;
                        }
                    }
                }

                return ldR;
            }

            set
            {
                base.LastActivityDate = value;
            }
        }

        public override DateTime LastLockoutDate
        {
            get
            {
                DateTime ldR = DateTime.MinValue;
                for (int lnL = 0; lnL < this.UserLogList.Count; lnL++)
                {
                    MaxUserLogEntity loMaxUserLog = (MaxUserLogEntity)this.UserLogList[lnL];
                    if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypeLockout)
                    {
                        if (loMaxUserLog.CreatedDate > ldR)
                        {
                            ldR = loMaxUserLog.CreatedDate;
                        }
                    }
                }

                return ldR;
            }
        }

        public override DateTime LastLoginDate
        {
            get
            {
                DateTime ldR = DateTime.MinValue;
                for (int lnL = 0; lnL < this.UserLogList.Count; lnL++)
                {
                    MaxUserLogEntity loMaxUserLog = (MaxUserLogEntity)this.UserLogList[lnL];
                    if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypeLogin)
                    {
                        if (loMaxUserLog.CreatedDate > ldR)
                        {
                            ldR = loMaxUserLog.CreatedDate;
                        }
                    }
                }

                return ldR;
            }

            set
            {
                base.LastLoginDate = value;
            }
        }

        public override DateTime LastPasswordChangedDate
        {
            get
            {
                DateTime ldR = DateTime.MinValue;
                for (int lnL = 0; lnL < this.UserLogList.Count; lnL++)
                {
                    MaxUserLogEntity loMaxUserLog = (MaxUserLogEntity)this.UserLogList[lnL];
                    if (loMaxUserLog.LogEntryType == MaxUserLogEntity.LogEntryTypePasswordChange && loMaxUserLog.Comment == "ChangePassword")
                    {
                        if (loMaxUserLog.CreatedDate > ldR)
                        {
                            ldR = loMaxUserLog.CreatedDate;
                        }
                    }
                }

                return ldR;
            }
        }

        public override string PasswordQuestion
        {
            get
            {
                string lsR = string.Empty;
                MaxUserPasswordEntity loMaxPassword = MaxUserPasswordEntity.Create().GetLatestByUserId(this._oMaxUser.Id);
                if (null != loMaxPassword)
                {
                    lsR = loMaxPassword.PasswordQuestion;
                }

                return lsR;
            }
        }

        public override string ProviderName
        {
            get
            {
                return this._oMembership.Name;
            }
        }

        public override object ProviderUserKey
        {
            get
            {
                return this._oMaxUser.Id;
            }
        }

        public override string UserName
        {
            get
            {
                return this._oMaxUser.UserName;
            }
        }

        public static bool SetPassword(MembershipUser loUser, string lsPassword)
        {
            string lsRandomPassword = loUser.ResetPassword();
            System.Threading.Thread.Sleep(1000); //// Make sure the new password is newer than the random one enough so sorting works
            if (loUser.ChangePassword(lsRandomPassword, lsPassword))
            {
                return true;
            }

            return false;
        }
    }
}
