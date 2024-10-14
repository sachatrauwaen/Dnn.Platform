// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI.WebControls
{
    public class TemplateColumn
    {
        public string HeaderText { get; private set; }

        public object HeaderStyle { get; private set; }

        public virtual void Initialize()
        {
        }
    }
}
