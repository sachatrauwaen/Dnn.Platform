// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace System.Web.Security
{
    using System;

    using DotNetNuke.Framework.Providers;
    using DotNetNuke.Security.Membership;

    internal class Membership
    {
        public static string PasswordStrengthRegularExpression { get; internal set; }

        public static bool EnablePasswordRetrieval { get; internal set; }

        public static bool EnablePasswordReset { get; internal set; }

        public static int MinRequiredPasswordLength { get; internal set; }

        public static int MinRequiredNonAlphanumericCharacters { get; internal set; }

        public static int MaxInvalidPasswordAttempts { get; internal set; }

        public static MembershipProvider Provider { get; internal set; }

        public static int PasswordAttemptWindow { get; internal set; }

        public static bool RequiresQuestionAndAnswer { get; internal set; }

        internal static void CreateUser(string userName, string password, string email, object value1, object value2, bool v, out MembershipCreateStatus status)
        {
            throw new NotImplementedException();
        }

        internal static void DeleteUser(string username, bool v)
        {
            throw new NotImplementedException();
        }

        internal static string GeneratePassword(int length, int minNonAlphanumericCharacters)
        {
            throw new NotImplementedException();
        }

        internal static DotNetNuke.Security.Membership.MembershipUser GetUser(string username)
        {
            throw new NotImplementedException();
        }

        internal static object GetUser(Guid guid)
        {
            throw new NotImplementedException();
        }

        internal static void UpdateUser(MembershipUser membershipUser)
        {
            throw new NotImplementedException();
        }

        internal static bool ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
