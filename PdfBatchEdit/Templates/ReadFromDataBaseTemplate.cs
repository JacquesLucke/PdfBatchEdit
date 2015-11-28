using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using PdfBatchEdit.Effects;
using PdfSharp.Drawing;
using System.IO;

namespace PdfBatchEdit.Templates
{
    class ReadFromDataBaseTemplate : ITemplate
    {
        public void Execute(PdfBatchEditData data)
        {
            /*
            string[] args = Environment.GetCommandLineArgs();
            if (args.Length != 3) return;
            if (args[1] != "-argument") return;
            string argument = args[2];
            */
            string argument = "105";
            
            string templateFilePath = Path.Combine(Utils.MainDirectory, "templates", "db_access.txt");
            if (!File.Exists(templateFilePath))
            {
                Console.WriteLine("templates\\db_access.txt file not found");
                return;
            }
            string 
                path = "",
                tableName = "",
                sql = "",
                addressFieldName = "",
                textFieldName = "";

            foreach (string line in File.ReadLines(templateFilePath))
            {
                string[] split = line.Split(new char[] { '=' }, 2);
                string id = split[0].Trim();
                string value = split[1].Trim();

                if (id == "PATH") path = value;
                if (id == "SQL") sql = value;
                if (id == "TABLE") tableName = value;
                if (id == "ADDRESS_FIELD") addressFieldName = value;
                if (id == "TEXT_FIELD") textFieldName = value;
            }

            if (path == "" || sql == "" || tableName == "" || addressFieldName == "" || textFieldName == "")
            {
                Console.WriteLine("Not all parameters in the db_access.txt files found.");
                Console.WriteLine("Needs the parameters PATH, SQL, TABLE, ADDRESS_FIELD and TEXT_FIELD");
                return;
            }


            sql = sql.Replace("{ARGUMENT}", argument);
            Console.WriteLine(sql);

            List<DBRecord> records = ReadDataBase(path, tableName, sql, addressFieldName, textFieldName);
            records = CorrectMultifileRecords(records);

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
                BatchFile file = data.AddFileWithAllEffects(record.address);
                LocalTextEffectSettings settings = (LocalTextEffectSettings)file.GetLocalSettingsForEffect(effect);
                settings.Text = record.text;
            }
        }

        private List<DBRecord> ReadDataBase(string path, string tableName, string sql, string addressFieldName, string textFieldName)
        {
            string accessString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path;
            try {
                OleDbConnection connection = new OleDbConnection(accessString);
                connection.Open();


                string[] restrictions = new string[4];
                restrictions[3] = "Table";
                DataTable userTables = connection.GetSchema("Tables", restrictions);
                foreach(DataRow row in userTables.Rows)
                {
                    Console.WriteLine(row[2].ToString());
                }


                OleDbCommand accessCommand = new OleDbCommand(sql, connection);
                OleDbDataAdapter dataAdapter = new OleDbDataAdapter(accessCommand);

                connection.Close();

                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, tableName);


                DataTable table = dataSet.Tables[tableName];
                List<DBRecord> records = new List<DBRecord>();
                foreach (DataRow row in table.Rows)
                {
                    string address = ExtractAddress((string)row[addressFieldName]);
                    string text = Convert.ToString(row[textFieldName]);
                    records.Add(new DBRecord(address, text));
                }

                return records;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new List<DBRecord>();
            }
        }

        private string ExtractAddress(string link)
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

        private List<DBRecord> CorrectMultifileRecords(List<DBRecord> sourceRecords)
        {
            Dictionary<string, List<string>> textsByPath = new Dictionary<string, List<string>>();

            foreach(DBRecord record in sourceRecords)
            {
                if (!textsByPath.ContainsKey(record.address))
                    textsByPath[record.address] = new List<string>();

                textsByPath[record.address].Add(record.text);
            }

            List<DBRecord> records = new List<DBRecord>();
            foreach(KeyValuePair<string, List<string>> item in textsByPath)
            {
                records.Add(new DBRecord(item.Key, String.Join(", ", item.Value)));
            }
            return records;
        }
        
        struct DBRecord
        {
            public string address;
            public string text;

            public DBRecord(string address, string text)
            {
                this.address = address;
                this.text = text;
            }
        }
    }
}
