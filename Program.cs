using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MailKit;
using MailKit.Net.Imap;


namespace GmailReceiver
{
    internal class Program
    {
        
        public static void Main(string[] args)
        {
            Console.WriteLine("Enter Gmail account");
            string gmailAccount = Console.ReadLine();
            Console.WriteLine("Enter password");
            string password = Console.ReadLine();
            Console.WriteLine("Enter search date:");
            string dateString = Console.ReadLine();
            DateTime date;
            while (!DateTime.TryParse(dateString, out date))
            {
                Console.WriteLine("Error reading date. Please re-enter date:");
                dateString = Console.ReadLine();
            }

            Console.WriteLine("Processing text message list...");

            Receiver receiver = new Receiver(gmailAccount, password);
            var smsList = receiver.GetSmsList(date);
            Console.WriteLine("List processed successfully");

            Console.WriteLine("Retrieving spreadsheets");

            SpreadsheetIO spreadsheetManager = new SpreadsheetIO(gmailAccount, password);
            var spreadsheet = spreadsheetManager.GetSpreadSheet("SMS Messages");
            Console.WriteLine("Spreadsheets retrieved");

            var streetNames1Sheet = spreadsheetManager.GetWorksheet(spreadsheet,"StreetNames1");
           
            var streetNames2Sheet = spreadsheetManager.GetWorksheet(spreadsheet, "StreetNames2");
            

            Console.WriteLine("Loading street names....");

            SmsParser parser = new SmsParser();
            parser.ValidStreetNamesWithOutSpace = spreadsheetManager.GetStreetNamesFromSpreadsheet(streetNames1Sheet);
            Console.WriteLine("Loaded street names without space");
            parser.StreetNamesWithSpace = spreadsheetManager.GetStreetNamesFromSpreadsheet(streetNames2Sheet);
            Console.WriteLine("Loaded street names with space");
            parser.StreetDirections = new List<string> {"North", "N", "South", "S", "East", "E", "West", "W"};
            parser.StreetTypes = new List<string>{"Street", "St", "Road", "Rd", "Lane", "Ln","Circle", "Avenue", "Ave","Place","Drive","Place","Terrace","Court", "Ct"};

            var addressesSheet = spreadsheetManager.GetWorksheet(spreadsheet, "Addresses");
            var noAddressSheet = spreadsheetManager.GetWorksheet(spreadsheet, "NoAddressSMS");

            Console.WriteLine("Parsing messages...");

            foreach (var sms in smsList)
            {
                sms.Address = parser.GetAddress(sms.Body);
                if (sms.Address == null)
                {
                    spreadsheetManager.InsertSMSIntoSpreadsheet(noAddressSheet, sms);
                }
                else
                {
                    spreadsheetManager.InsertSMSIntoSpreadsheet(addressesSheet, sms);
                }
                
            }

            Console.WriteLine("Messages successfully parsed");
            Console.WriteLine("Press any key to exit");


            

            Console.ReadKey(true);
            }

        
        }


    
    }