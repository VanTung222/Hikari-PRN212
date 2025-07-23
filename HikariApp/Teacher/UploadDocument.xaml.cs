using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;

namespace HikariApp.Teacher
{
    public partial class UploadDocument : Window
    {
        private readonly DocumentService _documentService;
        private string _selectedFilePath;

        public bool IsSuccess { get; private set; }

        public UploadDocument()
        {
            InitializeComponent();
            _documentService = new DocumentService();
        }

        private void SelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn file tài liệu",
                Filter = "PDF files (*.pdf)|*.pdf|Word files (*.doc;*.docx)|*.doc;*.docx|Excel files (*.xls;*.xlsx)|*.xls;*.xlsx|PowerPoint files (*.ppt;*.pptx)|*.ppt;*.pptx|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                txtFilePath.Text = Path.GetFileName(_selectedFilePath);
            }
        }

        private void Upload_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                // Create Documents folder if not exists
                string documentsFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Documents");
                if (!Directory.Exists(documentsFolder))
                    Directory.CreateDirectory(documentsFolder);

                // Copy file to application folder
                string fileName = $"{Guid.NewGuid()}_{Path.GetFileName(_selectedFilePath)}";
                string destinationPath = Path.Combine(documentsFolder, fileName);
                File.Copy(_selectedFilePath, destinationPath, true);

                // Create document record
                Document document = new Document
                {
                    Title = txtTitle.Text,
                    Description = txtDescription.Text,
                    FileUrl = $"Documents/{fileName}",
                    UploadDate = DateTime.Now,
                    UploadedBy = txtUploadedBy.Text
                };

                _documentService.AddDocument(document);
                IsSuccess = true;
                MessageBox.Show("Tải tài liệu thành công!", "Thành công",
                               MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải tài liệu: {ex.Message}", "Lỗi",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên tài liệu!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return false;
            }

            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                MessageBox.Show("Vui lòng chọn file tài liệu!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtUploadedBy.Text))
            {
                MessageBox.Show("Vui lòng nhập người tải lên!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtUploadedBy.Focus();
                return false;
            }

            return true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
