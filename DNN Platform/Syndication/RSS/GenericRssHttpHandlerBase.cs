// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.Services.Syndication
{
    using System;

    /// <summary>Late-bound RSS HTTP Handler to publish RSS channel.</summary>
    [CLSCompliant(false)]
    public class GenericRssHttpHandlerBase : RssHttpHandlerBase<GenericRssChannel, GenericRssElement, GenericRssElement>
    {
    }
}
