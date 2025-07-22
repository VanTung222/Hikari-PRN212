using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public class TestReponsitory : ITestReponsitory
    {
        private TestDAO dao = new TestDAO();
        public void AddTest(Test test) => dao.AddTest(test);

        public void DeleteTest(int testId) => dao.DeleteTest(testId);

        public List<Test> GetAllTests() => dao.GetAllTests();

        public Test GetTestById(int testId) => dao.GetTestById(testId);

        public void UpdateTest(Test test) => dao.UpdateTest(test);
    }
}
