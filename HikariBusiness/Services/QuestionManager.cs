using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess.DAO;
using HikariDataAccess.Entities;

namespace HikariBusiness.Services
{
    public class QuestionManager
    {
        private readonly QuestionDAO _questionDAO;

        public QuestionManager()
        {
            _questionDAO = new QuestionDAO();
        }

        // Lấy danh sách câu hỏi cho một bài test cụ thể
        public List<Question> GetTestQuestions(int testId)
        {
            // entityType là 'test' như chúng ta đã insert
            return _questionDAO.GetQuestionsByEntityId(testId, "test");
        }
    }
}
