using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Student
{
    public string StudentId { get; set; } = null!;

    public string? UserId { get; set; }

    public DateOnly? EnrollmentDate { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();

    public virtual UserAccount? User { get; set; }
}
