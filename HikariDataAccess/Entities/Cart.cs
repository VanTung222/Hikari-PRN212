using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Cart
{
    public int Id { get; set; }

    public string StudentID { get; set; } = null!;

    public string CourseID { get; set; } = null!;

    public DateTime AddedDate { get; set; }

    // Navigation properties
    public virtual Course Course { get; set; } = null!;
    public virtual Student Student { get; set; } = null!;
}
