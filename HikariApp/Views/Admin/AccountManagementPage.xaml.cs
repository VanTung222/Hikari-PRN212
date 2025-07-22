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
    /// Interaction logic for AccountManagementPage.xaml
    /// </summary>
    public partial class AccountManagementPage : Page
    {
        private readonly AccountService _accountService;
        private ObservableCollection<AccountViewModel> _accounts;
        private List<AccountViewModel> _allAccounts;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;

        public AccountManagementPage()
        {
            InitializeComponent();
            _accountService = new AccountService();
            _accounts = new ObservableCollection<AccountViewModel>();
            AccountDataGrid.ItemsSource = _accounts;
            LoadAccountsAsync();

            this.Unloaded += AccountManagementPage_Unloaded; // Đăng ký sự kiện
        }

        private async void LoadAccountsAsync()
        {
            try
            {
                _allAccounts = await _accountService.GetAllAccountsAsync();
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            if (_allAccounts == null || _allAccounts.Count == 0)
            {
                _accounts.Clear();
                _totalPages = 1;
                _currentPage = 1;
                PageInfo.Text = "Trang 1 / 1";
                PrevButton.IsEnabled = false;
                NextButton.IsEnabled = false;
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_allAccounts.Count / _itemsPerPage);
            
            // Get items for current page
            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pageItems = _allAccounts.Skip(startIndex).Take(_itemsPerPage).ToList();
            
            _accounts.Clear();
            foreach (var account in pageItems)
            {
                _accounts.Add(account);
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
                string username = UsernameTextBox.Text?.Trim();
                string fullName = FullNameTextBox.Text?.Trim();
                DateTime? fromDate = FromDatePicker.SelectedDate;
                DateTime? toDate = ToDatePicker.SelectedDate;

                _allAccounts = await _accountService.SearchAccountsAsync(username, fullName, fromDate, toDate);
                _currentPage = 1;
                UpdatePagination();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear all filter controls
            UsernameTextBox.Text = string.Empty;
            FullNameTextBox.Text = string.Empty;
            FromDatePicker.SelectedDate = null;
            ToDatePicker.SelectedDate = null;
            
            // Reload all data
            LoadAccountsAsync();
        }

        private async void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is AccountViewModel account)
            {
                try
                {
                    var accountDetails = await _accountService.GetAccountByIdAsync(account.Id);
                    if (accountDetails != null)
                    {
                        var detailsWindow = new AccountDetailsDialog(accountDetails)
                        {
                            Owner = Window.GetWindow(this)
                        };
                        detailsWindow.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin tài khoản.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (sender is Button button && button.DataContext is AccountViewModel account)
            {
                try
                {
                    var accountDetails = await _accountService.GetAccountByIdAsync(account.Id);
                    if (accountDetails != null)
                    {
                        var editDialog = new EditAccountDialog(accountDetails)
                        {
                            Owner = Window.GetWindow(this)
                        };
                        
                        if (editDialog.ShowDialog() == true)
                        {
                            // Refresh the accounts list
                            LoadAccountsAsync();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy thông tin tài khoản.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (sender is Button button && button.DataContext is AccountViewModel account)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn khóa tài khoản: {account.FullName}?\n\nTài khoản sẽ bị vô hiệu hóa và không thể đăng nhập.", 
                    "Xác nhận khóa tài khoản", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        bool success = await _accountService.BlockAccountAsync(account.Id);
                        if (success)
                        {
                            MessageBox.Show($"Đã khóa tài khoản {account.FullName} thành công.", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                            LoadAccountsAsync(); // Refresh the list
                        }
                        else
                        {
                            MessageBox.Show("Không thể khóa tài khoản. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi khóa tài khoản: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }


        private void AccountManagementPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _accountService?.Dispose();
        }

        private void AddAccountButton_Click(object sender, RoutedEventArgs e)
        {
            var addDialog = new AddAccountDialog()
            {
                Owner = Window.GetWindow(this)
            };
            
            if (addDialog.ShowDialog() == true || addDialog.IsSuccess)
            {
                // Refresh the accounts list
                LoadAccountsAsync();
            }
        }

        public class AccountModel
        {
            public string Id { get; set; }
            public string Avatar { get; set; }
            public string FullName { get; set; }
            public string Username { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public string Status { get; set; }
            public int CourseCount { get; set; }
            public string CreatedDate { get; set; }
        }

    }
}
