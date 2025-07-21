using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Course
{
    public string CourseId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? Fee { get; set; }

    public int? Duration { get; set; }

    public string? ImageUrl { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual ICollection<CourseEnrollment> CourseEnrollments { get; set; } = new List<CourseEnrollment>();

    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();

    public virtual ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
}
