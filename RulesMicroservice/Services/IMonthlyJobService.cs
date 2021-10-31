using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RulesMicroservice.Services
{
    public interface IMonthlyJobService
    {
        public Task toRunMonthlyjob();
    }
}
