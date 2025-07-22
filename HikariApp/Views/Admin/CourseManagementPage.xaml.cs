using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    /// <summary>
    /// Interaction logic for CourseManagementPage.xaml
    /// </summary>
    public partial class CourseManagementPage : Page
    {
        private readonly CourseService _courseService;
        private ObservableCollection<CourseViewModel> _courses;
        private List<CourseViewModel> _allCourses;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;

        public CourseManagementPage()
        {
            InitializeComponent();
            _courseService = new CourseService();
            _courses = new ObservableCollection<CourseViewModel>();
            CourseDataGrid.ItemsSource = _courses;
            LoadCoursesAsync();
            this.Unloaded += CourseManagementPage_Unloaded;
        }

        private async void LoadCoursesAsync()
        {
            try
            {
                _allCourses = await _courseService.GetAllCoursesAsync();
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            if (_allCourses == null || _allCourses.Count == 0)
            {
                _courses.Clear();
                _totalPages = 1;
                _currentPage = 1;
                PageInfo.Text = "Trang 1 / 1";
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_allCourses.Count / _itemsPerPage);
            
            // Get items for current page
            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pageItems = _allCourses.Skip(startIndex).Take(_itemsPerPage).ToList();
            
            _courses.Clear();
            foreach (var course in pageItems)
            {
                _courses.Add(course);
            }
            
            // Update UI
            PageInfo.Text = $"Trang {_currentPage} / {_totalPages}";
            PrevButton.IsEnabled = _currentPage > 1;
            NextButton.IsEnabled = _currentPage < _totalPages;
        }

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage > 1)
            {
                _currentPage--;
                UpdatePagination();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentPage < _totalPages)
            {
                _currentPage++;
                UpdatePagination();
            }
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get search criteria from UI controls
                string courseName = CourseNameTextBox.Text?.Trim();
                decimal? minFee = null;
                decimal? maxFee = null;
                DateTime? startDate = StartDatePicker.SelectedDate;

                // Parse fee values
                if (!string.IsNullOrEmpty(MinFeeTextBox.Text?.Trim()))
                {
                    if (decimal.TryParse(MinFeeTextBox.Text.Trim(), out decimal min))
                        minFee = min;
                }
                if (!string.IsNullOrEmpty(MaxFeeTextBox.Text?.Trim()))
                {
                    if (decimal.TryParse(MaxFeeTextBox.Text.Trim(), out decimal max))
                        maxFee = max;
                }

                _allCourses = await _courseService.SearchCoursesAsync(courseName, minFee, maxFee, startDate);
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all filter controls
            CourseNameTextBox.Text = string.Empty;
            MinFeeTextBox.Text = string.Empty;
            MaxFeeTextBox.Text = string.Empty;
            StartDatePicker.SelectedDate = null;
            
            // Reload all data
            LoadCoursesAsync();
        }


        private async void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CourseViewModel course)
            {
                try
                {
                    var courseDetails = await _courseService.GetCourseByIdAsync(course.Id);
                    if (courseDetails != null)
                    {
                        var detailsWindow = new CourseDetailsDialog(courseDetails)
                        {
                            Owner = Window.GetWindow(this)
                        };
                        detailsWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin khóa học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi xem chi tiết: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CourseViewModel course)
            {
                try
                {
                    var courseDetails = await _courseService.GetCourseByIdAsync(course.Id);
                    if (courseDetails != null)
                    {
                        var editDialog = new EditCourseDialog(courseDetails)
                        {
                            Owner = Window.GetWindow(this)
                        };
                        
                        if (editDialog.ShowDialog() == true)
                        {
                            LoadCoursesAsync(); // Refresh the list
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin khóa học.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi chỉnh sửa: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is CourseViewModel course)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn vô hiệu hóa khóa học: {course.Title}?\n\nKhóa học sẽ không còn hiển thị cho học viên mới.", 
                    "Xác nhận vô hiệu hóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = await _courseService.DeactivateCourseAsync(course.Id);
                        if (success)
                        {
                            MessageBox.Show($"Đã vô hiệu hóa khóa học {course.Title} thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadCoursesAsync(); // Refresh the list
                        }
                        else
                        {
                            MessageBox.Show("Không thể vô hiệu hóa khóa học. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi vô hiệu hóa khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void CourseManagementPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _courseService?.Dispose();
        }

        private void AddCourseButton_Click(object sender, RoutedEventArgs e)
        {
            var addDialog = new AddCourseDialog()
            {
                Owner = Window.GetWindow(this)
            };
            
            if (addDialog.ShowDialog() == true || addDialog.IsSuccess)
            {
                // Refresh the courses list
                LoadCoursesAsync();
            }
        }
    }
}
