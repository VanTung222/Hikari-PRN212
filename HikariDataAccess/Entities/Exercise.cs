using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Exercise
{
    public int Id { get; set; }

    public int LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int? PassMark { get; set; }

    public virtual Lesson Lesson { get; set; } = null!;
}
