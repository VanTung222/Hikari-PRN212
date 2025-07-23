using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public string StudentId { get; set; } = null!;

    public string EnrollmentId { get; set; } = null!;

    public decimal Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public string PaymentStatus { get; set; } = null!;

    public DateTime? PaymentDate { get; set; }

    public string? TransactionId { get; set; }

    public virtual CourseEnrollment Enrollment { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
