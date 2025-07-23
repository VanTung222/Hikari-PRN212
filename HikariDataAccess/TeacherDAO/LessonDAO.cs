using DataAccessLayer;
using DataAccessLayer.Entities;
using HikariDataAccess;

public class LessonDAO
{
    private readonly HikariContext _context;

    public LessonDAO()
    {
        _context = new HikariContext();
    }

    public List<Lesson> GetAllLessons()
    {
        return _context.Lessons.ToList();
    }

    public void AddLesson(Lesson lesson)
    {
        _context.Lessons.Add(lesson);
        _context.SaveChanges();
    }

    public void UpdateLesson(Lesson lesson)
    {
        var existingLesson = _context.Lessons.FirstOrDefault(l => l.Id == lesson.Id);
        if (existingLesson != null)
        {
            existingLesson.Title = lesson.Title;
            existingLesson.Description = lesson.Description;
            existingLesson.Duration = lesson.Duration;
            existingLesson.CourseId = lesson.CourseId;
            _context.SaveChanges();
        }
    }

    public void DeleteLesson(int lessonId)
    {
        var lesson = _context.Lessons.FirstOrDefault(l => l.Id == lessonId);
        if (lesson != null)
        {
            _context.Lessons.Remove(lesson);
            _context.SaveChanges();
        }
    }

    public Lesson GetLessonById(int lessonId)
    {
        return _context.Lessons.FirstOrDefault(l => l.Id == lessonId);
    }

    public List<Lesson> GetLessonsByCourseId(string courseId)
    {
        return _context.Lessons.Where(l => l.CourseId == courseId).ToList();
    }
}