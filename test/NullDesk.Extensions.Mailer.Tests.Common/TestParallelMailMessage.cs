﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NullDesk.Extensions.Mailer.Tests.Common
{
    public class TestParallelMailMessage
    {
        public string To { get; set; }
        public string ToDisplay { get; set; }
        public string Subject { get; set; }
        public string Html { get; set; }
        public string Text { get; set; }
    }
}