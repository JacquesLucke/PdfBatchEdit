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
            effect.UseLocalTexts = accessData.useLocalTexts;
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

                    file.OutputNameType = accessData.outputNameType;
                    int prefixNumber = batchFileData.GetLowestSortElement();
                    string prefixNumberString = Convert.ToString(prefixNumber).PadLeft(3, '0');

                    if (accessData.outputNameType == FileNameType.SourceName)
                        file.OutputNamePrefix = "";
                    if (accessData.outputNameType == FileNameType.PrefixOnly)
                        file.OutputNamePrefix = prefixNumberString;
                    if (accessData.outputNameType == FileNameType.SourceNameAndPrefix)
                        file.OutputNamePrefix = prefixNumberString + "_";

                    LocalTextEffectSettings settings = (LocalTextEffectSettings)file.GetLocalSettingsForEffect(effect);
                    settings.Text = accessData.textPrefix + batchFileData.CombinedText;
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

            if (!table.Columns.Contains(accessData.sortFieldName))
                Console.WriteLine($"The table has no '{accessData.sortFieldName}' field.");

            foreach (DataRow row in table.Rows)
            {
                string address = "";
                if (table.Columns.Contains(accessData.addressFieldName))
                    address = ExtractAddress(Convert.ToString(row[accessData.addressFieldName]));

                string text = "";
                if (table.Columns.Contains(accessData.textFieldName))
                    text = Convert.ToString(row[accessData.textFieldName]);

                string sortElement = "";
                if (table.Columns.Contains(accessData.sortFieldName))
                    sortElement = Convert.ToString(row[accessData.sortFieldName]);

                records.Add(new DBRecord(address, text, sortElement));
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
                data.AddEntry(record.text, record.sortElement);
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
            public string sortFieldName;
            public string textPrefix;
            public bool useLocalTexts;
            public FileNameType outputNameType;

            public DBAccessData(string tableName, string sql, string addressFieldName, string textFieldName, string sortFieldName, string textPrefix, bool useLocalTexts, FileNameType outputNameType)
            {
                this.tableName = tableName;
                this.sql = sql;
                this.addressFieldName = addressFieldName;
                this.textFieldName = textFieldName;
                this.sortFieldName = sortFieldName;
                this.textPrefix = textPrefix;
                this.useLocalTexts = useLocalTexts;
                this.outputNameType = outputNameType;
            }

            public override string ToString()
            {
                return String.Join("\n",
                    "Table:            " + tableName,
                    "SQL:              " + sql,
                    "Address Field:    " + addressFieldName,
                    "Text Field:       " + textFieldName,
                    "Sort Field:       " + sortFieldName,
                    "Text Prefix:      '" + textPrefix + "'",
                    "Use Local Texts:  " + useLocalTexts,
                    "Output Name Type: " + outputNameType);
            }

            public static DBAccessData FromFile(string accessDataFilepath)
            {                
                if (!File.Exists(accessDataFilepath))
                {
                    throw new Exception($"{accessDataFilepath} file not found");
                }

                Dictionary<string, string> data = ReadFileData(accessDataFilepath);

                string tableName, sql, addressFieldName, textFieldName, sortFieldName, textPrefix, _useLocalTexts, _outputNameType;
                
                data.TryGetValue("SQL", out sql);
                data.TryGetValue("TABLE", out tableName);
                data.TryGetValue("ADDRESS_FIELD", out addressFieldName);
                data.TryGetValue("TEXT_FIELD", out textFieldName);
                data.TryGetValue("SORT_FIELD", out sortFieldName);
                data.TryGetValue("TEXT_PREFIX", out textPrefix);
                data.TryGetValue("USE_LOCAL_TEXTS", out _useLocalTexts);
                data.TryGetValue("OUTPUT_NAME_TYPE", out _outputNameType);

                if (sql == null || tableName == null || addressFieldName == null || textFieldName == null || sortFieldName  == null || textPrefix == null || _useLocalTexts == null || _outputNameType == null)
                {
                    throw new Exception(String.Join(Environment.NewLine,
                        "Missing parameters in the db_access.txt file.",
                        "    Needs the parameters SQL, TABLE, ADDRESS_FIELD, TEXT_FIELD, SORT_FIELD, TEXT_PREFIX, USE_LOCAL_TEXTS and OUTPUT_NAME_TYPE"));
                }

                if(textPrefix.StartsWith("'") && textPrefix.EndsWith("'") && textPrefix.Length >= 2)
                {
                    textPrefix = textPrefix.Substring(1, textPrefix.Length - 2);
                }

                bool useLocalTexts = false;
                try { useLocalTexts = (bool)Convert.ToBoolean(_useLocalTexts); }
                catch { Console.WriteLine($"'{_useLocalTexts}' is not convertable to true or false"); }

                FileNameType outputNameType = FileNameType.SourceNameAndPrefix;
                if (_outputNameType == "NUMBER_ONLY")
                    outputNameType = FileNameType.PrefixOnly;
                else if (_outputNameType == "NUMBER_AND_SOURCE_NAME")
                    outputNameType = FileNameType.SourceNameAndPrefix;
                else if (_outputNameType == "SOURCE_NAME")
                    outputNameType = FileNameType.SourceName;
                else
                    throw new Exception("OUTPUT_NAME_TYPE has to be in ['NUMBER_ONLY', 'NUMBER_AND_SOURCE_NAME', 'SOURCE_NAME']");

                return new DBAccessData(tableName, sql, addressFieldName, textFieldName, sortFieldName, textPrefix, useLocalTexts, outputNameType);
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
            public string sortElement;

            public DBRecord(string path, string text, string sortElement)
            {
                this.path = path;
                this.text = text;
                this.sortElement = sortElement;
            }
        }

        public struct BatchFileData
        {
            public string path;
            public List<string> texts;
            public List<string> sortElements;

            public BatchFileData(string path)
            {
                this.path = path;
                this.texts = new List<string>();
                this.sortElements = new List<string>();
            }

            public void AddEntry(string text, string sortElement)
            {
                if (!texts.Contains(text))
                {
                    texts.Add(text);
                }
                if (!sortElements.Contains(sortElement))
                {
                    sortElements.Add(sortElement);
                }
                texts.Sort();
            }

            public string CombinedText
            {
                get { return String.Join("/", texts); }
            }

            public int GetLowestSortElement()
            {
                int min = int.MaxValue;
                foreach (string element in sortElements)
                {
                    try
                    {
                        int value = Convert.ToInt32(element);
                        if (value < min) min = value;
                    }
                    catch
                    {
                        Console.WriteLine($"'{element}' is not a number and can't be sorted.");
                    }
                }
                return min;
            }
            public static int SortByNumber(BatchFileData a, BatchFileData b)
            {
                int value1 = a.GetLowestSortElement();
                int value2 = b.GetLowestSortElement();
                if (value1 < value2) return -1;
                if (value1 > value2) return 1;
                return 0;
            }
        }
    }
}
