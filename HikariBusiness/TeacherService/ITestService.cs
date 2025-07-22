using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public interface ITestService
    {
        public void AddTest(Test test);
        public void UpdateTest(Test test);
        public void DeleteTest(int testId);
        public Test GetTestById(int testId);
        public List<Test> GetAllTests();

    }
}
