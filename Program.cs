
using SQL_And_Config_Handler;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Graphics.Printing.Workflow;
using Windows.Media.Protection.PlayReady;
using SCH = SQL_And_Config_Handler;

using SqlParser;
using SqlParser.Ast;


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
            /*
            var parser = new SqlQueryParser();
            var ast = parser.Parse(@""
            );
            
            foreach (var stmt in ast)
            {
                MessageBox.Show(stmt.ToSql());
                if (stmt is null) continue;
                var bar = stmt.AsSelect();
                bar.Deconstruct(out Query query);
                List<string> selectNames = [];
                foreach (var names in query.Body.AsSelect().Projection)
                {
                    MessageBox.Show(names.ToSql());
                    selectNames.Add("> " + names.AsUnnamed().Expression.AsIdentifier().Ident.Value);// (names as Expression).AsIdentifier().Ident.Value;
                }
                MessageBox.Show(string.Join("\n", selectNames));
            }
            */

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            var initRes = SCH.SQLDatabaseConnection.Init();
            if (initRes.Item1 is false && initRes.Item2 != null)
            {
                MessageBox.Show(initRes.Item2.Message + "\n" + initRes.Item2.StackTrace);
                Application.Exit();
            }
            //


            //
            ExportHostObjectJS().Wait();

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
            if (SqlLoadErrors.Count is not 0) {
                MessageBox.Show(String.Join("\n\n", SqlLoadErrors.Select(i => i.Item3?.Message)));
            }

            Application.Run(new Form1());
        }

        public static async Task<Dictionary<string, Dictionary<string, object?>?>> RunAll()
        {
            Dictionary<string, object> TestData = new()
            {
                { "limit", 1 },
                { "job", "J000035601" }
            };
            Debug.WriteLine("start");
            Dictionary<string, Dictionary<string, object?>?> returnValues = [];
            var methods = GlobalObjects.SqlMethods.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (var method in methods)
            {
                if (method.Name is "GetSql") continue;
                Debug.WriteLine(method.Name);
                var @params = method.GetParameters().Select(p => p.Name).Where(p => p is not null).ToList();
                object[] values = [.. @params.Select(p => TestData[p])];
                var @return = method.Invoke(GlobalObjects.SqlMethods, values);
                if (@return is null)
                {
                    continue;
                }
                var task = (Task<List<Dictionary<string, object?>?>>)@return;
                var result = await task;
                if (result is null || result.Count is 0)
                {
                    continue;
                }
                returnValues.Add(method.Name, result.FirstOrDefault());
                //
            }
            Debug.WriteLine("end");
            return returnValues;
        }



        // https://stackoverflow.com/a/64023495
        public static string FromSqlType(string sqlTypeString)
        {
            if (!Enum.TryParse(sqlTypeString, out SQLType typeCode))
            {
                throw new Exception("sql type not found");
            }
            switch (typeCode)
            {
                case SQLType.varbinary:
                case SQLType.binary:
                case SQLType.filestream:
                case SQLType.image:
                case SQLType.rowversion:
                case SQLType.timestamp://?
                    return "byte[]";
                case SQLType.tinyint:
                    return "byte";
                case SQLType.varchar:
                case SQLType.nvarchar:
                case SQLType.nchar:
                case SQLType.text:
                case SQLType.ntext:
                case SQLType.xml:
                    return "string";
                case SQLType.@char:
                    return "char";
                case SQLType.bigint:
                    return "long";
                case SQLType.bit:
                    return "bool";
                case SQLType.smalldatetime:
                case SQLType.datetime:
                case SQLType.date:
                case SQLType.datetime2:
                    return "DateTime";
                case SQLType.datetimeoffset:
                    return "DateTimeOffset";
                case SQLType.@decimal:
                case SQLType.money:
                case SQLType.numeric:
                case SQLType.smallmoney:
                    return "decimal";
                case SQLType.@float:
                    return "double";
                case SQLType.@int:
                    return "int";
                case SQLType.real:
                    return "Single";
                case SQLType.smallint:
                    return "short";
                case SQLType.uniqueidentifier:
                    return "Guid";
                case SQLType.sql_variant:
                    return "object";
                case SQLType.time:
                    return "TimeSpan";
                default:
                    throw new Exception("none equal type");
            }
        }

        public enum SQLType
        {
            varbinary,//(1)
            binary,//(1)
            image,
            varchar,
            @char,
            nvarchar,//(1)
            nchar,//(1)
            text,
            ntext,
            uniqueidentifier,
            rowversion,
            bit,
            tinyint,
            smallint,
            @int,
            bigint,
            smallmoney,
            money,
            numeric,
            @decimal,
            real,
            @float,
            smalldatetime,
            datetime,
            sql_variant,
            table,
            cursor,
            timestamp,
            xml,
            date,
            datetime2,
            datetimeoffset,
            filestream,
            time,
        }

        class DbNull() { };

        static async Task ExportHostObjectJS()
        {

            await GlobalObjects.SqlMethods.GetTypes("aIC_IceManuf");
            return;
            //
            var v = await RunAll();
            MessageBox.Show(string.Join(", ", v.Keys));
            var returnTypeClasses = "";
            foreach (var methodResult in v)
            {
                var name = methodResult.Key;
                if (methodResult.Value is null)
                {
                    MessageBox.Show($"Failed: {name}");
                    continue;
                }
                var returnTypeClass = $"class {name}_Result {{\n";
                var types = methodResult.Value.Select(v => (v.Value ?? new object()).GetType());
                foreach (var field in methodResult.Value)
                {
                    var typeName = "any";
                    if (field.Value is not null)
                    {
                        typeName = field.Value.GetType().Name;
                    }
                    returnTypeClass += $"\t/** @type {{{typeName}}} */\n\t\"{field.Key}\";\n";
                }
                returnTypeClass += "}";
                returnTypeClasses += returnTypeClass + "\n";
            }
            File.WriteAllText($"{Form1.WWWRootDirectory()}/js/test.js", returnTypeClasses);
            
            //
            //
            //

            var jsTypeMap = new Dictionary<string, string>() {
                { "byte", "number" },
                { "int", "number" },
                { "int16", "number" },
                { "int32", "number" },
                { "int64", "number" },
                { "int128", "number" },
                { "object", "any" },
                { "string", "string" },
            };
            var methods = typeof(HostObjectApi).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var methodsJs = "";
            foreach (var method in methods)
            {
                var methodName = method.Name;
                if (methodName is "RunAll")
                {
                    continue;
                }
                var @params = method.GetParameters();
                var paramTypeAnnotation = "/**";
                var funcDefParamsStr = "";
                var paramNames = @params.
                    Where(p => p.Name is not null).
                    Select(p => $"{p.Name?.ToLower()}");
                var methodJs = $"async function {methodName}({{paramNames}}) {{\n";
                foreach (var param in @params)
                {
                    if (param.Name is null)
                    {
                        continue;
                    }
                    var name = param.Name.ToLower();
                    var type = param.ParameterType.Name.ToLower();
                    if (jsTypeMap.TryGetValue(type, out string? jsType) is false || jsType is null)
                    {
                        MessageBox.Show($"{methodName}\n{name} of type c# {type} doesn't have mappable js type");
                        continue;
                    }
                    paramTypeAnnotation += $"\n * @param {{{jsType}}} {name} ";
                    if (funcDefParamsStr.Length is not 0)
                    {
                        funcDefParamsStr += ", ";
                    }
                    funcDefParamsStr += $"{name}";
                    if (param.HasDefaultValue)
                    {
                        funcDefParamsStr += $" = {JsonSerializer.Serialize(param.DefaultValue)}";
                    }
                    methodJs +=
                        $"\tif (typeof {name} !== '{jsType}') {{\n\t\treturn null;\n\t}}" +
                        Environment.NewLine;
                }
                methodJs = $"{paramTypeAnnotation}\n * @returns {{string|null}}\n */\n{methodJs}";
                methodJs = methodJs.Replace("{paramNames}", funcDefParamsStr);
                methodJs += $"\treturn (await HostObj().{methodName}({string.Join(", ", paramNames)}));";
                methodJs += "\n}";
                methodsJs += methodJs + Environment.NewLine;
            }
            File.WriteAllText($"{Form1.WWWRootDirectory()}/js/sql-methods.js", methodsJs);
        }
    }
}