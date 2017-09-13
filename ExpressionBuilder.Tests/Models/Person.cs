using System;

namespace ExpressionBuilder.Tests.Models
{
    class Person
    {
        public int id { get; set; }
        public string name { get; set; }
        public int color { get; set; }
        public bool isMarried { get; set; }
        public DateTime? birthdate { get; set; }
        public override string ToString()
        {
            return $"id: {id}; name: {name}; color: {color}; isMarried: {isMarried}; birthdate: {(birthdate.HasValue ? birthdate.Value.ToShortDateString() : "N/A")}";
        }
    }
}
