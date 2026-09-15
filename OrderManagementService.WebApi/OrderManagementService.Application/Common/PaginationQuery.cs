using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderManagementService.Application.Common
{
    public class PaginationQuery
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    }
}
