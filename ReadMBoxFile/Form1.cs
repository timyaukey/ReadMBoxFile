using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ReadMBoxFile
{
    public partial class Form1 : Form
    {
        private TextReader inputReader;
        private string currentLine;
        private string previousLine;
        private int lineNumber;

        public Form1()
        {
            InitializeComponent();
        }

        private void ReadFile(string nameRoot)
        {
            using (inputReader = new StreamReader(@"C:\Dropbox\Tim\StoreEmails\Takeout\Mail\" + nameRoot + ".mbox"))
            {
                currentLine = "";
                lineNumber = 0;
                List<MailMessage> messages = new List<MailMessage>();
                ReadLine();
                if (!IsFromLine())
                    throw new Exception("Invalid first line");
                // Iterate once for each message in the MBOX file.
                for (; ; )
                {
                    MailMessage msg = ReadMessage(new MessageHeaders(), IsFromLine);
                    messages.Add(msg);
                    if (msg.ReachedEOF)
                        break;
                }
                messages.Sort((m1, m2) => m1.Headers.Date.CompareTo(m2.Headers.Date));
                MessageBox.Show("Processed " + messages.Count + " messages.");
            }
        }

        private delegate bool IsMessageBreak();

        private MailMessage ReadMessage(MessageHeaders parentHeaders, IsMessageBreak isMessageBreak)
        {
            MailMessage result = new MailMessage();
            result.Headers = ReadHeaders(parentHeaders);
            if (result.Headers.IsMultiPart)
                ReadMultiPart(result);
            else
                ReadSinglePart(result, isMessageBreak);
            return result;
        }

        private bool IsFromLine()
        {
            if (currentLine != null && currentLine.StartsWith("From "))
            {
                if (previousLine == "")
                    return true;
            }
            return false;
        }

        private MessageHeaders ReadHeaders(MessageHeaders parentHeaders)
        {
            MessageHeaders result = new MessageHeaders(parentHeaders);
            ReadLine();
            for (; ; )
            {
                // At this point currentLine is either blank or the first line of a header
                if (string.IsNullOrEmpty(currentLine))
                    return result;
                int colonIndex = currentLine.IndexOf(':');
                if (colonIndex <= 0)
                {
                    ReadLine();
                    continue;
                }
                string headerName = currentLine.Substring(0, colonIndex);
                if (colonIndex + 1 < currentLine.Length)
                {
                    if (currentLine[colonIndex + 1] == ' ')
                        colonIndex++;
                }
                string value = currentLine.Substring(colonIndex + 1);
                for(; ;)
                {
                    ReadLine();
                    if (currentLine == null)
                        break;
                    if (!currentLine.StartsWith(" ") && !currentLine.StartsWith("\t"))
                        break;
                    value = value + " " + currentLine.TrimStart(' ', '\t');
                }
                if (headerName == "Content-Type")
                {
                    result.ContentType = value;
                    if (value.StartsWith("multipart/"))
                    {
                        int boundaryIndex = value.IndexOf("boundary=");
                        if (boundaryIndex > 0)
                        {
                            result.IsMultiPart = true;
                            result.Boundary = value.Substring(boundaryIndex + 9).Trim();
                            if (result.Boundary[0]=='"')
                            {
                                if (result.Boundary.Length > 2)
                                {
                                    if (result.Boundary[result.Boundary.Length - 1] == '"')
                                    {
                                        result.Boundary = result.Boundary.Substring(1, result.Boundary.Length - 2);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (headerName == "Subject")
                {
                    result.Subject = value;
                }
                else if (headerName == "To")
                {
                    result.SendTo = value;
                }
                else if (headerName == "Reply-To")
                {
                    result.ReplyTo = value;
                }
                else if (headerName == "Message-ID")
                {
                    result.MessageID = value;
                }
                else if (headerName == "Date")
                {
                    DateTime parsedDate;
                    if (DateTime.TryParse(value, out parsedDate))
                    {
                        result.Date = parsedDate;
                    }
                }
                else if (headerName == "Content-Transfer-Encoding")
                {
                    switch (value.ToLower())
                    {
                        case "quoted-printable":
                            result.ContentEncoding = ContentEncodingType.QuotedPrinted;
                            break;
                        case "7bit":
                            result.ContentEncoding = ContentEncodingType.SevenBit;
                            break;
                        case "8bit":
                            result.ContentEncoding = ContentEncodingType.ASCII;
                            break;
                        default:
                            throw new InvalidDataException("Unrecognized Content-Transfer-Encoding: " + value);
                    }
                }
            }
        }

        private void ReadMultiPart(MailMessage msg)
        {
            msg.ReachedEOF = true;
            for (; ; )
            {
                string bodyLine = ReadBodyLine(msg.Headers.ContentEncoding);
                if (bodyLine == null)
                    return;
                if (IsFromLine())
                {
                    msg.ReachedEOF = false;
                    break;
                }
                if (bodyLine.StartsWith("--" + msg.Headers.Boundary))
                {
                    for (; ; )
                    {
                        MailMessage messagePart = ReadMessage(msg.Headers, () =>
                            currentLine == null ||
                            currentLine.StartsWith("--" + msg.Headers.Boundary));
                        msg.Children.Add(messagePart);
                        if (currentLine == null || currentLine.EndsWith("--"))
                            break;
                    }
                }
                else
                    msg.Lines.Add(TranslateLine(bodyLine));
                if (msg.TooLong)
                    MessageBox.Show("Message too long: " + msg.ToString());
            }
        }

        private void ReadSinglePart(MailMessage msg, IsMessageBreak isMessageEnd)
        {
            msg.ReachedEOF = true;
            bool tooLongShown = false;
            for(; ; )
            {
                string bodyLine = ReadBodyLine(msg.Headers.ContentEncoding);
                if (bodyLine == null)
                    break;
                if (isMessageEnd())
                {
                    msg.ReachedEOF = false;
                    break;
                }
                msg.Lines.Add(TranslateLine(bodyLine));
                if (msg.TooLong && !tooLongShown)
                {
                    tooLongShown = true;
                    MessageBox.Show("Message too long: " + msg.ToString());
                }
            }
            if (msg.Headers.ContentType.StartsWith("text/html"))
            {
                if (msg.Headers.ReplyTo == "susan@schmidtsgardencenter.com" ||
                    msg.Headers.ReplyTo == "susanandtim@schmidtsgardencenter.com")
                {
                }
            }
        }

        private string ReadBodyLine(ContentEncodingType encodingType)
        {
            switch(encodingType)
            {
                case ContentEncodingType.ASCII:
                    ReadLine();
                    return currentLine;
                case ContentEncodingType.SevenBit:
                    ReadLine();
                    return currentLine;
                case ContentEncodingType.QuotedPrinted:
                    StringBuilder bodyLine = new StringBuilder();
                    bool anyContent = false;
                    for (; ; )
                    {
                        ReadLine();
                        if (currentLine == null)
                        {
                            if (anyContent)
                                return bodyLine.ToString();
                            else
                                return null;
                        }
                        anyContent = true;
                        if (!currentLine.EndsWith("="))
                        {
                            bodyLine.Append(currentLine);
                            return bodyLine.ToString();
                        }
                        bodyLine.Append(currentLine.Substring(0, currentLine.Length - 1));
                    }
                default:
                    throw new InvalidEnumArgumentException("Unrecognized encoding type in ReadBodyLine");
            }
        }

        private void ReadLine()
        {
            previousLine = currentLine;
            currentLine = inputReader.ReadLine();
            lineNumber++;
        }

        private string TranslateLine(string line)
        {
            return line.Replace("=0D", "\r").Replace("=0A", "\n").Replace("=3D", "=");
        }

        private void btnRunBasic_Click(object sender, EventArgs e)
        {
            ReadFile("Basic");
        }

        private void btnRunMultipart_Click(object sender, EventArgs e)
        {
            ReadFile("Multipart");
        }

        private void btnRunNested_Click(object sender, EventArgs e)
        {
            ReadFile("Nested");
        }

        private void btnRunAll_Click(object sender, EventArgs e)
        {
            ReadFile("All");
        }
    }

    class MailMessage
    {
        public MessageHeaders Headers;
        public List<string> Lines = new List<string>();
        public bool ReachedEOF = false;
        public List<MailMessage> Children = new List<MailMessage>();

        public bool TooLong
        {
            get { return Lines.Count > 50000; }
        }

        public override string ToString()
        {
            return Headers.Subject + " " + Headers.ReplyTo + " " + Headers.Date;
        }
    }

    class MessageHeaders
    {
        public bool IsMultiPart;
        public string Subject;
        public string Boundary;
        public string ContentType;
        public string SendTo;
        public string ReplyTo;
        public string MessageID;
        public DateTime Date;
        public ContentEncodingType ContentEncoding = ContentEncodingType.ASCII;

        public MessageHeaders()
        {
            IsMultiPart = false;
            Subject = "";
            Boundary = "";
            ContentType = "";
            SendTo = "";
            ReplyTo = "";
            MessageID = "";
            ContentEncoding = ContentEncodingType.ASCII;
        }

        public MessageHeaders(MessageHeaders parentHeaders)
            :this()
        {
            Subject = parentHeaders.Subject;
            SendTo = parentHeaders.SendTo;
            ReplyTo = parentHeaders.ReplyTo;
            MessageID = parentHeaders.MessageID;
            Date = parentHeaders.Date;
        }
    }

    enum ContentEncodingType
    {
        QuotedPrinted = 1,
        ASCII = 2,
        SevenBit = 3
    }
}
