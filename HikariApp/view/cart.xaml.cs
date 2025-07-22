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
using static HikariBusiness.Services.PaymentService;
using HikariDataAccess.Entities;

namespace HikariApp.view
{
    /// <summary>
    /// Interaction logic for Cart.xaml
    /// </summary>
    public partial class Cart : Window
    {
        private readonly CartService _cartService;
        private readonly PaymentService _paymentService;
        private readonly DiscountService _discountService;
        private const string CURRENT_STUDENT_ID = "S001"; // Hardcoded for demo
        private List<CartItemViewModel>? _cartItems;
        private Discount? _appliedDiscount;
        private decimal _originalTotal = 0;
        private decimal _discountAmount = 0;

        public Cart()
        {
            InitializeComponent();
            _cartService = new CartService();
            _paymentService = new PaymentService();
            _discountService = new DiscountService();
            LoadCartData();
            LoadAvailableDiscounts();
            InitializeDiscountUI();
        }
        
        // Load cart data from database
        private async void LoadCartData()
        {
            try
            {
                ShowLoading(true);
                
                _cartItems = await _cartService.GetCartItemsAsync(CURRENT_STUDENT_ID);
                
                ShowLoading(false);
                
                if (_cartItems == null || !_cartItems.Any())
                {
                    ShowEmptyCart(true);
                    UpdateSummary(0, 0);
                }
                else
                {
                    ShowEmptyCart(false);
                    DisplayCartItems();
                    UpdateSummary(_cartItems.Count, _cartItems.Sum(x => x.Fee));
                }
            }
            catch (Exception ex)
            {
                ShowLoading(false);
                MessageBox.Show($"Lỗi khi tải giỏ hàng: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Display cart items dynamically
        private void DisplayCartItems()
        {
            CartItemsContainer.Children.Clear();
            
            foreach (var item in _cartItems)
            {
                var cartItemBorder = CreateCartItemUI(item);
                CartItemsContainer.Children.Add(cartItemBorder);
            }
        }

        // Create UI for each cart item
        private Border CreateCartItemUI(CartItemViewModel item)
        {
            var border = new Border
            {
                Style = (Style)FindResource("CartItemStyle")
            };

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            // Course Icon
            var iconBorder = new Border
            {
                Background = GetCourseIconBackground(item.Title),
                CornerRadius = new CornerRadius(8),
                Width = 80,
                Height = 60
            };
            Grid.SetColumn(iconBorder, 0);

            var iconText = new TextBlock
            {
                Text = GetCourseIcon(item.Title),
                FontSize = 30,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Foreground = GetCourseIconColor(item.Title)
            };
            iconBorder.Child = iconText;

            // Course Info
            var infoPanel = new StackPanel
            {
                Margin = new Thickness(15, 0, 15, 0),
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(infoPanel, 1);

            infoPanel.Children.Add(new TextBlock
            {
                Text = item.Title,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(0, 0, 0, 5)
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = item.Description?.Length > 50 ? 
                       item.Description.Substring(0, 50) + "..." : 
                       item.Description,
                FontSize = 12,
                Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                Margin = new Thickness(0, 0, 0, 5)
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = item.FeeFormatted,
                FontSize = 16,
                FontWeight = FontWeights.Bold,
                Foreground = new SolidColorBrush(Color.FromRgb(255, 107, 53))
            });

            infoPanel.Children.Add(new TextBlock
            {
                Text = $"Thêm vào: {item.AddedDateFormatted}",
                FontSize = 10,
                Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153)),
                Margin = new Thickness(0, 5, 0, 0)
            });

            // Remove Button
            var actionPanel = new StackPanel
            {
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(actionPanel, 2);

            var removeButton = new Button
            {
                Content = "🗑️ Xóa",
                Background = new SolidColorBrush(Color.FromRgb(220, 53, 69)),
                Foreground = Brushes.White,
                BorderThickness = new Thickness(0),
                Padding = new Thickness(10, 5, 10, 5),
                FontSize = 10,
                Tag = item.CartId // Store cart ID for removal
            };
            removeButton.Click += RemoveButton_Click;

            // Button template for hover effect
            var template = new ControlTemplate(typeof(Button));
            var borderElement = new FrameworkElementFactory(typeof(Border));
            borderElement.SetBinding(Border.BackgroundProperty, new Binding("Background") { RelativeSource = RelativeSource.TemplatedParent });
            borderElement.SetValue(Border.CornerRadiusProperty, new CornerRadius(4));
            
            var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
            contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
            contentPresenter.SetBinding(ContentPresenter.MarginProperty, new Binding("Padding") { RelativeSource = RelativeSource.TemplatedParent });
            borderElement.AppendChild(contentPresenter);
            
            template.VisualTree = borderElement;
            
            var trigger = new Trigger { Property = Button.IsMouseOverProperty, Value = true };
            trigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(200, 35, 51))));
            template.Triggers.Add(trigger);
            
            removeButton.Template = template;
            actionPanel.Children.Add(removeButton);

            grid.Children.Add(iconBorder);
            grid.Children.Add(infoPanel);
            grid.Children.Add(actionPanel);
            border.Child = grid;

            return border;
        }

