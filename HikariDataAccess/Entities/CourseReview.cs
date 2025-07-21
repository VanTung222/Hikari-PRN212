using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class CourseReview
{
    public int Id { get; set; }

    public string CourseId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public int? Rating { get; set; }

    public string? ReviewText { get; set; }

    public DateOnly? ReviewDate { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual UserAccount User { get; set; } = null!;
}
