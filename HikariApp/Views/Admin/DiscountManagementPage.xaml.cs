using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HikariBusiness.Services;

namespace HikariApp.Views.Admin
{
    /// <summary>
    /// Interaction logic for DiscountManagementPage.xaml
    /// </summary>
    public partial class DiscountManagementPage : Page
    {
        private readonly DiscountService _discountService;
        private ObservableCollection<DiscountViewModel> _discounts;
        private List<DiscountViewModel> _allDiscounts;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;

        public DiscountManagementPage()
        {
            InitializeComponent();
            _discountService = new DiscountService();
            _discounts = new ObservableCollection<DiscountViewModel>();
            DiscountDataGrid.ItemsSource = _discounts;
            LoadDiscountsAsync();
            this.Unloaded += DiscountManagementPage_Unloaded;
        }

        private async void LoadDiscountsAsync()
        {
            try
            {
                _allDiscounts = await _discountService.GetAllDiscountsAsync();
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu giảm giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            if (_allDiscounts == null || _allDiscounts.Count == 0)
            {
                _discounts.Clear();
                _totalPages = 1;
                _currentPage = 1;
                PageInfo.Text = "Trang 1 / 1";
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_allDiscounts.Count / _itemsPerPage);
            
            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pageItems = _allDiscounts.Skip(startIndex).Take(_itemsPerPage).ToList();
            
            _discounts.Clear();
            foreach (var discount in pageItems)
            {
                _discounts.Add(discount);
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
                decimal? discountPercent = null;
                string status = null;

                // Parse discount percent
                if (!string.IsNullOrEmpty(DiscountPercentTextBox.Text?.Trim()))
                {
                    if (decimal.TryParse(DiscountPercentTextBox.Text.Trim(), out decimal percent))
                        discountPercent = percent;
                }

                // Get status from ComboBox
                if (StatusComboBox.SelectedItem is ComboBoxItem selectedItem)
                {
                    string selectedContent = selectedItem.Content.ToString();
                    if (selectedContent != "Tất cả")
                    {
                        status = selectedContent == "Hoạt động" ? "Active" : "Inactive";
                    }
                }

                // Gọi phương thức với tham số phù hợp
                _allDiscounts = await _discountService.SearchDiscountsAsync(
                    searchTerm: DiscountPercentTextBox.Text, // Sử dụng giá trị từ DiscountPercentTextBox làm searchTerm
                    type: discountPercent.HasValue ? GetDiscountType((int)discountPercent.Value) : null, // Chuyển discountPercent thành type
                    isActive: status != null ? status == "Active" : (bool?)null // Chuyển status thành isActive
                );
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm giảm giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Thêm phương thức hỗ trợ (nếu chưa có)
        private string GetDiscountType(int discountPercent)
        {
            return discountPercent switch
            {
                < 20 => "Thấp",
                >= 20 and < 50 => "Trung bình",
                _ => "Cao"
            };
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all filter controls
            DiscountPercentTextBox.Text = string.Empty;
            StatusComboBox.SelectedIndex = 0; // Reset to "Tất cả"
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            
            // Reload all data
            LoadDiscountsAsync();
        }



        private async void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is DiscountViewModel discount)
            {
                try
                {
                    var discountDetails = await _discountService.GetDiscountByIdAsync(discount.Id);
                    if (discountDetails != null)
                    {
                        var detailsWindow = new DiscountDetailsDialog(discountDetails)
                        {
                            Owner = Window.GetWindow(this)
                        };
                        detailsWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin mã giảm giá.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (sender is Button button && button.DataContext is DiscountViewModel discount)
            {
                try
                {
                    var discountDetails = await _discountService.GetDiscountByIdAsync(discount.Id);
                    if (discountDetails != null)
                    {
                        var editDialog = new EditDiscountDialog(discountDetails)
                        {
                            Owner = Window.GetWindow(this)
                        };
                        
                        if (editDialog.ShowDialog() == true)
                        {
                            LoadDiscountsAsync(); // Refresh the list
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin mã giảm giá.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (sender is Button button && button.DataContext is DiscountViewModel discount)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn vô hiệu hóa mã giảm giá: {discount.Code}?\n\nMã giảm giá sẽ không còn có thể sử dụng.", 
                    "Xác nhận vô hiệu hóa", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = await _discountService.DeactivateDiscountAsync(discount.Id);
                        if (success)
                        {
                            MessageBox.Show($"Đã vô hiệu hóa mã giảm giá {discount.Code} thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadDiscountsAsync(); // Refresh the list
                        }
                        else
                        {
                            MessageBox.Show("Không thể vô hiệu hóa mã giảm giá. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi vô hiệu hóa mã giảm giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DiscountManagementPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _discountService?.Dispose();
        }

        private void AddDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            var addDialog = new AddDiscountDialog()
            {
                Owner = Window.GetWindow(this)
            };
            
            if (addDialog.ShowDialog() == true || addDialog.IsSuccess)
            {
                // Refresh the discounts list
                LoadDiscountsAsync();
            }
        }
    }
}
