using Dapper.Contrib.Extensions;
using System;

namespace DataProcessor.Models
{
    public class EntityBase
    {
        [ExplicitKey]
        public Guid Id { get; set; }
    }
}
