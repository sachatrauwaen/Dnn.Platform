// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace System.Web.Security
{
    public class MembershipProvider
    {
        public MembershipPasswordFormat PasswordFormat { get; internal set; }

        public bool RequiresUniqueEmail { get; internal set; }
    }
}
