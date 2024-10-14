// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI.Skins.EventListeners
{
    using System;
    using DotNetNuke.Web.Mvc.Skins;

    /// <summary>SkinEventArgs provides a custom EventARgs class for Skin Events.</summary>
    [CLSCompliant(false)]
    public class SkinEventArgs : EventArgs
    {
        private readonly SkinModel skin;

        /// <summary>Initializes a new instance of the <see cref="SkinEventArgs"/> class.</summary>
        /// <param name="skin"></param>
        public SkinEventArgs(SkinModel skin)
        {
            this.skin = skin;
        }

        public SkinModel Skin
        {
            get
            {
                return this.skin;
            }
        }
    }
}
