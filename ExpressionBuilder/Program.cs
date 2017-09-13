using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ExpressionBuilder;
using System.Threading.Tasks;
using System.Globalization;

namespace ExpressionBuilder
{
    class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("ExpressionBuilder START");
            Console.WriteLine("----------------\n\n");
            startProgram();
            Console.WriteLine("\n\n----------------");
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }

        static void startProgram()
        {
            //------Create Persons & Print

            var persons = new List<Person>(){
                new Person() { id = 3, name = "Bob", color = 1 },
                new Person() { id = 2, name = "Alice", color = 2, isMarried = true, birthdate = new DateTime(2000, 01, 01, new GregorianCalendar()) },
                new Person() { id = 1, name = "John", color = 3 }
            };

            Console.WriteLine("Persons available:");
            foreach (var pers in persons) {
                Console.WriteLine($"- {pers.ToString()}");
            }

            //------Create Filters & Print

            var filter = new string[] { "id lt 3", "color eq 2", "name eq Alice", "isMarried eq true", "birthdate gt 01-01-1999" };

            Console.WriteLine("\nAdding filter(s) of:");
            foreach(var item in filter)
            {
                Console.WriteLine($"- {item}");
            }

            //------Apply Filters & Print

            var statement = ExpressionHelper.CreateNewStatement<Person>(filter);
            IEnumerable<Person> result = persons.Where(statement);

            Console.WriteLine("\nFiltered Persons:");
            foreach (var per in result)
            {
                Console.WriteLine($"- {per.ToString()}");
            }
        }
    }

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
