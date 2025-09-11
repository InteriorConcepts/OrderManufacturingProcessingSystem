using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OMPS
{
    public class GlobalObjects
    {
        public static readonly Form1 MainForm = new();
        public static readonly SqlMethods SqlMethods = new();
        public static readonly MyApp.DataAccess.Generated.example_queriesQueries GeneratedQueries = new();
        public static readonly WebView_EventsHandler WV_EventHandler = new();
    }
}
