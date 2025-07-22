using HikariApp.Models;
using HikariApp.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HikariApp.Views
{
    public partial class DictionaryView : Window
    {
        private readonly JishoService _jishoService;

        public DictionaryView()
        {
            InitializeComponent();
            _jishoService = new JishoService();
            
            // Thiết lập focus vào ô tìm kiếm khi khởi động
            Loaded += (s, e) => txtSearch.Focus();
            
            // Tải danh sách từ đã tìm kiếm gần đây
            LoadRecentSearches();
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                SearchWord();
            }
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchWord();
        }

        private async void SearchWord()
        {
            string searchText = txtSearch.Text?.Trim() ?? string.Empty;
            
            if (string.IsNullOrWhiteSpace(searchText))
                return;
                
            // Hiển thị loading và ẩn các panel khác
            loadingPanel.Visibility = Visibility.Visible;
            noResultsPanel.Visibility = Visibility.Collapsed;
            resultsPanel.Children.Clear();
            recentSearchesPanel.Visibility = Visibility.Collapsed;
            
            try
            {
                // Tìm kiếm từ
                var result = await _jishoService.SearchWordAsync(searchText);
                
                // Ẩn loading
                loadingPanel.Visibility = Visibility.Collapsed;
                
                if (result == null || result.Data == null || result.Data.Count == 0)
                {
                    // Không tìm thấy kết quả
                    noResultsPanel.Visibility = Visibility.Visible;
                    return;
                }
                
                // Hiển thị kết quả
                DisplayResults(result.Data);
                
                // Cập nhật danh sách từ đã tìm gần đây
                LoadRecentSearches();
            }
            catch (Exception ex)
            {
                // Ẩn loading và hiển thị lỗi
                loadingPanel.Visibility = Visibility.Collapsed;
                
                // Hiển thị thông báo lỗi
                var errorBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(255, 240, 240)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(255, 200, 200)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(8),
                    Padding = new Thickness(15),
                    Margin = new Thickness(0, 0, 0, 15)
                };
                
                var errorPanel = new StackPanel();
                errorBorder.Child = errorPanel;
                
                var errorTitle = new TextBlock
                {
                    Text = "Lỗi khi tìm kiếm",
                    FontSize = 16,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(200, 0, 0)),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                
                var errorMessage = new TextBlock
                {
                    Text = $"Không thể tìm kiếm từ: {ex.Message}",
                    TextWrapping = TextWrapping.Wrap
                };
                
                errorPanel.Children.Add(errorTitle);
                errorPanel.Children.Add(errorMessage);
                
                resultsPanel.Children.Add(errorBorder);
                
                System.Diagnostics.Debug.WriteLine($"Search error: {ex.Message}");
            }
        }
        
        private void DisplayResults(List<JishoData> data)
        {
            foreach (var item in data)
            {
                // Tạo một border cho mỗi kết quả
                var resultBorder = new Border
                {
                    Style = (Style)FindResource("WordItemStyle")
                };
                
                var resultPanel = new StackPanel();
                resultBorder.Child = resultPanel;
                
                // Tiêu đề - Từ tiếng Nhật và cách đọc
                if (item.Japanese != null && item.Japanese.Count > 0)
                {
                    var japaneseWord = item.Japanese[0];
                    string? wordToDisplay = japaneseWord.Word;
                    string? readingToDisplay = japaneseWord.Reading;
                    
                    // Tạo panel cho tiêu đề và nút phát âm
                    var headerPanel = new Grid();
                    headerPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                    headerPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                    headerPanel.Margin = new Thickness(0, 0, 0, 15);
                    
                    // Panel bên trái chứa từ và cách đọc
                    var titlePanel = new StackPanel
                    {
                        Orientation = Orientation.Vertical,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    
                    // Từ tiếng Nhật
                    if (!string.IsNullOrEmpty(wordToDisplay))
                    {
                        var wordText = new TextBlock
                        {
                            Text = wordToDisplay,
                            FontSize = 28,
                            FontWeight = FontWeights.Bold,
                            Margin = new Thickness(0, 0, 0, 5)
                        };
                        titlePanel.Children.Add(wordText);
                    }
                    
                    // Cách đọc
                    if (!string.IsNullOrEmpty(readingToDisplay))
                    {
                        var readingText = new TextBlock
                        {
                            Text = $"[{readingToDisplay}]",
                            FontSize = 18,
                            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102))
                        };
                        titlePanel.Children.Add(readingText);
                    }
                    
                    Grid.SetColumn(titlePanel, 0);
                    headerPanel.Children.Add(titlePanel);
                    
                    // Nút phát âm lớn
                    if (!string.IsNullOrEmpty(readingToDisplay) || !string.IsNullOrEmpty(wordToDisplay))
                    {
                        string audioWord = !string.IsNullOrEmpty(readingToDisplay) ? readingToDisplay : wordToDisplay ?? string.Empty;
                        var audioUrl = $"https://translate.google.com/translate_tts?ie=UTF-8&q={Uri.EscapeDataString(audioWord)}&tl=ja&client=tw-ob";
                        
                        // Tạo panel chứa nút phát âm và nhãn
                        var audioPanel = new StackPanel
                        {
                            Orientation = Orientation.Vertical,
                            HorizontalAlignment = HorizontalAlignment.Right,
                            VerticalAlignment = VerticalAlignment.Center,
                            Margin = new Thickness(0, 0, 0, 0)
                        };
                        
                        // Nút phát âm lớn hơn
                        var soundButton = new Button
                        {
                            Width = 60,
                            Height = 60,
                            Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Xanh lá
                            BorderThickness = new Thickness(0),
                            Margin = new Thickness(0, 0, 0, 5),
                            Tag = audioUrl,
                            ToolTip = "Nghe phát âm",
                            Cursor = Cursors.Hand
                        };
                        
                        // Tạo template cho nút tròn
                        var template = new ControlTemplate(typeof(Button));
                        var templateBorder = new FrameworkElementFactory(typeof(Border));
                        templateBorder.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                        templateBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(30));
                        
                        var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
                        contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                        contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                        
                        templateBorder.AppendChild(contentPresenter);
                        template.VisualTree = templateBorder;
                        
                        // Thêm trigger cho hover
                        var trigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
                        trigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(69, 160, 73))));
                        template.Triggers.Add(trigger);
                        
                        soundButton.Template = template;
                        
                        // Icon loa phát âm
                        soundButton.Content = new TextBlock
                        {
                            Text = "🔊",
                            FontSize = 24,
                            Foreground = Brushes.White,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center
                        };
                        
                        soundButton.Click += SoundButton_Click;
                        
                        // Nhãn "Nghe phát âm"
                        var audioLabel = new TextBlock
                        {
                            Text = "Nghe phát âm",
                            FontSize = 12,
                            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                            HorizontalAlignment = HorizontalAlignment.Center
                        };
                        
                        audioPanel.Children.Add(soundButton);
                        audioPanel.Children.Add(audioLabel);
                        
                        Grid.SetColumn(audioPanel, 1);
                        headerPanel.Children.Add(audioPanel);
                    }
                    
                    resultPanel.Children.Add(headerPanel);
                    
                    // Hiển thị các định dạng khác nếu có
                    if (item.Japanese.Count > 1)
                    {
                        var otherForms = new TextBlock
                        {
                            Text = "Các dạng khác: " + string.Join(", ", item.Japanese.GetRange(1, item.Japanese.Count - 1).ConvertAll(j => $"{j.Word ?? ""} [{j.Reading ?? ""}]")),
                            Foreground = new SolidColorBrush(Color.FromRgb(102, 102, 102)),
                            Margin = new Thickness(0, 0, 0, 10),
                            TextWrapping = TextWrapping.Wrap
                        };
                        resultPanel.Children.Add(otherForms);
                    }
                }
                
                // Hiển thị cấp độ JLPT nếu có
                if (item.Jlpt != null && item.Jlpt.Count > 0)
                {
                    var jlptText = new TextBlock
                    {
                        Text = $"JLPT: {string.Join(", ", item.Jlpt)}",
                        Foreground = new SolidColorBrush(Color.FromRgb(0, 102, 204)),
                        FontWeight = FontWeights.SemiBold,
                        Margin = new Thickness(0, 0, 0, 10)
                    };
                    resultPanel.Children.Add(jlptText);
                }
                
                // Hiển thị nghĩa
                if (item.Senses != null && item.Senses.Count > 0)
                {
                    var meaningsPanel = new StackPanel
                    {
                        Margin = new Thickness(0, 10, 0, 0)
                    };
                    
                    var meaningsTitle = new TextBlock
                    {
                        Text = "Nghĩa:",
                        FontWeight = FontWeights.SemiBold,
                        Margin = new Thickness(0, 0, 0, 5)
                    };
                    meaningsPanel.Children.Add(meaningsTitle);
                    
                    for (int i = 0; i < item.Senses.Count; i++)
                    {
                        var sense = item.Senses[i];
                        
                        // Loại từ (danh từ, động từ, ...)
                        if (sense.PartsOfSpeech != null && sense.PartsOfSpeech.Count > 0)
                        {
                            var partsOfSpeech = new TextBlock
                            {
                                Text = string.Join(", ", sense.PartsOfSpeech),
                                FontStyle = FontStyles.Italic,
                                Foreground = new SolidColorBrush(Color.FromRgb(153, 153, 153))
                            };
                            meaningsPanel.Children.Add(partsOfSpeech);
                        }
                        
                        // Nghĩa của từ
                        if (sense.EnglishDefinitions != null && sense.EnglishDefinitions.Count > 0)
                        {
                            var definitionsPanel = new StackPanel
                            {
                                Margin = new Thickness(15, 0, 0, 10)
                            };
                            
                            // Hiển thị mỗi nghĩa trên một dòng riêng với định dạng tốt hơn
                            foreach (var definition in sense.EnglishDefinitions)
                            {
                                // Kiểm tra xem nghĩa có phải là nghĩa tiếng Việt không
                                bool isVietnameseDefinition = definition.StartsWith("Tiếng Việt:");
                                
                                var definitionText = new TextBlock
                                {
                                    Text = $"{(isVietnameseDefinition ? "" : "• ")}{definition}",
                                    TextWrapping = TextWrapping.Wrap,
                                    Margin = new Thickness(0, 0, 0, 5),
                                    FontWeight = isVietnameseDefinition ? FontWeights.SemiBold : FontWeights.Normal,
                                    Foreground = isVietnameseDefinition ? 
                                        new SolidColorBrush(Color.FromRgb(0, 102, 204)) : 
                                        new SolidColorBrush(Color.FromRgb(0, 0, 0))
                                };
                                
                                definitionsPanel.Children.Add(definitionText);
                            }
                            
                            meaningsPanel.Children.Add(definitionsPanel);
                        }
                    }
                    
                    resultPanel.Children.Add(meaningsPanel);
                }
                
                resultsPanel.Children.Add(resultBorder);
            }
        }
        
        private async void SoundButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender is Button button && button.Tag is string audioUrl)
                {
                    // Đổi biểu tượng nút thành biểu tượng loading
                    var originalContent = button.Content;
                    button.Content = new TextBlock
                    {
                        Text = "⏳",
                        FontSize = button.Width >= 50 ? 24 : 16, // Điều chỉnh kích thước theo nút
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    button.IsEnabled = false;
                    
                    // Phát âm thanh
                    await _jishoService.PlayAudioAsync(audioUrl);
                    
                    // Khôi phục biểu tượng nút
                    button.Content = originalContent;
                    button.IsEnabled = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi phát âm thanh: {ex.Message}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                System.Diagnostics.Debug.WriteLine($"Audio error: {ex.Message}");
                
                // Khôi phục nút về trạng thái ban đầu trong trường hợp lỗi
                if (sender is Button button)
                {
                    button.IsEnabled = true;
                    button.Content = new TextBlock
                    {
                        Text = "🔊",
                        FontSize = button.Width >= 50 ? 24 : 16,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                }
            }
        }
        
        private void LoadRecentSearches()
        {
            var recentSearches = _jishoService.GetRecentSearches();
            
            recentWordsPanel.Children.Clear();
            
            if (recentSearches.Count == 0)
            {
                recentSearchesPanel.Visibility = Visibility.Collapsed;
                return;
            }
            
            recentSearchesPanel.Visibility = Visibility.Visible;
            
            foreach (var search in recentSearches)
            {
                // Tạo panel cho từ và nút phát âm
                var itemPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(0, 0, 10, 10)
                };
                
                // Border chứa từ
                var border = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(240, 240, 240)),
                    Padding = new Thickness(10, 5, 10, 5),
                    CornerRadius = new CornerRadius(20),
                    Cursor = Cursors.Hand,
                    Tag = search.Word
                };
                
                border.MouseLeftButtonDown += RecentWord_Click;
                
                var text = new TextBlock
                {
                    Text = search.Word ?? string.Empty,
                    FontSize = 14
                };
                
                border.Child = text;
                itemPanel.Children.Add(border);
                
                // Thêm nút phát âm nhỏ nếu có audio URL
                if (!string.IsNullOrEmpty(search.AudioUrl))
                {
                    var miniSoundButton = new Button
                    {
                        Width = 30,
                        Height = 30,
                        Background = new SolidColorBrush(Color.FromRgb(76, 175, 80)),
                        BorderThickness = new Thickness(0),
                        Margin = new Thickness(5, 0, 0, 0),
                        Tag = search.AudioUrl,
                        ToolTip = "Nghe phát âm",
                        Cursor = Cursors.Hand
                    };
                    
                    // Tạo template cho nút tròn
                    var template = new ControlTemplate(typeof(Button));
                    var templateBorder = new FrameworkElementFactory(typeof(Border));
                    templateBorder.SetValue(Border.BackgroundProperty, new TemplateBindingExtension(Button.BackgroundProperty));
                    templateBorder.SetValue(Border.CornerRadiusProperty, new CornerRadius(15));
                    
                    var contentPresenter = new FrameworkElementFactory(typeof(ContentPresenter));
                    contentPresenter.SetValue(ContentPresenter.HorizontalAlignmentProperty, HorizontalAlignment.Center);
                    contentPresenter.SetValue(ContentPresenter.VerticalAlignmentProperty, VerticalAlignment.Center);
                    
                    templateBorder.AppendChild(contentPresenter);
                    template.VisualTree = templateBorder;
                    
                    // Thêm trigger cho hover
                    var trigger = new Trigger { Property = UIElement.IsMouseOverProperty, Value = true };
                    trigger.Setters.Add(new Setter(Button.BackgroundProperty, new SolidColorBrush(Color.FromRgb(69, 160, 73))));
                    template.Triggers.Add(trigger);
                    
                    miniSoundButton.Template = template;
                    
                    // Icon loa phát âm
                    miniSoundButton.Content = new TextBlock
                    {
                        Text = "🔊",
                        FontSize = 12,
                        Foreground = Brushes.White,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center
                    };
                    
                    miniSoundButton.Click += SoundButton_Click;
                    itemPanel.Children.Add(miniSoundButton);
                }
                
                recentWordsPanel.Children.Add(itemPanel);
            }
        }
        
        private void RecentWord_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.Tag is string word)
            {
                txtSearch.Text = word;
                SearchWord();
            }
        }
    }
} 