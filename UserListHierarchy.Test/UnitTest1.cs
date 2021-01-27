using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UserListHierarchy.Test
{
    [TestClass]
    public class UserListHierarchyTest
    {
        static List<User> Users = new List<User>();
        static List<role> Roles = new List<role>();
        static Dictionary<int, List<int>> result = new Dictionary<int, List<int>>();


        [TestInitialize]
        public void InitializeData()
        {          

            User user1 = new User { name = "Adam", id = 1, role = 1 };
            User user2 = new User { name = "Emily", id = 2, role = 4 };
            User user3 = new User { name = "Sam", id = 3, role = 3 };
            User user4 = new User { name = "Mary", id = 4, role = 2 };
            User user5 = new User { name = "Steve", id = 5, role = 5 };

            Role role1 = new Role { id = 1, name = "Admin", pid = 0 };
            Role role2 = new Role { id = 2, name = "LM", pid = 1 };
            Role role3 = new Role { id = 3, name = "Sup", pid = 2 };
            Role role4 = new Role { id = 4, name = "Emp", pid = 3 };
            Role role5 = new Role { id = 5, name = "Trainee", pid = 4 };

          
        }


        [TestMethod]
        public void CheckUserCount()
        {

            var res = Users
                .Where(e => e.EmpId > 5)
                .Where(t => t.Salary >= 5000);

            Assert.AreEqual(2, res.Count());

            res = Employees
                .Where(e => e.EmpId > 5);

            Assert.AreEqual(5, res.Count());

        }

        [TestMethod]
        public void TestGroupBy()
        {
            var res = from e in Employees
                      group e by e.Salary;

            Assert.AreEqual(5, res.Count());

            var res1 = Employees.GroupBy(e => e.Salary);
            Assert.AreEqual(5, res1.Count());
        }

        [TestMethod]
        public void TestJoin()
        {
            var res = from o in Orders
                      join Employee e in Employees
                          on o.EmpId equals e.EmpId
                      where o.EmpId == 11
                      select o;

            Assert.AreEqual(2, res.Count());
        }

        [TestMethod]
        public void TestJoinData()
        {
            var res = from o in Orders
                      join Employee e in Employees
                          on o.EmpId equals e.EmpId
                      join Book b in Books
                          on o.BookId equals b.BookId
                      orderby e.EmpId
                      select new { o.OrderId, e.Name, b.Title, b.Price };

            Assert.AreEqual("Test1", res.First().Name);

        }

    }
}