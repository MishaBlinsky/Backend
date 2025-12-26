using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.V1.Requests
{
    public class V1QueryOrdersRequest
    {
        public long[] Ids { get; set; }
        public long[] CustomerIds { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
        public bool IncludeOrderItems { get; set; }
    }
}