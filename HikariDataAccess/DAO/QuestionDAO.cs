using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HikariDataAccess;
using HikariDataAccess.Entities;

namespace HikariDataAccess.DAO
{
    public class QuestionDAO
    {
        public List<Question> GetQuestionsByEntityId(int entityId, string entityType)
        {
            using (var context = new HikariContext())
            {
                return context.Questions
                              .Where(q => q.EntityId == entityId && q.EntityType == entityType)
                              .OrderBy(q => q.Id) // Sắp xếp để câu hỏi hiển thị theo thứ tự
                              .ToList();
            }
        }
    }
}
