using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Progress
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public string EnrollmentId { get; set; } = null!;

    public int LessonId { get; set; }

    public string CompletionStatus { get; set; } = null!;

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public decimal? Score { get; set; }

    public string? Feedback { get; set; }

    public virtual CourseEnrollment Enrollment { get; set; } = null!;

    public virtual Lesson Lesson { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