        // Remove item from cart
        private async void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var button = sender as Button;
                var cartId = (int)button.Tag;
                
                var result = MessageBox.Show("Bạn có chắc muốn xóa khóa học này khỏi giỏ hàng?", 
                                            "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                
                if (result == MessageBoxResult.Yes)
                {
                    var success = await _cartService.RemoveFromCartAsync(cartId);
                    if (success)
                    {
                        ShowNotification("Đã xóa khóa học khỏi giỏ hàng!");
                        LoadCartData(); // Reload cart
                    }
                    else
                    {
                        MessageBox.Show("Không thể xóa khóa học. Vui lòng thử lại.", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xóa khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Update summary section
        private void UpdateSummary(int courseCount, decimal total)
        {
            _originalTotal = total;
            
            CourseCountText.Text = courseCount.ToString();
            SubtotalText.Text = $"{total:N0} VNĐ";
            DiscountText.Text = $"-{_discountAmount:N0} VNĐ";
            
            var finalTotal = total - _discountAmount;
            TotalText.Text = $"{finalTotal:N0} VNĐ";
            
            // Enable/disable payment button based on cart content
            PaymentButton.IsEnabled = courseCount > 0;
            PaymentButton.Content = courseCount > 0 ? "💳 Thanh toán ngay" : "🚫 Giỏ hàng trống";
            
            // Ensure payment button is visible
            PaymentButton.Visibility = Visibility.Visible;
        }
        
        // Overload for when we need to recalculate with current cart
        private void UpdateSummary()
        {
            if (_cartItems != null && _cartItems.Any())
            {
                UpdateSummary(_cartItems.Count, _cartItems.Sum(x => x.Fee));
            }
            else
            {
                UpdateSummary(0, 0);
            }
        }

        // Show/hide loading indicator
        private void ShowLoading(bool show)
        {
            LoadingIndicator.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        // Show/hide empty cart message
        private void ShowEmptyCart(bool show)
        {
            EmptyCartMessage.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        // Helper methods for course icons
        private string GetCourseIcon(string courseTitle)
        {
            if (courseTitle.Contains("Nhật")) return "🏯";
            if (courseTitle.Contains("C#") || courseTitle.Contains("Programming")) return "💻";
            if (courseTitle.Contains("Web")) return "🌐";
            if (courseTitle.Contains("Data") || courseTitle.Contains("Python")) return "📊";
            if (courseTitle.Contains("Mobile")) return "📱";
            if (courseTitle.Contains("Security")) return "🔒";
            return "📚";
        }

        private Brush GetCourseIconBackground(string courseTitle)
        {
            if (courseTitle.Contains("Nhật")) return new SolidColorBrush(Color.FromRgb(255, 243, 224));
            if (courseTitle.Contains("C#")) return new SolidColorBrush(Color.FromRgb(227, 242, 253));
            if (courseTitle.Contains("Web")) return new SolidColorBrush(Color.FromRgb(232, 245, 232));
            if (courseTitle.Contains("Data")) return new SolidColorBrush(Color.FromRgb(253, 231, 243));
            if (courseTitle.Contains("Mobile")) return new SolidColorBrush(Color.FromRgb(243, 229, 245));
            return new SolidColorBrush(Color.FromRgb(245, 245, 245));
        }

        private Brush GetCourseIconColor(string courseTitle)
        {
            if (courseTitle.Contains("Nhật")) return new SolidColorBrush(Color.FromRgb(245, 124, 0));
            if (courseTitle.Contains("C#")) return new SolidColorBrush(Color.FromRgb(25, 118, 210));
            if (courseTitle.Contains("Web")) return new SolidColorBrush(Color.FromRgb(76, 175, 80));
            if (courseTitle.Contains("Data")) return new SolidColorBrush(Color.FromRgb(233, 30, 99));
            if (courseTitle.Contains("Mobile")) return new SolidColorBrush(Color.FromRgb(156, 39, 176));
            return new SolidColorBrush(Color.FromRgb(102, 102, 102));
        }

        // Show notification
        private void ShowNotification(string message)
        {
            MessageBox.Show(message, "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Refresh button click
        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            LoadCartData();
        }

        // Payment button click
        private async void PaymentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate payment first
                var validation = await _paymentService.ValidatePaymentAsync(CURRENT_STUDENT_ID);
                if (!validation.IsValid)
                {
                    MessageBox.Show(validation.Message, "Không thể thanh toán", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Show payment confirmation
                var confirmMessage = $"Xác nhận thanh toán:\n\n" +
                                   $"Số khóa học: {validation.ItemsCount}\n" +
                                   $"Tổng tiền: {validation.TotalAmount:N0} VNĐ\n\n" +
                                   $"Bạn có muốn tiếp tục thanh toán?";

                var result = MessageBox.Show(confirmMessage, "Xác nhận thanh toán", 
                                            MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    await ProcessPayment();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Process payment
        private async Task ProcessPayment()
        {
            try
            {
                // Show loading
                PaymentButton.IsEnabled = false;
                PaymentButton.Content = "⏳ Đang xử lý...";

                // Process payment with discount if applied
                var paymentResult = await _paymentService.ProcessPaymentAsync(CURRENT_STUDENT_ID, PaymentMethod.CreditCard, _appliedDiscount);

                if (paymentResult.Success)
                {
                    // Show success message with discount info
                    var successMessage = $"Thanh toán thành công!\n\n" +
                                        $"Mã thanh toán: {paymentResult.PaymentId}\n";
                    
                    if (paymentResult.DiscountAmount > 0)
                    {
                        successMessage += $"Tổng tiền gốc: {paymentResult.OriginalAmount:N0} VNĐ\n" +
                                         $"Giảm giá ({paymentResult.DiscountCode}): -{paymentResult.DiscountAmount:N0} VNĐ\n" +
                                         $"Thành tiền: {paymentResult.TotalAmount:N0} VNĐ\n";
                    }
                    else
                    {
                        successMessage += $"Số tiền: {paymentResult.TotalAmount:N0} VNĐ\n";
                    }
                    
                    successMessage += $"Đã đăng ký: {paymentResult.EnrolledCoursesCount} khóa học\n\n" +
                                     $"Bạn có muốn chuyển đến trang 'Khóa học của tôi'?";

                    var goToMyCourses = MessageBox.Show(successMessage, "Thanh toán thành công", 
                                                       MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (goToMyCourses == MessageBoxResult.Yes)
                    {
                        // Navigate to MyCourses
                        var myCoursesWindow = new MyCourses();
                        myCoursesWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        // Reload cart (should be empty now)
                        LoadCartData();
                        
                        // Show success notification
                        ShowNotification($"Thanh toán thành công! Đã đăng ký {paymentResult.EnrolledCoursesCount} khóa học.");
                    }
                }
                else
                {
                    MessageBox.Show(paymentResult.Message, "Thanh toán thất bại", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xử lý thanh toán: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                // Reset button
                PaymentButton.IsEnabled = true;
                PaymentButton.Content = "💳 Thanh toán ngay";
            }
        }

        private void CoursesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var coursesWindow = new Course();
                coursesWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở trang khóa học: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private void MyCoursesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var myCoursesWindow = new MyCourses();
                myCoursesWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi mở khóa học của tôi: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Discount functionality
        private void InitializeDiscountUI()
        {
            DiscountCodeTextBox.Text = "Nhập mã giảm giá";
            DiscountCodeTextBox.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void DiscountCodeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (DiscountCodeTextBox.Text == "Nhập mã giảm giá")
            {
                DiscountCodeTextBox.Text = "";
                DiscountCodeTextBox.Foreground = new SolidColorBrush(Colors.Black);
            }
        }

        private void DiscountCodeTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(DiscountCodeTextBox.Text))
            {
                DiscountCodeTextBox.Text = "Nhập mã giảm giá";
                DiscountCodeTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            }
        }

        private async void ApplyDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var code = DiscountCodeTextBox.Text?.Trim();
                if (string.IsNullOrEmpty(code) || code == "Nhập mã giảm giá")
                {
                    ShowDiscountStatus("Vui lòng nhập mã giảm giá", false);
                    return;
                }

                ApplyDiscountButton.IsEnabled = false;
                ApplyDiscountButton.Content = "Đang kiểm tra...";

                // Validate discount code
                var result = await _discountService.ValidateDiscountCodeAsync(code);
                
                if (result.IsValid && result.Discount != null)
                {
                    // Check if discount applies to any course in cart
                    var applicableCourse = _cartItems?.FirstOrDefault(item => item.CourseId == result.Discount.CourseId);
                    
                    if (applicableCourse != null)
                    {
                        _appliedDiscount = result.Discount;
                        ApplyDiscount();
                        ShowAppliedDiscount();
                        ShowDiscountStatus(result.Message, true);
                        
                        // Force payment button to be visible
                        PaymentButton.Visibility = Visibility.Visible;
                        PaymentButton.IsEnabled = true;
                    }
                    else
                    {
                        ShowDiscountStatus($"Mã giảm giá chỉ áp dụng cho khóa học: {result.Discount.Course.Title}. Khóa học này không có trong giỏ hàng.", false);
                    }
                }
                else
                {
                    ShowDiscountStatus(result.Message, false);
                }
            }
            catch (Exception ex)
            {
                ShowDiscountStatus($"Lỗi khi áp dụng mã giảm giá: {ex.Message}", false);
            }
            finally
            {
                ApplyDiscountButton.IsEnabled = true;
                ApplyDiscountButton.Content = "Áp dụng";
            }
        }

        private void RemoveDiscountButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveDiscount();
        }

        private void ApplyDiscount()
        {
            if (_appliedDiscount == null || _cartItems == null) return;

            var applicableCourse = _cartItems.FirstOrDefault(item => item.CourseId == _appliedDiscount.CourseId);
            if (applicableCourse != null)
            {
                _discountAmount = _discountService.CalculateDiscountAmount(applicableCourse.Fee, _appliedDiscount.DiscountPercent ?? 0);
                UpdateSummary();
            }
        }

        private void RemoveDiscount()
        {
            _appliedDiscount = null;
            _discountAmount = 0;
            AppliedDiscountInfo.Visibility = Visibility.Collapsed;
            DiscountCodeTextBox.Text = "Nhập mã giảm giá";
            DiscountCodeTextBox.Foreground = new SolidColorBrush(Colors.Gray);
            HideDiscountStatus();
            UpdateSummary();
        }

        private void ShowAppliedDiscount()
        {
            if (_appliedDiscount == null) return;

            AppliedDiscountCode.Text = $"Mã: {_appliedDiscount.Code} (-{_appliedDiscount.DiscountPercent}%)";
            AppliedDiscountDescription.Text = $"Áp dụng cho: {_appliedDiscount.Course.Title}";
            AppliedDiscountInfo.Visibility = Visibility.Visible;
            
            // Ensure payment button remains visible
            PaymentButton.Visibility = Visibility.Visible;
        }

        private void ShowDiscountStatus(string message, bool isSuccess)
        {
            DiscountStatusText.Text = message;
            DiscountStatusText.Foreground = new SolidColorBrush(isSuccess ? Colors.Green : Colors.Red);
            DiscountStatusText.Visibility = Visibility.Visible;
        }

        private void HideDiscountStatus()
        {
            DiscountStatusText.Visibility = Visibility.Collapsed;
        }

        private async void LoadAvailableDiscounts()
        {
            try
            {
                var discounts = await _discountService.GetActiveDiscountsAsync();
                PopulateAvailableDiscounts(discounts);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading available discounts: {ex.Message}");
            }
        }

        private void PopulateAvailableDiscounts(List<Discount> discounts)
        {
            AvailableDiscountsPanel.Children.Clear();

            if (!discounts.Any())
            {
                var noDiscountText = new TextBlock
                {
                    Text = "Hiện tại không có mã giảm giá nào khả dụng",
                    FontSize = 10,
                    Foreground = new SolidColorBrush(Colors.Gray),
                    FontStyle = FontStyles.Italic
                };
                AvailableDiscountsPanel.Children.Add(noDiscountText);
                return;
            }

            foreach (var discount in discounts.Take(5)) // Show max 5 discounts
            {
                var discountButton = new Button
                {
                    Content = $"{discount.Code} (-{discount.DiscountPercent}%) - {discount.Course.Title}",
                    FontSize = 10,
                    Padding = new Thickness(8, 4, 8, 4),
                    Margin = new Thickness(0, 2, 0, 2),
                    Background = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(224, 224, 224)),
                    BorderThickness = new Thickness(1),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Tag = discount.Code
                };

                discountButton.Click += (s, e) =>
                {
                    DiscountCodeTextBox.Text = discount.Code;
                    DiscountCodeTextBox.Foreground = new SolidColorBrush(Colors.Black);
                    AvailableDiscountsExpander.IsExpanded = false;
                };

                AvailableDiscountsPanel.Children.Add(discountButton);
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _cartService?.Dispose();
            _paymentService?.Dispose();
            _discountService?.Dispose();
            base.OnClosed(e);
        }
    }
}
