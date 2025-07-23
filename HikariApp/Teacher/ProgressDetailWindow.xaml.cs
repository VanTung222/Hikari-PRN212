using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class ProgressDetailWindow : Window
    {
        private readonly Student _student;
        private readonly List<CourseEnrollment> _enrollments;
        private readonly ProgressService _progressService = new ProgressService();

        public ProgressDetailWindow(Student student, List<Progress> progresses = null)
        {
            InitializeComponent();
            _student = student;
            var user = student.User;
            txtFullName.Text = user?.FullName ?? "";
            txtEmail.Text = user?.Email ?? "";
            txtBirthDate.Text = user?.BirthDate?.ToString("dd/MM/yyyy") ?? "";
            this.DataContext = new {
                AvatarUrl = user?.ProfilePicture ?? "/HikariApp/Teacher/Resources/avatar.png"
            };
            // Lấy danh sách khoá học đã đăng ký
            var studentService = new StudentService();
            _enrollments = studentService.GetEnrollmentsWithCourse(student.StudentId);
            CourseList.ItemsSource = _enrollments;
            // Không chọn khoá học nào khi mở popup
            CourseList.SelectedIndex = -1;
            ProgressList.ItemsSource = null;
        }

        private void CourseList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedEnrollment = CourseList.SelectedItem as CourseEnrollment;
            if (selectedEnrollment != null)
            {
                // Lấy tiến trình các bài học của khoá này
                var progresses = _progressService.GetProgressByStudent(_student.StudentId)
                    .Where(p => p.EnrollmentId == selectedEnrollment.EnrollmentId)
                    .ToList();
                ProgressList.ItemsSource = progresses;
            }
            else
            {
                ProgressList.ItemsSource = null;
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
} 