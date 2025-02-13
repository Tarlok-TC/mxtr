using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Helpers
{
    public class SharpspringResponse
    {
        public Result result { get; set; }
        public List<Error> error { get; set; }
        public string id { get; set; }
    }

    public class Create
    {
        public bool success { get; set; }
        public long id { get; set; }
        public Error error { get; set; }
    }

    public class Result
    {
        public List<Create> creates { get; set; }
        public List<Delete> deletes { get; set; }
    }

    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public List<object> data { get; set; }
    }

    public class Delete
    {
        public bool success { get; set; }
        public Error error { get; set; }
    }
}