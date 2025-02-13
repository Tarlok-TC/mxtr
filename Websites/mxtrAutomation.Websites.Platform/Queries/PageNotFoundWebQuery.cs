using mxtrAutomation.Web.Common.Attributes;
using mxtrAutomation.Web.Common.Queries;
using mxtrAutomation.Websites.Platform.Models.ManageMenu.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace mxtrAutomation.Websites.Platform.Queries
{
    public class PageNotFoundWebQuery : WebQueryBase
    {
        public static readonly string Route = "/page-not-found";

        public PageNotFoundWebQuery()
        {
        }
    }
}