using System.Collections;
using System.Collections.Generic;

namespace NullDesk.Extensions.Mailer.Tests.Common
{
    public class StandardMailerTestData : IEnumerable<object[]>
    {
        private const string HtmlBody = "<!doctypehtml><htmllang=\"en\"><head><metacharset=\"utf-8\"><title>TestMessage</title></head><body><p>Hello,</p><p>This is a test html message from xUnit.</p><p>Thanks,</p><p>Bot</p></body></html>";

        private const string TextBody = "Hello,\n\nThis is a test plain text message from xUnit.\n\nThanks,\n\nBot\n\n";

        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] { HtmlBody, TextBody, new string[] { } },
            new object[] { HtmlBody, TextBody, new[] { @"..\..\..\..\TestData\attachments\testFile.1.txt" } },
            new object[] { HtmlBody, TextBody, new[] { @"..\..\..\..\TestData\attachments\testFile.1.txt", @"..\..\..\..\TestData\attachments\testFile.2.txt" } },
            new object[] { null, TextBody, new[] { @"..\..\..\..\TestData\attachments\testFile.1.txt" } },
            new object[] { HtmlBody, null, null }
        };

        public IEnumerator<object[]> GetEnumerator()
        { return _data.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}