using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Litium.Accelerator.Constants
{
    public class CookieNotificationMessage
    {
        public const string CookieName = "Tengtools.CookieAccepted";
        public static TimeSpan Expires = new TimeSpan(365, 0, 0, 0);
        public const string UserAcceptedValue = "UserAccepted";
    }
}
