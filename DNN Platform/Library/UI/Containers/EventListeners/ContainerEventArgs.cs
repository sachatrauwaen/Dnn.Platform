// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI.Containers.EventListeners
{
    using System;
    using DotNetNuke.Web.Mvc.Skins;

    /// <summary>ContainerEventArgs provides a custom EventARgs class for Container Events.</summary>
    [CLSCompliant(false)]
    public class ContainerEventArgs : EventArgs
    {
        private readonly ContainerModel container;

        /// <summary>Initializes a new instance of the <see cref="ContainerEventArgs"/> class.</summary>
        /// <param name="container"></param>
        public ContainerEventArgs(ContainerModel container)
        {
            this.container = container;
        }

        public ContainerModel Container
        {
            get
            {
                return this.container;
            }
        }
    }
}
