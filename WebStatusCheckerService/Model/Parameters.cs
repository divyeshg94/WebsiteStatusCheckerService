using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebStatusCheckerService.Model
{
    public class Parameters
    {
        public List<Site> Sites { get; set; }
        public string SenderMailId { get; set; }
        public string SenderMailPassword { get; set; }
    }

    public class Site
    {
        public string Name { get; set; }
        public string ReceiverList { get; set; }
        public string Url { get; set; }
    }
}
