using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Test
{
    public int Id { get; set; }

    public string JlptLevel { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public decimal? TotalMarks { get; set; }

    public int? TotalQuestions { get; set; }

    public bool? IsActive { get; set; }
}
