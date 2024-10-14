// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI
{
    public class DNNNode
    {
        private object xMLNode;

        public DNNNode()
        {
        }

        public DNNNode(object xMLNode)
        {
            this.xMLNode = xMLNode;
        }

        public DNNNode ParentNode { get; internal set; }

        public bool Enabled { get; internal set; }

        public string JSFunction { get; internal set; }

        public object ClickAction { get; internal set; }

        public string NavigateURL { get; internal set; }

        public string Target { get; internal set; }

        public string Image { get; internal set; }

        public string ID { get; internal set; }

        public string Key { get; internal set; }

        public string Text { get; internal set; }

        public bool Selected { get; internal set; }

        public bool BreadCrumb { get; internal set; }

        public string LargeImage { get; internal set; }

        public string ToolTip { get; internal set; }

        public int Level { get; internal set; }

        public bool HasNodes { get; internal set; }
        public bool IsBreak { get; internal set; }
        internal DNNNodeCollection DNNNodes { get; set; }

    }
}
