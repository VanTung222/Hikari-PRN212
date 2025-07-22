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
using System.Windows.Markup;
using System.IO;

namespace HikariApp.Views.Admin
{
    /// <summary>
    /// Interaction logic for ReviewManagementPage.xaml
    /// </summary>
    public partial class ReviewManagementPage : Page
    {
        private readonly ReviewService _reviewService;
        private ObservableCollection<ReviewViewModel> _reviews;
        private List<ReviewViewModel> _allReviews;
        private DataGrid _reviewDataGrid;
        private int _currentPage = 1;
        private int _itemsPerPage = 10;
        private int _totalPages = 1;
        private Button _prevButton;
        private Button _nextButton;
        private TextBlock _pageInfo;
        
        // Filter controls
        private TextBox _searchTextBox;
        private ComboBox _studentComboBox;
        private ComboBox _courseComboBox;
        private ComboBox _minRatingComboBox;
        private ComboBox _maxRatingComboBox;
        private ComboBox _statusComboBox;
        private DatePicker _fromDatePicker;
        private DatePicker _toDatePicker;

        public ReviewManagementPage()
        {
            _reviewService = new ReviewService();
            _reviews = new ObservableCollection<ReviewViewModel>();
            
            // Create UI programmatically to avoid XAML loading issues
            CreateUIManually();
            
            this.Loaded += ReviewManagementPage_Loaded;
            this.Unloaded += ReviewManagementPage_Unloaded;
        }

