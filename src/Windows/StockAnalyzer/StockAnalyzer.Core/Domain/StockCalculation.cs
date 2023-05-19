using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StockAnalyzer.Core.Domain
{
    public class StockCalculation
    {
        public string Identifier { get; set; }
        public decimal? Result { get; set; }
        public int TotalSeconds { get; set; }
    }
}
