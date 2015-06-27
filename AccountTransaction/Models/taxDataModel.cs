using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AccountTransaction.Models
{
    public class taxDataModel
    {
        public class TransactionDataModel
        {
            [Required]
            public string Account { get; set; }
            [Required]
            public string Description { get; set; }
            [Required]
            public string CurrencyCode { get; set; }

            [Required]
            [Range(0.01, 100000.00)]
            public decimal Amount { get; set; }

        }
    }
}