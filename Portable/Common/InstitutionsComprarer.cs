using System;
using System.Collections.Generic;
using dona.Forms.Model;

namespace dona.Forms.Common
{
    public class InstitutionsComprarer : IComparer<Institution>
    {
        public int Compare(Institution x, Institution y)
        {
            return string.Compare(x.Name.ReplaceAccentsInVocals(), y.Name.ReplaceAccentsInVocals(), StringComparison.OrdinalIgnoreCase);
        }
    }
}