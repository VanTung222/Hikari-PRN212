using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public class TestService : ITestService
    {
        private readonly ITestReponsitory _testRepository;

        public TestService()
        {
            _testRepository = new TestReponsitory();
        }
        public void AddTest(Test test) => _testRepository.AddTest(test);
        public void DeleteTest(int testId) => _testRepository.DeleteTest(testId);

        public List<Test> GetAllTests() => _testRepository.GetAllTests();

        public Test GetTestById(int testId) => _testRepository.GetTestById(testId);

        public void UpdateTest(Test test) => _testRepository.UpdateTest(test);
    }
}
