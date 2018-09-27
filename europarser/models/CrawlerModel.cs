using DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace europarser.models
{
    public class CrawlerModel
    {
        public List<VerkkItem> items { get; set; }
        public CrawlerFilters filters { get; set; }
    }
}
