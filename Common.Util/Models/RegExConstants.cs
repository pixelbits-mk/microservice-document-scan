using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Util.Models
{
    public class RegExConstants
    {
        public const string EmailAddress = @"(?:\s*""?(?<DisplayName>[^""]+)""?\s*<)?(?<Address>\b[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Z|a-z]{2,7}\b)>?";
        public const string DocumentUrl = @"^https?://[^\s/$.?#].[^\s]*$"; 

    }
}
