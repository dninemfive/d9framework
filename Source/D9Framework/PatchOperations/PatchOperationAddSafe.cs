using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using Verse;

namespace D9Framework
{
    /// <summary>
    /// Works exactly like PatchOperationAdd, except that if the parent node doesn't exist this operation creates it.
    /// </summary>
    class PatchOperationAddSafe : PatchOperationAdd
    {
        private enum Order { Append, Prepend }
        private XmlContainer value;
        private Order parentOrder = Order.Append, childOrder = Order.Append;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            // select nodes with xpath
            // for each parent:
            //      if identical node exists,
            //          append children to node
            //      else,
            //          create parent
            //          append children to node
        }
    }
}