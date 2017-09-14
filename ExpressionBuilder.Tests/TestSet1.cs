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
        public void SingleFilterMultipleResults1()
        {
            IList<int> list = new List<int>() { 5 };
            var filters = getFilters(list);
            var statement = ExpressionHelper.CreateNewStatementFunc<Person>(filters);
            IEnumerable<Person> result = persons.Where(statement);
            Assert.IsTrue(result.ToList().Count == 2);
        }

        [TestMethod]
        public void MultipleFiltersSingleResult1()
        {
            IList<int> list = new List<int>() { 0, 3 };
            var filters = getFilters(list);
            var statement = ExpressionHelper.CreateNewStatementFunc<Person>(filters);
            IEnumerable<Person> result = persons.Where(statement);
            Assert.IsTrue(result.ToList().Count == 1);
        }

        [TestMethod]
        public void EmptyStringNullException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => ExpressionHelper.CreateNewStatement<Person>(String.Empty));
        }

        public string getFilters(IEnumerable<int> values)
        {
            IList<string> list = new List<string>();
            foreach(var value in values)
            {
                list.Add(filter[value]);
            }
           
            return list.Aggregate((f, s) => $"{f} AND {s}"); ;
        }
    }
}
