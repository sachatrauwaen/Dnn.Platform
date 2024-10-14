// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information
namespace DotNetNuke.UI
{
    using System;
    using System.Collections.Generic;

    public class DNNNodeCollection : List<DNNNode>
    {
        private string strNamespace;

        public DNNNodeCollection(string strNamespace)
        {
            this.strNamespace = strNamespace;
        }

        public object XMLNode { get; internal set; }

        internal int Add()
        {
            throw new NotImplementedException();
        }

        internal void AddBreak()
        {
            throw new NotImplementedException();
        }

        internal DNNNode FindNode(string strID)
        {
            throw new NotImplementedException();
        }

        internal void Import(DNNNode objNode)
        {
            throw new NotImplementedException();
        }
    }
}
