using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System.Collections.Generic;

namespace HikariBusiness.TeacherReponsitory
{
    public class QuestionReponsitory
    {
        private readonly QuestionDAO _dao = new QuestionDAO();
        public List<Question> GetQuestionsByTestId(int testId)
        {
            return _dao.GetQuestionsByTestId(testId);
        }
    }
} 