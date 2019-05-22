using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataProcessor.Models
{
    public class EntityBase
    {
        [ExplicitKey]
        public Guid Id { get; set; }
    }
}
