using ExpressionBuilder.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ExpressionBuilder.Tests
{
    [TestClass]
    public class TestSet1
    {
        IList<Person> persons;
        readonly static string[] filter = new string[] {
            "id lt 3",
            "color eq 2",
            "name eq Alice",
            "isMarried eq true",
            "birthdate gt 01-01-1999",
            "birthdate eq null"
        };

        //------Create Persons & Print
        [TestInitialize]
        public void Initialize() =>
            persons = new List<Person>(){
                new Person() { id = 1, name = "John", color = 3 },
                new Person() { id = 2, name = "Alice", color = 2, isMarried = true, birthdate = new DateTime(2000, 01, 01, new GregorianCalendar()) },
                new Person() { id = 3, name = "Bob", color = 2, isMarried = true },
            };

        [TestMethod]
        public void SingleResult1()
        {
            var filters = filter.Where(o => o.Contains("lt")).ToArray();
            var statement = ExpressionHelper.CreateNewStatement<Person>(filters);
            IEnumerable<Person> result = persons.Where(statement);
            Assert.IsTrue(result.ToList().Count > 1);
        }

        [TestMethod]
        public void SingleResult2()
        {
            var filters = new string[] { filter[5] };
            var statement = ExpressionHelper.CreateNewStatement<Person>(filters);
            IEnumerable<Person> result = persons.Where(statement);
            Assert.IsTrue(result.ToList().Count == 2);
        }
    }
}
