
using SQL_And_Config_Handler;
using SCH = SQL_And_Config_Handler;

namespace OMPS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var initRes = SCH.SQLDatabaseConnection.Init();
            if (initRes.Item1 is false && initRes.Item2 != null)
            {
                MessageBox.Show(initRes.Item2.Message + "\n" + initRes.Item2.StackTrace);
                Application.Exit();
            }

            List<(string, bool, Exception?)> SqlLoadErrors = [];
            Parallel.ForEach(new DirectoryInfo($"{SCH.Global.Config["sql.query-files-dir"]}").GetFiles("*.toml", SearchOption.TopDirectoryOnly), o =>
            {
                var name = o.Name.Split('.', StringSplitOptions.RemoveEmptyEntries)[0];
                try
                {
                    var res = SCH.SqlTemplateHandler.LoadFromName(name);
                    if (res.Item1 is false || res.Item2 is null)
                    {
                        SqlLoadErrors.Add((name, res.Item1, new("[Error] in SqlTemplateHandler: Couldn't load template: " + name)));
                        return;
                    }
                    //SqlTemplateLoadResults.Add((name, true, null));
                }
                catch (Exception ex)
                {
                    SqlLoadErrors.Add((name, false, ex));
                }
            });
            if (SqlLoadErrors.Any()) {
                MessageBox.Show(String.Join("\n\n", SqlLoadErrors.Select(i => i.Item3?.Message)));
            }

            Application.Run(new Form1());
        }
    }
}