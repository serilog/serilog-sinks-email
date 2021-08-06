using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serilog.Sinks.Email.Sinks.Email
{
    internal class Email
    {
        public string From { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsBodyHtml { get; set; }
        public IEnumerable<string> Tos { get; set; }
    }
}
