// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Security.Membership
{
    using System;

    internal class MembershipUser
    {
        public bool IsLockedOut { get; internal set; }

        public string ProviderUserKey { get; internal set; }

        public string UserName { get; internal set; }

        public DateTime LastLockoutDate { get; internal set; }

        public DateTime CreationDate { get; internal set; }

        public DateTime LastActivityDate { get; internal set; }

        public DateTime LastLoginDate { get; internal set; }

        public DateTime LastPasswordChangedDate { get; internal set; }

        public string PasswordQuestion { get; internal set; }

        public bool IsApproved { get; internal set; }

        public string Email { get; internal set; }

        internal bool ChangePassword(string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        internal bool ChangePasswordQuestionAndAnswer(string password, string passwordQuestion, string passwordAnswer)
        {
            throw new NotImplementedException();
        }

        internal string GetPassword()
        {
            throw new NotImplementedException();
        }

        internal string GetPassword(string passwordAnswer)
        {
            throw new NotImplementedException();
        }

        internal string ResetPassword(string passwordAnswer)
        {
            throw new NotImplementedException();
        }

        internal string ResetPassword()
        {
            throw new NotImplementedException();
        }

        internal bool UnlockUser()
        {
            throw new NotImplementedException();
        }
    }
}
