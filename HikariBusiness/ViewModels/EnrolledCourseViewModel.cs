using System;

namespace HikariBusiness.ViewModels
{
    public class EnrolledCourseViewModel
    {
        public string CourseID { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public string FormattedFee => $"{Fee:N0} VNĐ";
        public DateTime EnrollmentDate { get; set; }
        public string FormattedEnrollmentDate => EnrollmentDate.ToString("dd/MM/yyyy");
        
        // Progress information
        public int TotalLessons { get; set; }
        public int CompletedLessons { get; set; }
        public double ProgressPercentage => TotalLessons > 0 ? (double)CompletedLessons / TotalLessons * 100 : 0;
        public string FormattedProgress => $"{CompletedLessons}/{TotalLessons} bài học";
        public string ProgressText => $"{ProgressPercentage:F0}% hoàn thành";
        
        // Status
        public bool IsCompleted => CompletedLessons >= TotalLessons && TotalLessons > 0;
        public string StatusText => IsCompleted ? "Đã hoàn thành" : "Đang học";
        
        // UI Properties
        public string CourseIcon { get; set; } = "📚";
        public string BackgroundColor { get; set; } = "#6C5CE7";
        
        // Constructor
        public EnrolledCourseViewModel()
        {
            // Set default values for demo
            TotalLessons = 20;
            CompletedLessons = 0;
        }
    }
}
