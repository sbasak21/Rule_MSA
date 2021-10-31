using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RulesMicroservice.Models
{
    public class Account
    {
        public int AccId { get; set; }
        public int Balance { get; set; }

        public string AccountType { get; set; }
    }
}
