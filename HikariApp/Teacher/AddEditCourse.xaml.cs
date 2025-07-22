using DataAccessLayer.Entities;
using System;
using System.Windows;
using System.Windows.Controls;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class AddEditCourse : Window
    {
        public Course Course { get; private set; }
        public bool IsSuccess { get; private set; }

        private readonly CourseService _courseService = new CourseService();

        public AddEditCourse(Course course = null)
        {
            InitializeComponent();
            Course = course ?? new Course();
            IsSuccess = false;
            LoadCourseData();
            // Đặt tiêu đề phù hợp
            if (course != null && !string.IsNullOrEmpty(course.CourseId))
            {
                this.Title = "Sửa Khóa Học";
                WindowTitle.Text = "SỬA KHÓA HỌC";
            }
            else
            {
                this.Title = "Thêm Khóa Học";
                WindowTitle.Text = "THÊM KHÓA HỌC MỚI";
            }
        }

        private void LoadCourseData()
        {
            if (Course != null)
            {
                txtCourseID.Text = Course.CourseId ?? string.Empty; // Updated to CourseId
                txtTitle.Text = Course.Title ?? string.Empty;
                txtDescription.Text = Course.Description ?? string.Empty;
                txtFee.Text = Course.Fee?.ToString() ?? string.Empty;
                txtDuration.Text = Course.Duration?.ToString() ?? string.Empty;
                dpStartDate.SelectedDate = Course.StartDate.HasValue ? Course.StartDate.Value.ToDateTime(new TimeOnly(0, 0)) : null; // Convert DateOnly to DateTime
                dpEndDate.SelectedDate = Course.EndDate.HasValue ? Course.EndDate.Value.ToDateTime(new TimeOnly(0, 0)) : null; // Convert DateOnly to DateTime
                chkIsActive.IsChecked = Course.IsActive ?? false;

                Title = string.IsNullOrEmpty(Course.CourseId) ? "Thêm Khóa Học" : "Sửa Khóa Học"; // Updated to CourseId
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtTitle.Text))
                {
                    MessageBox.Show("Tên khóa học không được để trống!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!decimal.TryParse(txtFee.Text, out decimal fee) || fee < 0)
                {
                    MessageBox.Show("Học phí không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(txtDuration.Text, out int duration) || duration <= 0)
                {
                    MessageBox.Show("Thời lượng không hợp lệ!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dpStartDate.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dpEndDate.SelectedDate == null)
                {
                    MessageBox.Show("Vui lòng chọn ngày kết thúc!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (dpEndDate.SelectedDate < dpStartDate.SelectedDate)
                {
                    MessageBox.Show("Ngày kết thúc phải sau ngày bắt đầu!", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update Course object
                Course.CourseId = txtCourseID.Text; // Updated to CourseId
                Course.Title = txtTitle.Text;
                Course.Description = txtDescription.Text;
                Course.Fee = fee;
                Course.Duration = duration;
                Course.StartDate = DateOnly.FromDateTime(dpStartDate.SelectedDate.Value); // Convert DateTime to DateOnly
                Course.EndDate = DateOnly.FromDateTime(dpEndDate.SelectedDate.Value); // Convert DateTime to DateOnly
                Course.IsActive = chkIsActive.IsChecked ?? false;

                // Lưu vào database
                if (string.IsNullOrEmpty(Course.CourseId))
                {
                    Course.CourseId = GenerateNextCourseId();
                    _courseService.AddCourse(Course);
                }
                else
                {
                    _courseService.UpdateCourse(Course);
                }

                IsSuccess = true;
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu khóa học: {ex.Message}\n{ex.InnerException?.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private string GenerateNextCourseId()
        {
            var courses = _courseService.GetAllCourses();
            int maxNum = 0;
            foreach (var c in courses)
            {
                if (c.CourseId != null && c.CourseId.StartsWith("CO") && c.CourseId.Length == 5)
                {
                    if (int.TryParse(c.CourseId.Substring(2), out int num))
                    {
                        if (num > maxNum) maxNum = num;
                    }
                }
            }
            return $"CO{(maxNum + 1).ToString("D3")}";
        }
    }
}