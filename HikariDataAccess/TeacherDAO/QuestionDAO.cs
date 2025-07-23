using DataAccessLayer;
using DataAccessLayer.Entities;
using System.Collections.Generic;
using System.Linq;

namespace HikariDataAccess.TeacherDAO
{
    public class QuestionDAO
    {
        public List<Question> GetQuestionsByTestId(int testId)
        {
            using (var context = new HikariContext())
            {
                return context.Questions.Where(q => q.EntityType == "test" && q.EntityId == testId).ToList();
            }
        }
    }
} 