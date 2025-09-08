using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using SCH = SQL_And_Config_Handler;

namespace OMPS
{
    public class SqlMethods
    {
        public static async Task<List<object[]>?> GetJobOrderLines(object[] fields, object[] filters, int limit = 0)
        {
            MessageBox.Show(String.Join(" AND ", filters.Select(f => $"({f})")));
            Clipboard.SetText(String.Join(" AND ", filters.Select(f => $"({f})")));
            try
            {
                var t = SCH.SqlTemplateHandler.LoadFromName("JobLineItems");
                if (t.Item1 is false || t.Item2 is null)
                {
                    return null;
                }
                var resp = await t.Item2.AsyncQuery(
                    new() {
                        { "limit", (limit is 0 ? "TOP 20" : $"TOP {limit}") },
                        { "filters", String.Join(" AND ", filters.Select(f => $"({f})")) }
                    }
                );
                List<object[]> ret = [];
                for (global::System.Int32 i = 0; i < resp.ToArray().AsSpan().Length; i++)
                {
                    ret.Add([.. fields.Select(k => { resp[i].TryGetValue($"{k}", out object? value); return value; }).Where(o => o is not null)]);
                    /*
                    var keys = resp[i].Keys.ToArray();
                    for (global::System.Int32 j = 0; j < keys.Length; j++)
                    {
                        if (fields.Contains(keys[j]))
                        {
                            continue;
                        }
                        resp[i].Remove(keys[j]);
                    }
                    */
                }
                return ret;
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\n" + ex.StackTrace);
            }
            return null;
        }
    }
}
