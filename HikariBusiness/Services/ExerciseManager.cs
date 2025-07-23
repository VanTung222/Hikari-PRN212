using HikariDataAccess.DAO;
using HikariDataAccess.Entities;

namespace HikariBusiness.Services
{
    public class ExerciseManager
    {
        private readonly ExerciseDAO _exerciseDAO;

        public ExerciseManager()
        {
            _exerciseDAO = new ExerciseDAO();
        }

        public Exercise? GetExerciseForLesson(int lessonId)
        {
            return _exerciseDAO.GetExerciseByLessonId(lessonId);
        }
    }
}
