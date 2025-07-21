using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Document
{
    public int Id { get; set; }

    public int? LessonId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? FileUrl { get; set; }

    public DateTime? UploadDate { get; set; }

    public string? UploadedBy { get; set; }

    public virtual Lesson? Lesson { get; set; }

    public virtual Teacher? UploadedByNavigation { get; set; }
}
