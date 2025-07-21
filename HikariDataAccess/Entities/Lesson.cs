using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Lesson
{
    public int Id { get; set; }

    public string CourseId { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? MediaUrl { get; set; }

    public int? Duration { get; set; }

    public bool? IsCompleted { get; set; }

    public bool? IsActive { get; set; }

    public virtual Course Course { get; set; } = null!;

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Progress> Progresses { get; set; } = new List<Progress>();
}
