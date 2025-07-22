using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class ManageDocument : UserControl
    {
        public ObservableCollection<Document> Documents { get; set; }
        private readonly DocumentService _documentService;

        public ManageDocument()
        {
            InitializeComponent();
            _documentService = new DocumentService();
            Documents = new ObservableCollection<Document>();
            DocumentList.ItemsSource = Documents;
            LoadDocuments();
        }

        private void LoadDocuments()
        {
            try
            {
                Documents.Clear();
                var documents = _documentService.GetAllDocuments();
                foreach (var document in documents)
                {
                    Documents.Add(document);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải danh sách tài liệu: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            var uploadWindow = new UploadDocument();
            uploadWindow.WindowState = WindowState.Maximized;
            if (uploadWindow.ShowDialog() == true && uploadWindow.IsSuccess)
            {
                LoadDocuments(); // Refresh the document list
            }
        }

        private void ViewDocument_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Document document = button?.DataContext as Document;

            if (document != null)
            {
                try
                {
                    string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, document.FileUrl);
                    if (File.Exists(filePath))
                    {
                        Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                    }
                    else
                    {
                        MessageBox.Show("Không tìm thấy tệp!", "Lỗi",
                                       MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi mở tài liệu: {ex.Message}", "Lỗi",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void DownloadDocument_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Document document = button?.DataContext as Document;

            if (document != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    FileName = document.Title,
                    Filter = "All files (*.*)|*.*"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    try
                    {
                        string sourcePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, document.FileUrl);
                        File.Copy(sourcePath, saveFileDialog.FileName, true);
                        MessageBox.Show("Tải xuống thành công!", "Thành công",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi khi tải xuống: {ex.Message}", "Lỗi",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            Document document = button?.DataContext as Document;

            if (document != null)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Bạn có chắc chắn muốn xóa tài liệu '{document.Title}'?",
                    "Xác nhận xóa",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        _documentService.DeleteDocument(document.Id);

                        // Delete physical file
                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, document.FileUrl);
                        if (File.Exists(filePath))
                        {
                            File.Delete(filePath);
                        }

                        MessageBox.Show("Xóa tài liệu thành công!", "Thành công",
                                       MessageBoxButton.OK, MessageBoxImage.Information);
                        LoadDocuments();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi",
                                       MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            LoadDocuments();
        }
    }
}