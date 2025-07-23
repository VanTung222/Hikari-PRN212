using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HikariBusiness.Services;
using HikariDataAccess.Entities;

namespace HikariApp
{
    /// <summary>
    /// Interaction logic for CourseListPage.xaml
    /// </summary>
    public partial class CourseListPage : Window
    {
        public CourseListPage()
        {
            InitializeComponent();
            LoadCourseData();
        }

        private void LoadCourseData()
        {
            try
            {
                CourseManager courseManager = new CourseManager();
                // Lấy tất cả các khóa học đang hoạt động
                List<Course> courses = courseManager.GetAllActiveCourses();
                CoursesListView.ItemsSource = courses;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                // Có thể đóng ứng dụng hoặc ẩn giao diện nếu không tải được dữ liệu quan trọng
            }
        }

        private void ViewDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            if (btn != null)
            {
                // Lấy CourseID từ Tag của nút
                string courseId = btn.Tag?.ToString(); // Sử dụng ?. để tránh lỗi nếu Tag là null

                if (!string.IsNullOrEmpty(courseId))
                {
                    // Mở trang CourseDetailPage và truyền CourseID
                    CourseDetailPage courseDetailPage = new CourseDetailPage(courseId);
                    courseDetailPage.ShowDialog(); // ShowDialog() để chặn cửa sổ hiện tại và quay lại khi cửa sổ mới đóng

                    // Tùy chọn: Nếu bạn muốn tải lại dữ liệu sau khi quay lại từ CourseDetailPage,
                    // bạn có thể gọi LoadCourseData() ở đây
                    // LoadCourseData();
                }
                else
                {
                    MessageBox.Show("Không thể lấy Course ID. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Đóng CourseListPage và quay lại trang trước đó (nếu có)
                          // Ví dụ: Trang đăng nhập/đăng ký nếu đây là trang đầu tiên sau khi login
        }
    }
}
