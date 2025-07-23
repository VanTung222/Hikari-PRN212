using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;

namespace HikariDataAccess.DAO
{
    public class TestDAO
    {
        public List<Test> GetAllActiveTests()
        {
            using (var context = new HikariContext()) // Sử dụng HikariContext
            {
                // Truy vấn EF Core để lấy danh sách các bài Test đang hoạt động
                return context.Tests.Where(t => (bool)t.IsActive).ToList();
            }
        }

        public Test GetTestById(int id)
        {
            using (var context = new HikariContext())
            {
                return context.Tests.FirstOrDefault(t => t.Id == id);
            }
        }

        public void AddTest(Test newTest)
        {
            using (var context = new HikariContext())
            {
                context.Tests.Add(newTest);
                context.SaveChanges();
            }
        }
    }
}
