using HikariDataAccess.Entities;
using System.Linq;

namespace HikariDataAccess.DAO
{
    public class ExerciseDAO
    {
        private readonly HikariContext _context;

        public ExerciseDAO()
        {
            _context = new HikariContext();
        }

        public Exercise? GetExerciseByLessonId(int lessonId)
        {
            // An exercise is unique to a lesson, so FirstOrDefault is appropriate.
            return _context.Exercises.FirstOrDefault(e => e.LessonId == lessonId);
        }
    }
}
