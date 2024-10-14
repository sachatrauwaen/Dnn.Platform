// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Security.Membership
{
    internal enum MembershipCreateStatus
    {
        DuplicateEmail,
        UserRejected,
        ProviderError,
        InvalidUserName,
        InvalidAnswer,
        InvalidEmail,
        InvalidPassword,
        InvalidProviderUserKey,
        InvalidQuestion,
        DuplicateUserName,
        DuplicateProviderUserKey
    }
}
