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

            List<BatchFileData> batchFileDataList = LoadAndCorrectData(databasePath, accessData);
            batchFileDataList.Sort(BatchFileData.SortByNumber);

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
            foreach(BatchFileData batchFileData in batchFileDataList)
            {
                if (File.Exists(batchFileData.path))
                {
                    BatchFile file = data.AddFileWithAllEffects(batchFileData.path);
                    int prefixNumber = batchFileData.GetLowestNumberInTexts();
                    file.OutputNamePrefix = Convert.ToString(prefixNumber).PadLeft(3, '0') + " ";
                    LocalTextEffectSettings settings = (LocalTextEffectSettings)file.GetLocalSettingsForEffect(effect);
                    settings.Text = "Pos. " + batchFileData.CombinedText;
                    counter++;
                }
                else
                {
                    Console.WriteLine($"File '{batchFileData.path}' not found.");
                }
            }

            Console.WriteLine($"{counter} files loaded.");
        }

        public static List<BatchFileData> LoadAndCorrectData(string databasePath, DBAccessData accessData)
        {
            List<DBRecord> records = ReadDataBase(databasePath, accessData);
            return CombineRecordsWithSameFile(records);
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

        private static List<BatchFileData> CombineRecordsWithSameFile(List<DBRecord> sourceRecords)
        {
            var batchFileData = new List<BatchFileData>();
            foreach(DBRecord record in sourceRecords)
            {
                var data = GetBatchFileDataWithPath(batchFileData, record.path);
                data.AddText(record.text);
                if (!batchFileData.Contains(data))
                {
                    batchFileData.Add(data);
                }
            }
            return batchFileData;
        }

        private static BatchFileData GetBatchFileDataWithPath(List<BatchFileData> existingDataList, string path)
        {
            foreach(BatchFileData data in existingDataList)
            {
                if (data.path == path) return data;
            }
            return new BatchFileData(path);
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

        public struct BatchFileData
        {
            public string path;
            public List<string> texts;

            public BatchFileData(string path)
            {
                this.path = path;
                this.texts = new List<string>();
            }

            public void AddText(string text)
            {
                texts.Add(text);
            }

            public string CombinedText
            {
                get { return String.Join("/", texts); }
            }

            public int GetLowestNumberInTexts()
            {
                int min = int.MaxValue;
                foreach (string text in texts)
                {
                    try
                    {
                        int value = Convert.ToInt32(text);
                        if (value < min) min = value;
                    }
                    catch
                    {
                        Console.WriteLine($"'{text}' is not a number and can't be sorted.");
                    }
                }
                return min;
            }
            public static int SortByNumber(BatchFileData a, BatchFileData b)
            {
                int value1 = a.GetLowestNumberInTexts();
                int value2 = b.GetLowestNumberInTexts();
                if (value1 < value2) return -1;
                if (value1 > value2) return 1;
                return 0;
            }
        }
    }
}
