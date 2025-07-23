using System;
using System.Collections.Generic;

namespace HikariDataAccess.Entities;

public partial class UserAccount
{
    public string UserId { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Role { get; set; } = null!;

    public DateOnly? RegistrationDate { get; set; }

    public string? ProfilePicture { get; set; }

    public string? Phone { get; set; }

    public DateOnly? BirthDate { get; set; }
    
    public Boolean IsActive { get; set; } = true;
    public virtual ICollection<CourseReview> CourseReviews { get; set; } = new List<CourseReview>();

    public virtual Student? Student { get; set; }

    public virtual Teacher? Teacher { get; set; }
}
