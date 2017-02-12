using System.Collections;
using System.Collections.Generic;

namespace NullDesk.Extensions.Mailer.Tests.Common
{
    public class TemplateMailerTestData : IEnumerable<object[]>
    {

        private readonly List<object[]> _data = new List<object[]>
        {
            new object[] { "template1", new string[] { } },
            new object[] { "template1", new[] { @"..\..\..\..\TestData\attachments\testFile.1.txt" } },
            new object[] { "template1", new[] { @"..\..\..\..\TestData\attachments\testFile.1.txt", @"..\..\..\..\TestData\attachments\testFile.2.txt" } },
            new object[] { "template2", new[] { @"..\..\..\..\TestData\attachments\testFile.1.txt" } },
            new object[] { "template2", null}
        };

        public IEnumerator<object[]> GetEnumerator()
        { return _data.GetEnumerator(); }

        IEnumerator IEnumerable.GetEnumerator()
        { return GetEnumerator(); }
    }
}