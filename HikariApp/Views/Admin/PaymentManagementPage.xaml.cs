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
    /// Interaction logic for PaymentManagementPage.xaml
    /// </summary>
    public partial class PaymentManagementPage : Page
    {
        private readonly PaymentService _paymentService;
        private ObservableCollection<PaymentViewModel> _payments;
        private List<PaymentViewModel> _allPayments;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;

        public PaymentManagementPage()
        {
            InitializeComponent();
            _paymentService = new PaymentService();
            _payments = new ObservableCollection<PaymentViewModel>();
            PaymentDataGrid.ItemsSource = _payments;
            LoadPaymentsAsync();
            this.Unloaded += PaymentManagementPage_Unloaded;
        }

        private async Task LoadPaymentsAsync()
        {
            try
            {
                _allPayments = await _paymentService.GetAllPaymentsAsync();
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            if (_allPayments == null || _allPayments.Count == 0)
            {
                _payments.Clear();
                _totalPages = 1;
                _currentPage = 1;
                PageInfo.Text = "Trang 1 / 1";
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_allPayments.Count / _itemsPerPage);
            
            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pageItems = _allPayments.Skip(startIndex).Take(_itemsPerPage).ToList();
            
            _payments.Clear();
            foreach (var payment in pageItems)
            {
                _payments.Add(payment);
            }
            
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
                string courseLevel = null;
                string status = null;
                DateTime? fromDate = FromDatePicker.SelectedDate;
                DateTime? toDate = ToDatePicker.SelectedDate;

                // Get course level from ComboBox
                if (CourseComboBox.SelectedItem is ComboBoxItem courseItem)
                {
                    string selectedCourse = courseItem.Content.ToString();
                    if (selectedCourse != "Tất cả")
                    {
                        courseLevel = selectedCourse;
                    }
                }

                // Get status from ComboBox
                if (StatusComboBox.SelectedItem is ComboBoxItem statusItem)
                {
                    string selectedStatus = statusItem.Content.ToString();
                    if (selectedStatus != "Tất cả")
                    {
                        status = selectedStatus switch
                        {
                            "Thành công" => "Success",
                            "Chờ xử lý" => "Pending",
                            "Thất bại" => "Failed",
                            _ => null
                        };
                    }
                }

                _allPayments = await _paymentService.SearchPaymentsAsync(courseLevel, status, fromDate, toDate);
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all filter controls
            CourseComboBox.SelectedIndex = 0; // Reset to "Tất cả"
            StatusComboBox.SelectedIndex = 0; // Reset to "Tất cả"
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            
            // Reload all data
            LoadPaymentsAsync();
        }



        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is PaymentViewModel payment)
            {
                try
                {
                    var detailsDialog = new PaymentDetailsDialog(payment);
                    detailsDialog.Owner = Window.GetWindow(this);
                    detailsDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi xem chi tiết thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is PaymentViewModel payment)
            {
                try
                {
                    var editDialog = new EditPaymentDialog(payment);
                    editDialog.Owner = Window.GetWindow(this);
                    
                    if (editDialog.ShowDialog() == true)
                    {
                        // Refresh the data after successful edit
                        await LoadPaymentsAsync();
                        MessageBox.Show("Cập nhật thanh toán thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi chỉnh sửa thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is PaymentViewModel payment)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa thanh toán: {payment.PaymentCode}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Implement delete functionality
                    MessageBox.Show("Tính năng xóa sẽ được triển khai sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void PaymentManagementPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _paymentService?.Dispose();
        }
    }
}
