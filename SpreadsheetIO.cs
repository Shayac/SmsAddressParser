using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.GData.Client;
using Google.GData.Spreadsheets;

namespace GmailReceiver
{
    public class SpreadsheetIO
    {
        private readonly string _userName;
        private readonly string _password;
        private readonly SpreadsheetsService _service;
        private readonly SpreadsheetQuery _query;
        private readonly SpreadsheetFeed _feed;

        public SpreadsheetIO(string username, string password)
        {
            _userName = username;
            _password = password;
            _service = new SpreadsheetsService("MySpreadsheetIntegration-v1");
            _service.setUserCredentials(_userName, _password);
            _query = new SpreadsheetQuery();
            _feed = _service.Query(_query);
        }

        public IEnumerable<string> GetStreetNamesFromSpreadsheet(WorksheetEntry worksheet)
        {
            List<string> streetNames = new List<string>();
            // Loop through rows in worksheet and map out objects
            if (worksheet != null)
            {
                // Define the URL to request the list feed of the worksheet.
                AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

                // Fetch the list feed of the worksheet.
                ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
                ListFeed listFeed = _service.Query(listQuery);

                // Iterate through each row, printing its cell values.
                streetNames.AddRange(from ListEntry row in listFeed.Entries select row.Title.Text);
            }
            return streetNames;
        }

        public void InsertSMSIntoSpreadsheet(WorksheetEntry worksheet, Sms sms)
        {
            // Loop through rows in worksheet 
            if (worksheet != null)
            {
                // Define the URL to request the list feed of the worksheet.
                AtomLink listFeedLink = worksheet.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

                // Fetch the list feed of the worksheet.
                ListQuery listQuery = new ListQuery(listFeedLink.HRef.ToString());
                ListFeed listFeed = _service.Query(listQuery);


                // Create a local representation of the new row.
                ListEntry row = new ListEntry();
                row.Elements.Add(new ListEntry.Custom() {LocalName = "number", Value = sms.PhoneNumber});
                row.Elements.Add(new ListEntry.Custom() {LocalName = "message", Value = sms.Body});
                row.Elements.Add(new ListEntry.Custom() {LocalName = "date", Value = sms.Date.ToString()});
                if (sms.Address != null)
                {
                    row.Elements.Add(new ListEntry.Custom()
                        {
                            LocalName = "address",
                            Value = sms.Address.ToString()
                        });
                }

                // Send the new row to the API for insertion.
                _service.Insert(listFeed, row);
            }
        }


        public SpreadsheetEntry GetSpreadSheet(string spreadsheetName)
        {
            // Loop through spreadsheets and find matching spreadsheet
            SpreadsheetEntry spreadsheet = null;
            foreach (
                var s in
                    _feed.Entries.Where(
                        s => String.Equals(s.Title.Text, spreadsheetName, StringComparison.OrdinalIgnoreCase)))
            {
                spreadsheet = (SpreadsheetEntry) s;
            }
            return spreadsheet;
        }

        public WorksheetEntry GetWorksheet(SpreadsheetEntry spreadsheet, string worksheetName)
        {
            WorksheetEntry worksheet = null;
            // Loop through worksheets and find matching worksheet
            if (spreadsheet != null)
            {
                WorksheetFeed wsFeed = spreadsheet.Worksheets;
                foreach (
                    var w in
                        wsFeed.Entries.Where(
                            w => String.Equals(w.Title.Text, worksheetName, StringComparison.OrdinalIgnoreCase)))
                {
                    worksheet = (WorksheetEntry) w;
                }
            }
            return worksheet;
        }
    }
}