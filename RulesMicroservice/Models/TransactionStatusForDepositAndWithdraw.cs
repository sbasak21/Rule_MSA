using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RulesMicroservice.Models
{
    public class TransactionStatusForDepositAndWithDraw
    {
        public string Status { get; set; }

        public double CurrentBalance { get; set; }
    }
}
