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
    public class Receiver
    {
        private readonly string _userName;
        private readonly string _password;

        public Receiver(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public IEnumerable<Sms> GetSmsList(DateTime startingDate)
        {
            List<Sms> smsList = new List<Sms>();
            using (var client = new ImapClient())
            {
                var credentials = new NetworkCredential(_userName, _password);
                var uri = new Uri("imaps://imap.gmail.com");

                using (var cancel = new CancellationTokenSource())
                {
                    client.Connect(uri, cancel.Token);
                    client.Authenticate(credentials, cancel.Token);

                    // The Inbox folder is always available on all IMAP servers...
                    var inbox = client.Inbox;
                    inbox.Open(FolderAccess.ReadOnly, cancel.Token);


                    for (int i = 0; i < inbox.Count; i++)
                    {
                        var message = inbox.GetMessage(inbox.Count - 1 - i, cancel.Token);
                        if (message.Subject.Contains("SMS") && message.Date.DateTime >= startingDate)
                        {
                            try
                            {
                                using (MemoryStream stream = new MemoryStream())
                                {
                                    message.Body.WriteTo(stream);
                                    var buffer = stream.GetBuffer();
                                    string originalSms = Encoding.UTF8.GetString(buffer);
                                    smsList.Add(
                                        new Sms
                                            {
                                                PhoneNumber = GetPhoneNumber(message.Subject),
                                                Body = GetTrimmedSmsBody(originalSms),
                                                Date = message.Date.Date
                                            }
                                        );
                                }
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Failed to write sms body to stream");
                            }
                        }

                        if (message.Date.DateTime < startingDate)
                        {
                            break;
                        }
                    }

                    client.Disconnect(true, cancel.Token);
                }
            }
            return smsList;
        }

        private string GetTrimmedSmsBody(string body)
        {
            int startPoint = body.IndexOf("delsp=yes") + 9;
            int endPoint = body.IndexOf("--");
            int altenativeEndPoint = body.IndexOf("\0");
            if (endPoint > 1)
            {
                body = body.Remove(endPoint);
            }
            else if (altenativeEndPoint > 1)
            {
                body = body.Remove(altenativeEndPoint);
            }

            string trimmedMessage = body.Substring(startPoint).Trim();
            trimmedMessage = trimmedMessage.Replace("\r", string.Empty);
            trimmedMessage = trimmedMessage.Replace("\n", string.Empty);

            return trimmedMessage;
        }

        private string GetPhoneNumber(string smsSubject)
        {
            return smsSubject.Substring(smsSubject.IndexOf('('));
        }
    }
}