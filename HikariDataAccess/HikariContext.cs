using System;
using System.Collections.Generic;
using HikariDataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HikariDataAccess;

public partial class HikariContext : DbContext
{
    public HikariContext()
    {
    }

    public HikariContext(DbContextOptions<HikariContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<CourseEnrollment> CourseEnrollments { get; set; }

    public virtual DbSet<CourseReview> CourseReviews { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<Lesson> Lessons { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Progress> Progresses { get; set; }

    public virtual DbSet<Question> Questions { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    public virtual DbSet<Teacher> Teachers { get; set; }

    public virtual DbSet<Test> Tests { get; set; }

    public virtual DbSet<UserAccount> UserAccounts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer(GetConnectionString());

    private string GetConnectionString()
    {
        IConfiguration config = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
        var strConn = config["ConnectionStrings:DefaultConnectionStringDB"];
        return strConn;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Course>(entity =>
        {
            entity.HasKey(e => e.CourseId).HasName("PK__Courses__2AA84FF10D85E1CF");

            entity.Property(e => e.CourseId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("courseID");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.Fee)
                .HasDefaultValue(0.00m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("fee");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("imageUrl");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<CourseEnrollment>(entity =>
        {
            entity.HasKey(e => e.EnrollmentId).HasName("PK__Course_E__ACFF2C66DBFA55B2");

            entity.ToTable("Course_Enrollments");

            entity.Property(e => e.EnrollmentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("enrollmentID");
            entity.Property(e => e.CompletionDate).HasColumnName("completionDate");
            entity.Property(e => e.CourseId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("courseID");
            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("enrollmentDate");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("studentID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Course_En__cours__5070F446");

            entity.HasOne(d => d.Student).WithMany(p => p.CourseEnrollments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Course_En__stude__4F7CD00D");
        });

        modelBuilder.Entity<CourseReview>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Course_R__3213E83F3F2020D8");

            entity.ToTable("Course_Reviews");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("courseID");
            entity.Property(e => e.Rating).HasColumnName("rating");
            entity.Property(e => e.ReviewDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("reviewDate");
            entity.Property(e => e.ReviewText)
                .HasColumnType("text")
                .HasColumnName("reviewText");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("userID");

            entity.HasOne(d => d.Course).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Course_Re__cours__5535A963");

            entity.HasOne(d => d.User).WithMany(p => p.CourseReviews)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Course_Re__userI__5629CD9C");
        });

        modelBuilder.Entity<Discount>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Discount__3213E83F762DADFC");

            entity.ToTable("Discount");

            entity.HasIndex(e => e.Code, "UQ__Discount__357D4CF93100E5B1").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Code)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("code");
            entity.Property(e => e.CourseId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("courseID");
            entity.Property(e => e.DiscountPercent).HasColumnName("discountPercent");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.StartDate).HasColumnName("startDate");

            entity.HasOne(d => d.Course).WithMany(p => p.Discounts)
                .HasForeignKey(d => d.CourseId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Discount__course__7D439ABD");
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Document__3213E83FD6947C49");

            entity.ToTable("Document");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.FileUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("fileUrl");
            entity.Property(e => e.LessonId).HasColumnName("lessonID");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.UploadDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("uploadDate");
            entity.Property(e => e.UploadedBy)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("uploadedBy");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Documents)
                .HasForeignKey(d => d.LessonId)
                .HasConstraintName("FK__Document__lesson__656C112C");

            entity.HasOne(d => d.UploadedByNavigation).WithMany(p => p.Documents)
                .HasForeignKey(d => d.UploadedBy)
                .HasConstraintName("FK__Document__upload__66603565");
        });

        modelBuilder.Entity<Lesson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Lesson__3213E83FD2CF1037");

            entity.ToTable("Lesson");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CourseId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("courseID");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Duration).HasColumnName("duration");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.IsCompleted)
                .HasDefaultValue(false)
                .HasColumnName("isCompleted");
            entity.Property(e => e.MediaUrl)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("mediaUrl");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");

            entity.HasOne(d => d.Course).WithMany(p => p.Lessons)
                .HasForeignKey(d => d.CourseId)
                .HasConstraintName("FK__Lesson__courseID__5AEE82B9");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Payment__3213E83F4AF25974");

            entity.ToTable("Payment");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.EnrollmentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("enrollmentID");
            entity.Property(e => e.PaymentDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("paymentDate");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("paymentMethod");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("paymentStatus");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("studentID");
            entity.Property(e => e.TransactionId)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("transactionID");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Payments)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__enrollm__778AC167");

            entity.HasOne(d => d.Student).WithMany(p => p.Payments)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payment__student__76969D2E");
        });

        modelBuilder.Entity<Progress>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Progress__3213E83F2D14EF0C");

            entity.ToTable("Progress");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CompletionStatus)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("in progress")
                .HasColumnName("completionStatus");
            entity.Property(e => e.EndDate).HasColumnName("endDate");
            entity.Property(e => e.EnrollmentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("enrollmentID");
            entity.Property(e => e.Feedback)
                .HasMaxLength(500)
                .IsUnicode(false)
                .HasColumnName("feedback");
            entity.Property(e => e.LessonId).HasColumnName("lessonID");
            entity.Property(e => e.Score)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("score");
            entity.Property(e => e.StartDate).HasColumnName("startDate");
            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("studentID");

            entity.HasOne(d => d.Enrollment).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.EnrollmentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__enroll__60A75C0F");

            entity.HasOne(d => d.Lesson).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.LessonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__lesson__619B8048");

            entity.HasOne(d => d.Student).WithMany(p => p.Progresses)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Progress__studen__5FB337D6");
        });

        modelBuilder.Entity<Question>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Question__3213E83F089EA449");

            entity.ToTable("Question");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CorrectOption)
                .HasMaxLength(1)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("correctOption");
            entity.Property(e => e.EntityId).HasColumnName("entityID");
            entity.Property(e => e.EntityType)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("entityType");
            entity.Property(e => e.Mark)
                .HasDefaultValue(1m)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("mark");
            entity.Property(e => e.OptionA)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("optionA");
            entity.Property(e => e.OptionB)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("optionB");
            entity.Property(e => e.OptionC)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("optionC");
            entity.Property(e => e.OptionD)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("optionD");
            entity.Property(e => e.QuestionText)
                .HasColumnType("text")
                .HasColumnName("questionText");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Student__4D11D65C7A7F2681");

