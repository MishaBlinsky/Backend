using Models.Dto.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Dto.V1.Responses
{
    public class V1CreateOrderResponse
    {
        public OrderUnit[] Orders { get; set; }
    }
}