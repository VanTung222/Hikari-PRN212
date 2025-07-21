using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Discount
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string CourseId { get; set; } = null!;

    public int? DiscountPercent { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public bool? IsActive { get; set; }

    public virtual Course Course { get; set; } = null!;
}
