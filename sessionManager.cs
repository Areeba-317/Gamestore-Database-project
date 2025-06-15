using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace stupid_project
{
    internal class sessionManager
    {
    }
    public static class SessionManager
    {
        // Static properties to store session data
        public static int CurrentUserID { get; set; }
        public static string CurrentUsername { get; set; }
        public static string CurrentUserRole { get; set; }
    }

}
