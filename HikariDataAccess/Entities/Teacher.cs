using System;
using System.Collections.Generic;

namespace DataAccessLayer.Entities;

public partial class Teacher
{
    public string TeacherId { get; set; } = null!;

    public string UserId { get; set; } = null!;

    public string? Specialization { get; set; }

    public int? ExperienceYears { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual UserAccount User { get; set; } = null!;
}
