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
        public static void Execute(PdfBatchEditData data, string dbAccessDataFileName)
        {
            string templateFilePath = Path.Combine(Utils.MainDirectory, "template_data", dbAccessDataFileName);
            DBAccessData accessData = DBAccessData.FromFile(templateFilePath);
            accessData.CustomizeSQLWithCommandLineArguments("ARGUMENT");
            Console.WriteLine($"SQL Command:  {accessData.sql}");

            List<DBRecord> records = LoadAndCorrectData(accessData);

            TextEffect effect = new TextEffect("");
            effect.UseLocalTexts = true;
            effect.FontColor = XColors.Black;
            effect.FontSize = 20;
            effect.VerticalAlignment = VerticalAlignment.Bottom;
            effect.HorizontalAlignment = HorizontalAlignment.Left;
            effect.RelativeX = 0.03;
            effect.RelativeY = 0.95;
            data.AddEffectToAllFiles(effect);

            foreach(DBRecord record in records)
            {
                BatchFile file = data.AddFileWithAllEffects(record.path);
                LocalTextEffectSettings settings = (LocalTextEffectSettings)file.GetLocalSettingsForEffect(effect);
                settings.Text = record.text;
            }
        }

        public static List<DBRecord> LoadAndCorrectData(DBAccessData accessData)
        {
            List<DBRecord> records = ReadDataBase(accessData);
            return CorrectMultifileRecords(records);
        }

        private static List<DBRecord> ReadDataBase(DBAccessData accessData)
        {
            string accessString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + accessData.path;

            OleDbConnection connection = new OleDbConnection(accessString);
            connection.Open();
            
            string[] restrictions = new string[4];
            restrictions[3] = "Table";
            DataTable userTables = connection.GetSchema("Tables", restrictions);
            foreach (DataRow row in userTables.Rows)
            {
                Console.WriteLine(row[2].ToString());
            }


            OleDbCommand accessCommand = new OleDbCommand(accessData.sql, connection);
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(accessCommand);

            connection.Close();

            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, accessData.tableName);


            DataTable table = dataSet.Tables[accessData.tableName];
            List<DBRecord> records = new List<DBRecord>();
            foreach (DataRow row in table.Rows)
            {
                string address = ExtractAddress((string)row[accessData.addressFieldName]);
                string text = Convert.ToString(row[accessData.textFieldName]);
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
                records.Add(new DBRecord(item.Key, String.Join(", ", item.Value)));
            }
            return records;
        }

        public struct DBAccessData
        {
            public string path;
            public string tableName;
            public string sql;
            public string addressFieldName;
            public string textFieldName;

            public DBAccessData(string path, string tableName, string sql, string addressFieldName, string textFieldName)
            {
                this.path = path;
                this.tableName = tableName;
                this.sql = sql;
                this.addressFieldName = addressFieldName;
                this.textFieldName = textFieldName;
            }

            public static DBAccessData FromFile(string accessDataFilepath)
            {                
                if (!File.Exists(accessDataFilepath))
                {
                    throw new Exception($"{accessDataFilepath} file not found");
                }

                Dictionary<string, string> data = ReadFileData(accessDataFilepath);

                string path, tableName, sql, addressFieldName, textFieldName;

                data.TryGetValue("PATH", out path);
                data.TryGetValue("SQL", out sql);
                data.TryGetValue("TABLE", out tableName);
                data.TryGetValue("ADDRESS_FIELD", out addressFieldName);
                data.TryGetValue("TEXT_FIELD", out textFieldName);

                if (path == null || sql == null || tableName == null || addressFieldName == null || textFieldName == null)
                {
                    throw new Exception(String.Join(Environment.NewLine,
                        "Missing parameters in the db_access.txt file.",
                        "    Needs the parameters PATH, SQL, TABLE, ADDRESS_FIELD and TEXT_FIELD"));
                }

                return new DBAccessData(path, tableName, sql, addressFieldName, textFieldName);
            }

            private static Dictionary<string, string> ReadFileData(string filepath)
            {
                var dict = new Dictionary<string, string>();
                foreach (string line in File.ReadLines(filepath))
                {
                    string[] split = line.Split(new char[] { '=' }, 2);
                    string id = split[0].Trim();
                    string value = split[1].Trim();

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
