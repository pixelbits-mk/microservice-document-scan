using Common.Util.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common.Util.Services
{
    public class EmailAddressUtility
    {
        public static IList<EmailAddress> Parse(string emailAddress)
        {
            List<EmailAddress> addresses = new List<EmailAddress>();

            // Define a regular expression pattern to match email addresses with or without display names
            string pattern = RegExConstants.EmailAddress;

            MatchCollection matches = Regex.Matches(emailAddress, pattern);

            foreach (Match match in matches)
            {
                string displayName = match.Groups["DisplayName"].Value.Trim();
                string address = match.Groups["Address"].Value.Trim();

                EmailAddress email = new EmailAddress
                {
                    DisplayName = displayName,
                    Address = address
                };

                addresses.Add(email);
            }

            return addresses;
        }
    }
}
