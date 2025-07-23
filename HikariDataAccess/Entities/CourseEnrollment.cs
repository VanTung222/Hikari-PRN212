using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class CourseEnrollment
{
    public string EnrollmentId { get; set; } = null!;

    public string StudentId { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public DateOnly? EnrollmentDate { get; set; }

    public DateOnly? CompletionDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual Student Student { get; set; } = null!;
}
