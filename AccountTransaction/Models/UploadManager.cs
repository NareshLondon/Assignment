using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace AccountTransaction.Models
{
    public class UploadManager
    {
        protected const int _batchSize = 100000;
        public static string ProcessBulkCopy(HttpPostedFileBase uploadFile)
        {
            string fileName = Path.GetFileName(uploadFile.FileName);

            string path = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("~/App_Data/Upload"), fileName);
            if (File.Exists(path)) File.Delete(path);
            //string pathToSave = Server.MapPath("~/Content/");
            uploadFile.SaveAs(path);
            string Feedback = string.Empty;
            string line = string.Empty;
            string[] strArray;

            var dataTable = new DataTable("TransactionData");

            // Add the columns in the temp table
            dataTable.Columns.Add("Account", typeof(string));
            dataTable.Columns.Add("Amount", typeof(decimal));
            dataTable.Columns.Add("Description", typeof(string));
            dataTable.Columns.Add("CurrencyCode", typeof(string));
            DataRow row;

            // start This code can be used but I need more time to implement further.
            //var lines = File.ReadAllLines(path).Select(a => a.Split(';'));
            //var csv = (from lineItem in lines
            //           where lineItem.Any(a => !string.IsNullOrWhiteSpace(a))
            //           select (from col in line
            //                   select col).Skip(1).ToArray()

            //          ).Skip(2).ToArray();
            //Array.ForEach(csv, c => dataTable.Columns.Add());
            //end
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");

            //Set the filename in to our stream
            StreamReader sr = new StreamReader(path);
            var LineNbr = 0;
            //Read the first line and split the string at , with our regular expression in to an array
            line = sr.ReadLine();
            strArray = r.Split(line);

            //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
            Array.ForEach(strArray, s => dataTable.Columns.Add());

            //Read each line in the CVS file until it’s empty
            while ((line = sr.ReadLine()) != null)
            {
                LineNbr++;
                row = dataTable.NewRow();

                //add our current value to our data row
                row.ItemArray = r.Split(line);
                if (!IsNumeric(row[1].ToString()))
                {
                    //throw new Exception("Amount is not numeric on line " + LineNbr + " in " + fileName);
                }
                if (row[3].ToString().Length > 3)
                {
                    //throw new Exception("Currency Code is invalid on line " + LineNbr + " in " + fileName);
                }

                dataTable.Rows.Add(row);
            }
            sr.Dispose();

            var uploadResult = "";
            try
            {
                uploadResult = ProcessBulkCopy(dataTable);
            }
            catch (Exception e)
            {

            }
            return uploadResult;
        }
        private static string ProcessBulkCopy(DataTable tData)
        {
            string Feedback = string.Empty;
            string connString = ConfigurationManager.ConnectionStrings["FileUpload"].ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();


                using (var copy = new SqlBulkCopy(conn))
                {
                    copy.ColumnMappings.Add(0, 1);
                    copy.ColumnMappings.Add(1, 2);
                    copy.ColumnMappings.Add(2, 3);
                    copy.ColumnMappings.Add(3, 4);

                    copy.DestinationTableName = "TransactionData";
                    copy.BatchSize = tData.Rows.Count;
                    var count = tData.Rows.Count;
                    try
                    {
                        copy.WriteToServer(tData);
                        tData.Rows.Clear();
                        Feedback = "File Upload complete, total records inserted:" + count;
                    }
                    catch (Exception ex)
                    {
                        Feedback = ex.Message;
                    }
                }
            }

            return Feedback;
        }
        public static bool IsNumeric(string NumericText)
        {
            bool isnumber = true;
            foreach (char c in NumericText)
            {
                isnumber = char.IsNumber(c);
                if (!isnumber)
                {
                    return false;
                }
            }
            return isnumber;
        }


    }
}