        private void CreateUIManually()
        {
            // Create the main grid
            var mainGrid = new Grid();
            mainGrid.Margin = new Thickness(20);
            
            // Add row definitions
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Filter section
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }); // Data table
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto }); // Pagination

            // Create Filter Section
            var filterBorder = CreateFilterSection();
            Grid.SetRow(filterBorder, 0);
            mainGrid.Children.Add(filterBorder);

            // Create the DataGrid
            _reviewDataGrid = new DataGrid
            {
                Name = "ReviewDataGrid",
                AutoGenerateColumns = false,
                CanUserAddRows = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                GridLinesVisibility = DataGridGridLinesVisibility.Horizontal,
                AlternatingRowBackground = new SolidColorBrush(Color.FromRgb(248, 249, 250)),
                Background = Brushes.White,
                RowHeight = 45,
                FontSize = 13,
                BorderThickness = new Thickness(0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Hidden,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden
            };

            // Add columns to DataGrid
            AddDataGridColumns();

            // Create border for DataGrid
            var dataTableBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(0),
                Height = 500,
                Child = _reviewDataGrid
            };
            
            Grid.SetRow(dataTableBorder, 1);
            mainGrid.Children.Add(dataTableBorder);

            // Create Pagination Section
            var paginationPanel = CreatePaginationSection();
            Grid.SetRow(paginationPanel, 2);
            mainGrid.Children.Add(paginationPanel);
            
            this.Content = mainGrid;
        }

        private Border CreateFilterSection()
        {
            var mainGrid = new Grid();
            
            // Create two rows for better layout
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            mainGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            
            // First row - Search, Student, Course, Status
            var firstRowGrid = new Grid();
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(200) });
            firstRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            
            // Search TextBox
            var searchLabel = new TextBlock
            {
                Text = "Tìm Kiếm",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(searchLabel, 0);
            firstRowGrid.Children.Add(searchLabel);

            _searchTextBox = new TextBox
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(_searchTextBox, 1);
            firstRowGrid.Children.Add(_searchTextBox);

            // Student Name ComboBox
           

            _studentComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _studentComboBox.Items.Add("Tất cả");
            _studentComboBox.SelectedIndex = 0;
            Grid.SetColumn(_studentComboBox, 3);
            firstRowGrid.Children.Add(_studentComboBox);

            // Course ComboBox
            var courseLabel = new TextBlock
            {
                Text = "Khóa Học",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(courseLabel, 4);
            firstRowGrid.Children.Add(courseLabel);

            _courseComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _courseComboBox.Items.Add("Tất cả");
            _courseComboBox.SelectedIndex = 0;
            Grid.SetColumn(_courseComboBox, 5);
            firstRowGrid.Children.Add(_courseComboBox);

            // Status ComboBox
            var statusLabel = new TextBlock
            {
                Text = "Trạng Thái",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(statusLabel, 6);
            firstRowGrid.Children.Add(statusLabel);

            _statusComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _statusComboBox.Items.Add("Tất cả");
            _statusComboBox.Items.Add("Đã duyệt");
            _statusComboBox.Items.Add("Chờ duyệt");
            _statusComboBox.SelectedIndex = 0;
            Grid.SetColumn(_statusComboBox, 7);
            firstRowGrid.Children.Add(_statusComboBox);
            
            Grid.SetRow(firstRowGrid, 0);
            mainGrid.Children.Add(firstRowGrid);
            
            // Second row - Rating filters, Date filters, and Action buttons
            var secondRowGrid = new Grid();
            secondRowGrid.Margin = new Thickness(0, 15, 0, 0);
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(120) });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(150) });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            secondRowGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            
            // Min Rating
            var minRatingLabel = new TextBlock
            {
                Text = "Điểm Tối Thiểu",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(minRatingLabel, 0);
            secondRowGrid.Children.Add(minRatingLabel);

            _minRatingComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _minRatingComboBox.Items.Add("Tất cả");
            for (int i = 1; i <= 5; i++)
            {
                _minRatingComboBox.Items.Add(i.ToString());
            }
            _minRatingComboBox.SelectedIndex = 0;
            Grid.SetColumn(_minRatingComboBox, 1);
            secondRowGrid.Children.Add(_minRatingComboBox);

            // Max Rating
            var maxRatingLabel = new TextBlock
            {
                Text = "Điểm Tối Đa",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(maxRatingLabel, 2);
            secondRowGrid.Children.Add(maxRatingLabel);

            _maxRatingComboBox = new ComboBox
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            _maxRatingComboBox.Items.Add("Tất cả");
            for (int i = 1; i <= 5; i++)
            {
                _maxRatingComboBox.Items.Add(i.ToString());
            }
            _maxRatingComboBox.SelectedIndex = 0;
            Grid.SetColumn(_maxRatingComboBox, 3);
            secondRowGrid.Children.Add(_maxRatingComboBox);

            // From Date
            var fromDateLabel = new TextBlock
            {
                Text = "Từ Ngày",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(fromDateLabel, 4);
            secondRowGrid.Children.Add(fromDateLabel);

            _fromDatePicker = new DatePicker
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(_fromDatePicker, 5);
            secondRowGrid.Children.Add(_fromDatePicker);

            // To Date
            var toDateLabel = new TextBlock
            {
                Text = "Đến Ngày",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 0, 10, 0)
            };
            Grid.SetColumn(toDateLabel, 6);
            secondRowGrid.Children.Add(toDateLabel);

            _toDatePicker = new DatePicker
            {
                Margin = new Thickness(0, 0, 20, 0),
                Height = 30,
                VerticalContentAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(_toDatePicker, 7);
            secondRowGrid.Children.Add(_toDatePicker);

            // Action Buttons
            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };

            var searchButton = new Button
            {
                Content = "Tìm Kiếm",
                Background = new SolidColorBrush(Color.FromRgb(92, 184, 92)),
                Foreground = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 10, 0),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };
            searchButton.Click += SearchButton_Click;

            var resetButton = new Button
            {
                Content = "Đặt Lại",
                Background = new SolidColorBrush(Color.FromRgb(108, 117, 125)),
                Foreground = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold
            };
            resetButton.Click += ResetButton_Click;

            buttonPanel.Children.Add(searchButton);
            buttonPanel.Children.Add(resetButton);
            Grid.SetColumn(buttonPanel, 9);
            secondRowGrid.Children.Add(buttonPanel);
            
            Grid.SetRow(secondRowGrid, 1);
            mainGrid.Children.Add(secondRowGrid);

            // Create filter border
            var filterBorder = new Border
            {
                Background = Brushes.White,
                CornerRadius = new CornerRadius(8),
                Padding = new Thickness(20),
                Margin = new Thickness(0, 0, 0, 20),
                Child = mainGrid
            };

            return filterBorder;
        }

        private StackPanel CreatePaginationSection()
        {
            var paginationPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 20, 0, 0)
            };

            _prevButton = new Button
            {
                Content = "< Trước",
                Background = new SolidColorBrush(Color.FromRgb(108, 117, 125)),
                Foreground = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(0, 0, 10, 0),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                IsEnabled = false
            };
            _prevButton.Click += PrevButton_Click;

            _pageInfo = new TextBlock
            {
                Text = "Trang 1 / 1",
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 10, 0)
            };

            _nextButton = new Button
            {
                Content = "Sau >",
                Background = new SolidColorBrush(Color.FromRgb(108, 117, 125)),
                Foreground = Brushes.White,
                Padding = new Thickness(8, 4, 8, 4),
                Margin = new Thickness(10, 0, 0, 0),
                BorderThickness = new Thickness(0),
                FontSize = 12,
                FontWeight = FontWeights.Bold,
                IsEnabled = false
            };
            _nextButton.Click += NextButton_Click;

            paginationPanel.Children.Add(_prevButton);
            paginationPanel.Children.Add(_pageInfo);
            paginationPanel.Children.Add(_nextButton);

            return paginationPanel;
        }

        private void AddDataGridColumns()
        {
            // ID Column
            _reviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "ID",
                Binding = new Binding("Id"),
                Width = 80
            });

            // Student Name Column
            _reviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "NGƯỜI ĐÁNH GIÁ",
                Binding = new Binding("StudentName"),
                Width = 200
            });

            // Course Name Column
            _reviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "KHÓA HỌC",
                Binding = new Binding("CourseName"),
                Width = 250
            });

            // Rating Column
            var ratingColumn = new DataGridTemplateColumn
            {
                Header = "ĐIỂM ĐÁNH GIÁ",
                Width = 150
            };
            
            var ratingTemplate = new DataTemplate();
            var stackPanelFactory = new FrameworkElementFactory(typeof(StackPanel));
            stackPanelFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            
            var starsFactory = new FrameworkElementFactory(typeof(TextBlock));
            starsFactory.SetValue(TextBlock.TextProperty, "⭐⭐⭐⭐⭐");
            starsFactory.SetValue(TextBlock.FontSizeProperty, 14.0);
            starsFactory.SetValue(TextBlock.MarginProperty, new Thickness(0, 0, 5, 0));
            
            var ratingBorderFactory = new FrameworkElementFactory(typeof(Border));
            ratingBorderFactory.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(240, 173, 78)));
            ratingBorderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(12));
            ratingBorderFactory.SetValue(Border.PaddingProperty, new Thickness(6, 2, 6, 2));
            
            var ratingTextFactory = new FrameworkElementFactory(typeof(TextBlock));
            ratingTextFactory.SetBinding(TextBlock.TextProperty, new Binding("Rating"));
            ratingTextFactory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            ratingTextFactory.SetValue(TextBlock.FontSizeProperty, 10.0);
            ratingTextFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            
            ratingBorderFactory.AppendChild(ratingTextFactory);
            stackPanelFactory.AppendChild(starsFactory);
            stackPanelFactory.AppendChild(ratingBorderFactory);
            ratingTemplate.VisualTree = stackPanelFactory;
            ratingColumn.CellTemplate = ratingTemplate;
            
            _reviewDataGrid.Columns.Add(ratingColumn);

            // Status Column
            var statusColumn = new DataGridTemplateColumn
            {
                Header = "TRẠNG THÁI",
                Width = 120
            };
            
            var statusTemplate = new DataTemplate();
            var statusBorderFactory = new FrameworkElementFactory(typeof(Border));
            statusBorderFactory.SetValue(Border.BackgroundProperty, new SolidColorBrush(Color.FromRgb(92, 184, 92)));
            statusBorderFactory.SetValue(Border.CornerRadiusProperty, new CornerRadius(12));
            statusBorderFactory.SetValue(Border.PaddingProperty, new Thickness(8, 4, 8, 4));
            
            var statusTextFactory = new FrameworkElementFactory(typeof(TextBlock));
            statusTextFactory.SetBinding(TextBlock.TextProperty, new Binding("Status"));
            statusTextFactory.SetValue(TextBlock.ForegroundProperty, Brushes.White);
            statusTextFactory.SetValue(TextBlock.FontSizeProperty, 10.0);
            statusTextFactory.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            statusTextFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            
            statusBorderFactory.AppendChild(statusTextFactory);
            statusTemplate.VisualTree = statusBorderFactory;
            statusColumn.CellTemplate = statusTemplate;
            
            _reviewDataGrid.Columns.Add(statusColumn);

            // Review Date Column
            _reviewDataGrid.Columns.Add(new DataGridTextColumn
            {
                Header = "NGÀY ĐÁNH GIÁ",
                Binding = new Binding("ReviewDate"),
                Width = 120
            });

            // Action Column
            var actionColumn = new DataGridTemplateColumn
            {
                Header = "HÀNH ĐỘNG",
                Width = 150
            };
            
            var actionTemplate = new DataTemplate();
            var actionStackFactory = new FrameworkElementFactory(typeof(StackPanel));
            actionStackFactory.SetValue(StackPanel.OrientationProperty, Orientation.Horizontal);
            
            // View Button
            var viewButtonFactory = new FrameworkElementFactory(typeof(Button));
            viewButtonFactory.SetValue(Button.ContentProperty, "👁");
            viewButtonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(74, 144, 226)));
            viewButtonFactory.SetValue(Button.ForegroundProperty, Brushes.White);
            viewButtonFactory.SetValue(Button.WidthProperty, 30.0);
            viewButtonFactory.SetValue(Button.HeightProperty, 25.0);
            viewButtonFactory.SetValue(Button.MarginProperty, new Thickness(2));
            viewButtonFactory.SetValue(Button.BorderThicknessProperty, new Thickness(0));
            viewButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(ViewButton_Click));
            
            // Edit Button
            var editButtonFactory = new FrameworkElementFactory(typeof(Button));
            editButtonFactory.SetValue(Button.ContentProperty, "✏");
            editButtonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(240, 173, 78)));
            editButtonFactory.SetValue(Button.ForegroundProperty, Brushes.White);
            editButtonFactory.SetValue(Button.WidthProperty, 30.0);
            editButtonFactory.SetValue(Button.HeightProperty, 25.0);
            editButtonFactory.SetValue(Button.MarginProperty, new Thickness(2));
            editButtonFactory.SetValue(Button.BorderThicknessProperty, new Thickness(0));
            editButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(EditButton_Click));
            
            // Delete Button
            var deleteButtonFactory = new FrameworkElementFactory(typeof(Button));
            deleteButtonFactory.SetValue(Button.ContentProperty, "🗑");
            deleteButtonFactory.SetValue(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(217, 83, 79)));
            deleteButtonFactory.SetValue(Button.ForegroundProperty, Brushes.White);
            deleteButtonFactory.SetValue(Button.WidthProperty, 30.0);
            deleteButtonFactory.SetValue(Button.HeightProperty, 25.0);
            deleteButtonFactory.SetValue(Button.MarginProperty, new Thickness(2));
            deleteButtonFactory.SetValue(Button.BorderThicknessProperty, new Thickness(0));
            deleteButtonFactory.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteButton_Click));
            
            actionStackFactory.AppendChild(viewButtonFactory);
            actionStackFactory.AppendChild(editButtonFactory);
            actionStackFactory.AppendChild(deleteButtonFactory);
            actionTemplate.VisualTree = actionStackFactory;
            actionColumn.CellTemplate = actionTemplate;
            
            _reviewDataGrid.Columns.Add(actionColumn);
        }

        private void ReviewManagementPage_Loaded(object sender, RoutedEventArgs e)
        {
            // Bind the DataGrid and load data
            try
            {
                if (_reviewDataGrid != null)
                {
                    _reviewDataGrid.ItemsSource = _reviews;
                    LoadReviewsAsync();
                }
                else
                {
                    MessageBox.Show("DataGrid not initialized properly.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error binding data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadReviewsAsync()

        {
            try
            {
                _allReviews = await _reviewService.GetAllReviewsAsync();
                _currentPage = 1;
                UpdatePagination();
                
                // Load filter dropdown data
                await LoadFilterDropdownsAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        
        private async Task LoadFilterDropdownsAsync()
        {
            try
            {
                // Load student names
                var studentNames = await _reviewService.GetDistinctStudentNamesAsync();
                _studentComboBox.Items.Clear();
                _studentComboBox.Items.Add("Tất cả");
                foreach (var name in studentNames)
                {
                    _studentComboBox.Items.Add(name);
                }
                _studentComboBox.SelectedIndex = 0;
                
                // Load course names
                var courseNames = await _reviewService.GetDistinctCourseNamesAsync();
                _courseComboBox.Items.Clear();
                _courseComboBox.Items.Add("Tất cả");
                foreach (var name in courseNames)
                {
                    _courseComboBox.Items.Add(name);
                }
                _courseComboBox.SelectedIndex = 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu bộ lọc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UpdatePagination()
        {
            if (_allReviews == null || _allReviews.Count == 0)
            {
                _reviews.Clear();
                _totalPages = 1;
                _currentPage = 1;
                _pageInfo.Text = "Trang 1 / 1";
                _prevButton.IsEnabled = false;
                _nextButton.IsEnabled = false;
                return;
            }

            _totalPages = (int)Math.Ceiling((double)_allReviews.Count / _itemsPerPage);
            
            // Get items for current page
            var startIndex = (_currentPage - 1) * _itemsPerPage;
            var pageItems = _allReviews.Skip(startIndex).Take(_itemsPerPage).ToList();
            
            _reviews.Clear();
            foreach (var review in pageItems)
            {
                _reviews.Add(review);
            }
            
            // Update UI
            _pageInfo.Text = $"Trang {_currentPage} / {_totalPages}";
            _prevButton.IsEnabled = _currentPage > 1;
            _nextButton.IsEnabled = _currentPage < _totalPages;
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
                string searchTerm = _searchTextBox.Text?.Trim();
                
                // Get selected student filter
                string selectedStudent = _studentComboBox.SelectedItem?.ToString();
                if (selectedStudent == "Tất cả") selectedStudent = null;
                
                // Get selected course filter
                string selectedCourse = _courseComboBox.SelectedItem?.ToString();
                if (selectedCourse == "Tất cả") selectedCourse = null;
                
                // Get rating filters
                int? minRating = null;
                if (_minRatingComboBox.SelectedItem?.ToString() != "Tất cả" && 
                    int.TryParse(_minRatingComboBox.SelectedItem?.ToString(), out int minVal))
                {
                    minRating = minVal;
                }
                
                int? maxRating = null;
                if (_maxRatingComboBox.SelectedItem?.ToString() != "Tất cả" && 
                    int.TryParse(_maxRatingComboBox.SelectedItem?.ToString(), out int maxVal))
                {
                    maxRating = maxVal;
                }
                
                // Get date filters
                DateTime? fromDate = _fromDatePicker.SelectedDate;
                DateTime? toDate = _toDatePicker.SelectedDate;
                
                // Get status filter
                string selectedStatus = _statusComboBox.SelectedItem?.ToString();
                if (selectedStatus == "Tất cả") selectedStatus = null;

                // Build comprehensive search term
                var searchTerms = new List<string>();
                if (!string.IsNullOrEmpty(searchTerm))
                    searchTerms.Add(searchTerm);
                if (!string.IsNullOrEmpty(selectedStudent))
                    searchTerms.Add(selectedStudent);
                if (!string.IsNullOrEmpty(selectedCourse))
                    searchTerms.Add(selectedCourse);
                
                string combinedSearchTerm = searchTerms.Count > 0 ? string.Join(" ", searchTerms) : null;

                var reviews = await _reviewService.SearchReviewsAsync(combinedSearchTerm, minRating, maxRating, fromDate, toDate, selectedStatus);
                
                _allReviews = reviews;
                _currentPage = 1;
                UpdatePagination();
                
                MessageBox.Show($"Tìm thấy {reviews.Count} kết quả phù hợp.", "Kết quả tìm kiếm", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Reset all filter controls
                _searchTextBox.Text = string.Empty;
                _studentComboBox.SelectedIndex = 0;
                _courseComboBox.SelectedIndex = 0;
                _minRatingComboBox.SelectedIndex = 0;
                _maxRatingComboBox.SelectedIndex = 0;
                _statusComboBox.SelectedIndex = 0;
                _fromDatePicker.SelectedDate = null;
                _toDatePicker.SelectedDate = null;
                
                // Reload all reviews
                await LoadReviewsAsync();
                
                MessageBox.Show("Bộ lọc đã được đặt lại.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đặt lại bộ lọc: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ReviewViewModel review)
            {
                try
                {
                    var detailsDialog = new ReviewDetailsDialog(review);
                    detailsDialog.Owner = Window.GetWindow(this);
                    detailsDialog.ShowDialog();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi xem chi tiết đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ReviewViewModel review)
            {
                try
                {
                    var editDialog = new EditReviewDialog(review);
                    editDialog.Owner = Window.GetWindow(this);
                    
                    if (editDialog.ShowDialog() == true)
                    {
                        // Refresh the data after successful edit
                        await LoadReviewsAsync();
                        MessageBox.Show("Cập nhật đánh giá thành công!", "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Có lỗi xảy ra khi chỉnh sửa đánh giá: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.DataContext is ReviewViewModel review)
            {
                var result = MessageBox.Show($"Bạn có chắc chắn muốn xóa đánh giá của {review.StudentName}?", "Xác nhận", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    // Implement delete functionality
                    MessageBox.Show("Tính năng xóa sẽ được triển khai sau.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ReviewManagementPage_Unloaded(object sender, RoutedEventArgs e)
        {
            _reviewService?.Dispose();
        }
    }
}