            entity.ToTable("Student");

            entity.HasIndex(e => e.UserId, "UQ__Student__CB9A1CDEC02A8EC4").IsUnique();

            entity.Property(e => e.StudentId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("studentID");
            entity.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("enrollmentDate");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("userID");

            entity.HasOne(d => d.User).WithOne(p => p.Student)
                .HasForeignKey<Student>(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__Student__userID__3F466844");
        });

        modelBuilder.Entity<Teacher>(entity =>
        {
            entity.HasKey(e => e.TeacherId).HasName("PK__Teacher__98E938752D0BF9F4");

            entity.ToTable("Teacher");

            entity.HasIndex(e => e.UserId, "UQ__Teacher__CB9A1CDE216643A1").IsUnique();

            entity.Property(e => e.TeacherId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("teacherID");
            entity.Property(e => e.ExperienceYears).HasColumnName("experienceYears");
            entity.Property(e => e.Specialization)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("specialization");
            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("userID");

            entity.HasOne(d => d.User).WithOne(p => p.Teacher)
                .HasForeignKey<Teacher>(d => d.UserId)
                .HasConstraintName("FK__Teacher__userID__44FF419A");
        });

        modelBuilder.Entity<Test>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Test__3213E83F9406D306");

            entity.ToTable("Test");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("isActive");
            entity.Property(e => e.JlptLevel)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("jlptLevel");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.TotalMarks)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalMarks");
            entity.Property(e => e.TotalQuestions).HasColumnName("totalQuestions");
        });

        modelBuilder.Entity<UserAccount>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__UserAcco__CB9A1CDF9F01297F");

            entity.ToTable("UserAccount");

            entity.HasIndex(e => e.Email, "UQ__UserAcco__AB6E616418801967").IsUnique();

            entity.Property(e => e.UserId)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("userID");
            entity.Property(e => e.BirthDate).HasColumnName("birthDate");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullName");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("profilePicture");
            entity.Property(e => e.RegistrationDate)
                .HasDefaultValueSql("(CONVERT([date],getdate()))")
                .HasColumnName("registrationDate");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
