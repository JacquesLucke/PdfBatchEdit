using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using PdfBatchEdit.Effects;
using PdfSharp.Drawing;
using System.IO;

namespace PdfBatchEdit.Templates
{
    class ReadFromDataBaseTemplate
    {
        public static void Execute(PdfBatchEditData data, string databasePath, string dbAccessDataFileName)
        {
            if (!File.Exists(databasePath))
            {
                Console.WriteLine($"Database not found: '{databasePath}'");
                return;
            }

            string templateFilePath = Path.Combine(Utils.MainDirectory, "template_data", dbAccessDataFileName);
            if (!File.Exists(templateFilePath))
            {
                Console.WriteLine($"Template file not found: '{templateFilePath}'");
                return;
            }

            DBAccessData accessData = DBAccessData.FromFile(templateFilePath);
            accessData.CustomizeSQLWithCommandLineArguments("ARGUMENT");
            Console.WriteLine(accessData);
            Console.WriteLine();

            List<DBRecord> records = LoadAndCorrectData(databasePath, accessData);

            TextEffect effect = new TextEffect("");
            effect.UseLocalTexts = true;
            effect.FontColor = XColors.Black;
            effect.FontSize = 20;
            effect.VerticalAlignment = VerticalAlignment.Top;
            effect.HorizontalAlignment = HorizontalAlignment.Right;
            effect.RelativeX = 0.96;
            effect.RelativeY = 0.01;
            effect.UseOrientation = true;
            data.AddEffectToAllFiles(effect);

            int counter = 0;
            foreach(DBRecord record in records)
            {
                if (File.Exists(record.path))
                {
                    BatchFile file = data.AddFileWithAllEffects(record.path);
                    LocalTextEffectSettings settings = (LocalTextEffectSettings)file.GetLocalSettingsForEffect(effect);
                    settings.Text = "Pos. " + record.text;
                    counter++;
                }
                else
                {
                    Console.WriteLine($"File '{record.path}' not found.");
                }
            }

            Console.WriteLine($"{counter} files loaded.");
        }

        public static List<DBRecord> LoadAndCorrectData(string databasePath, DBAccessData accessData)
        {
            List<DBRecord> records = ReadDataBase(databasePath, accessData);
            return CorrectMultifileRecords(records);
        }

        private static List<DBRecord> ReadDataBase(string databasePath, DBAccessData accessData)
        {
            string accessString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + databasePath;
            OleDbConnection connection = new OleDbConnection(accessString);
            connection.Open();

            OleDbCommand accessCommand = new OleDbCommand(accessData.sql, connection);
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(accessCommand);

            connection.Close();

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, accessData.tableName);

            DataTable table = dataSet.Tables[accessData.tableName];
            List<DBRecord> records = new List<DBRecord>();

            if (!table.Columns.Contains(accessData.addressFieldName))
                Console.WriteLine($"The table has no '{accessData.addressFieldName}' field.");

            if (!table.Columns.Contains(accessData.textFieldName))
                Console.WriteLine($"The table has no '{accessData.textFieldName}' field.");

            foreach (DataRow row in table.Rows)
            {
                string address = "";
                if (table.Columns.Contains(accessData.addressFieldName))
                    address = ExtractAddress(Convert.ToString(row[accessData.addressFieldName]));

                string text = "";
                if (table.Columns.Contains(accessData.textFieldName))
                    text = Convert.ToString(row[accessData.textFieldName]);

                records.Add(new DBRecord(address, text));
            }


            return records;
        }

        private static string ExtractAddress(string link)
        {
            if (link.Length < 2) return String.Empty;
            if (link[0] == '#' && link[1] != '#')
            {
                return link.Substring(1, link.IndexOf('#', 1) - 1).Trim();
            }
            if (link[0] != '#')
            {
                int firstIndex = link.IndexOf('#');
                int secondIndex = link.IndexOf('#', firstIndex + 1);
                return link.Substring(firstIndex + 1, secondIndex - firstIndex - 1).Trim();
            }
            return String.Empty;
        }

        private static List<DBRecord> CorrectMultifileRecords(List<DBRecord> sourceRecords)
        {
            Dictionary<string, List<string>> textsByPath = new Dictionary<string, List<string>>();

            foreach(DBRecord record in sourceRecords)
            {
                if (!textsByPath.ContainsKey(record.path))
                    textsByPath[record.path] = new List<string>();

                textsByPath[record.path].Add(record.text);
            }

            List<DBRecord> records = new List<DBRecord>();
            foreach(KeyValuePair<string, List<string>> item in textsByPath)
            {
                records.Add(new DBRecord(item.Key, String.Join("/", item.Value)));
            }
            return records;
        }

        public struct DBAccessData
        {
            public string tableName;
            public string sql;
            public string addressFieldName;
            public string textFieldName;

            public DBAccessData(string tableName, string sql, string addressFieldName, string textFieldName)
            {
                this.tableName = tableName;
                this.sql = sql;
                this.addressFieldName = addressFieldName;
                this.textFieldName = textFieldName;
            }

            public override string ToString()
            {
                return String.Join("\n",
                    "Table:         " + tableName,
                    "SQL:           " + sql,
                    "Address Field: " + addressFieldName,
                    "Text Field:    " + textFieldName);
            }

            public static DBAccessData FromFile(string accessDataFilepath)
            {                
                if (!File.Exists(accessDataFilepath))
                {
                    throw new Exception($"{accessDataFilepath} file not found");
                }

                Dictionary<string, string> data = ReadFileData(accessDataFilepath);

                string tableName, sql, addressFieldName, textFieldName;
                
                data.TryGetValue("SQL", out sql);
                data.TryGetValue("TABLE", out tableName);
                data.TryGetValue("ADDRESS_FIELD", out addressFieldName);
                data.TryGetValue("TEXT_FIELD", out textFieldName);

                if (sql == null || tableName == null || addressFieldName == null || textFieldName == null)
                {
                    throw new Exception(String.Join(Environment.NewLine,
                        "Missing parameters in the db_access.txt file.",
                        "    Needs the parameters SQL, TABLE, ADDRESS_FIELD and TEXT_FIELD"));
                }

                return new DBAccessData(tableName, sql, addressFieldName, textFieldName);
            }

            private static Dictionary<string, string> ReadFileData(string filepath)
            {
                var dict = new Dictionary<string, string>();
                foreach (string line in File.ReadLines(filepath))
                {
                    string[] split = line.Split(new char[] { '=' }, 2);
                    string id = split[0].Trim();
                    string value = "";
                    if (split.Length == 2)
                    {
                        value = split[1].Trim();
                    }

                    dict[id] = value;
                }
                return dict;
            }

            public void CustomizeSQLWithCommandLineArguments(string prefix)
            {
                Dictionary<string, string> args = Utils.GetArgumentsDictionary();
                foreach (string key in args.Keys)
                {
                    if (key.StartsWith(prefix))
                    {
                        sql = sql.Replace("{" + key + "}", args[key]);
                    }
                }
            }
        }

        public struct DBRecord
        {
            public string path;
            public string text;

            public DBRecord(string path, string text)
            {
                this.path = path;
                this.text = text;
            }
        }
    }
}
