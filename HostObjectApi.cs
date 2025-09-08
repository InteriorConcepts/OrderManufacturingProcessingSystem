using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SCH = SQL_And_Config_Handler;

namespace OMPS
{
    [ComVisible(true)]
    public class HostObjectApi
    {
        static int MAX_LIMIT = int.MaxValue;
        public async Task<Dictionary<string, string>[]> GetOrders(string job, int limit = 0)
        {
            if (limit is 0)
            {
                limit = MAX_LIMIT;
            }
            return null;
        }

        public async Task<int> Test(IList<string> fields)
        {
            return 123;
        }

        public async Task<string?> GetJobOrderLines(object[] fields, object[] filters, int limit = 0)
        {
            var resp = await SqlMethods.GetJobOrderLines(fields, filters, limit);
            //MessageBox.Show(resp.ToString());
            if (resp is null)
            {
                return null;
            }
            return JsonSerializer.Serialize(resp);
        }
    }
}
