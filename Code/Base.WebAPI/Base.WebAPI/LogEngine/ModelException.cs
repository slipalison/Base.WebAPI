using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Principal;

namespace Base.WebAPI
{
    public class ModelException
    {
        public ModelException()
        {
            Date = DateTime.Now;
        }
        public IIdentity Identity { get; set; }
        public string request { get; set; }
        public IEnumerable<string> msg { get; set; }
        public string source { get; set; }
        public string stackTrace { get; set; }
        public string targetSite { get; set; }

        public DateTime Date { get; private set; }

        public override string ToString() => JsonConvert.SerializeObject(this, Formatting.Indented);

    }
}
