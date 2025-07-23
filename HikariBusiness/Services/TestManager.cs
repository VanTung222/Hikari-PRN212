using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess.DAO;
using HikariDataAccess.Entities;
using static System.Net.Mime.MediaTypeNames;

namespace HikariBusiness.Services
{
    public class TestManager
    {
        private TestDAO _testDAO;

        public TestManager()
        {
            _testDAO = new TestDAO();
        }

        public List<Test> GetAvailableTests()
        {
            // Các logic nghiệp vụ khác (ví dụ: kiểm tra quyền, lọc bổ sung) có thể được thêm ở đây
            return _testDAO.GetAllActiveTests();
        }

        // Ví dụ: Lấy chi tiết một bài test
        public Test GetTestDetails(int testId)
        {
            // Các logic nghiệp vụ khác trước khi lấy chi tiết
            return _testDAO.GetTestById(testId);
        }

        // Ví dụ: thêm một bài test mới qua Business Logic
        public void CreateNewTest(Test test)
        {
            // Thêm các validation hoặc business rules ở đây
            if (string.IsNullOrWhiteSpace(test.Title))
            {
                throw new ArgumentException("Tiêu đề bài kiểm tra không được để trống.");
            }
            // ... các quy tắc khác

            _testDAO.AddTest(test);
        }
    }
}
