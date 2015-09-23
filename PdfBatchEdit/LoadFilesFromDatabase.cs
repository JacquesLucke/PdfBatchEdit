using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;

namespace PdfBatchEdit
{
    class LoadFilesFromDatabase : IBatchFilesGenerator
    {
        public List<BatchFile> LoadBatchFiles()
        {
            string accessString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\\Users\\Jacques Lucke\\Desktop\\test.accdb";
            OleDbConnection connection;
            connection = new OleDbConnection(accessString);
            connection.Open();
            string sql = "SELECT ID, SCHMELZE FROM Zeugnis_Browser";
            Console.WriteLine(connection.State);
            OleDbCommand accessCommand = new OleDbCommand(sql, connection);
            OleDbDataAdapter dataAdapter = new OleDbDataAdapter(accessCommand);
            DataSet dataSet = new DataSet();
            dataAdapter.Fill(dataSet, "Zeugnis_Browser");
            DataTableCollection data = dataSet.Tables;
            DataTable table = dataSet.Tables["Zeugnis_Browser"];
            DataColumn column = table.Columns["SCHMELZE"];
            DataRowCollection rows = table.Rows;
            foreach (DataRow row in rows)
            {
                Console.WriteLine("ID: {0} \t\tSchmelze: {1}", row["ID"], row["SCHMELZE"]);
            }
            connection.Close();

            return new List<BatchFile>();
        }
    }
}
