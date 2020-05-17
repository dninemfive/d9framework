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
    class PatchOperationAddSafe : PatchOperationPathed
    {
        private enum Order { Append, Prepend }
        private XmlContainer value;
        private Order order = Order.Append;

        protected override bool ApplyWorker(XmlDocument xml)
        {
            string parentPath = GetParentPath(xpath);

        }

        private string GetParentPath(string xp)
        {
            if (xp.Length < 3) return xp;
            if (xp.Substring(xp.Length - 3) == "/..") return GetParentPath(xp.Substring(0, xp.Length - 3));
            return xp;
        }
    }
}