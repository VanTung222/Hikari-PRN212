using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using DataAccessLayer.Entities;
using HikariBusiness.TeacherService;
using OfficeOpenXml;
using DataAccessLayer;
using System.Collections.Generic; // Added for Dictionary
using System.Linq; // Added for FirstOrDefault

namespace HikariApp.Teacher
{
    public partial class CreateTest : Window
    {
        private Test _test;
        private bool _isEditMode;
        private readonly TestService _testService;
        private string _selectedExcelPath;

        public Test Test => _test;
        public bool IsSuccess { get; private set; }

        // Đặt license context cho EPPlus ở đầu chương trình
        static CreateTest()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public CreateTest(Test test = null)
        {
            InitializeComponent();
            _testService = new TestService();
            _isEditMode = test != null;
            _test = test ?? new Test();

            LoadData();
        }

        private void LoadData()
        {
            if (_isEditMode)
            {
                WindowTitle.Text = "✏️ CHỈNH SỬA TEST";
                txtTitle.Text = _test.Title;
                txtDescription.Text = _test.Description;
                cmbJlptLevel.Text = _test.JlptLevel;
                txtTotalMarks.Text = _test.TotalMarks.ToString();
                chkIsActive.IsChecked = _test.IsActive;
            }
            else
            {
                WindowTitle.Text = "📝 TẠO TEST MỚI";
                cmbJlptLevel.SelectedIndex = 0;
                chkIsActive.IsChecked = true;
            }
        }

        private void SelectExcel_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Chọn file Excel chứa câu hỏi",
                Filter = "Excel files (*.xlsx;*.xls)|*.xlsx;*.xls|All files (*.*)|*.*",
                FilterIndex = 1
            };

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedExcelPath = openFileDialog.FileName;
                txtExcelPath.Text = Path.GetFileName(_selectedExcelPath);
                txtExcelPath.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!ValidateInput())
                    return;

                _test.Title = txtTitle.Text;
                _test.Description = txtDescription.Text;
                _test.JlptLevel = cmbJlptLevel.Text;
                _test.TotalMarks = decimal.Parse(txtTotalMarks.Text);
                _test.IsActive = chkIsActive.IsChecked.Value;

                if (_isEditMode)
                {
                    _testService.UpdateTest(_test);
                }
                else
                {
                    _testService.AddTest(_test);
                }

                // Process Excel file if selected
                if (!string.IsNullOrEmpty(_selectedExcelPath))
                {
                    ProcessExcelFile(_selectedExcelPath, _test.Id);
                }

                IsSuccess = true;
                MessageBox.Show(_isEditMode ? "Cập nhật test thành công!" : "Tạo test thành công!",
                              "Thành công", MessageBoxButton.OK, MessageBoxImage.Information);
                DialogResult = true;
                Close();
            }
            catch (Exception ex)
            {
                string inner = ex.InnerException?.Message ?? "";
                MessageBox.Show($"Lỗi: {ex.Message}\n{inner}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProcessExcelFile(string filePath, int testId)
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;
                int colCount = worksheet.Dimension.Columns;

                // Tìm vị trí các cột theo header
                var header = new Dictionary<string, int>();
                for (int col = 1; col <= colCount; col++)
                {
                    var colName = worksheet.Cells[1, col].Text.Trim().ToLower();
                    header[colName] = col;
                }

                // Chấp nhận nhiều biến thể tên cột
                string[] questionKeys = { "questiontext", "questiont" };
                string[] correctKeys = { "correctoption", "correctopt" };
                string qKey = questionKeys.FirstOrDefault(k => header.ContainsKey(k));
                string cKey = correctKeys.FirstOrDefault(k => header.ContainsKey(k));
                if (qKey == null)
                    throw new Exception("Không tìm thấy cột questionText hoặc questionT trong file Excel!");
                if (cKey == null)
                    throw new Exception("Không tìm thấy cột correctOption hoặc correctOpt trong file Excel!");

                using (var context = new HikariContext())
                {
                    for (int row = 2; row <= rowCount; row++)
                    {
                        var question = new Question
                        {
                            QuestionText = worksheet.Cells[row, header[qKey]].Text,
                            OptionA = worksheet.Cells[row, header["optiona"]].Text,
                            OptionB = worksheet.Cells[row, header["optionb"]].Text,
                            OptionC = worksheet.Cells[row, header["optionc"]].Text,
                            OptionD = worksheet.Cells[row, header["optiond"]].Text,
                            CorrectOption = worksheet.Cells[row, header[cKey]].Text,
                            Mark = decimal.TryParse(worksheet.Cells[row, header["mark"]].Text, out var m) ? m : 1,
                            EntityType = "test",
                            EntityId = testId
                        };
                        context.Questions.Add(question);
                    }
                    context.SaveChanges();
                }
            }
        }

        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(txtTitle.Text))
            {
                MessageBox.Show("Vui lòng nhập tên test!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTitle.Focus();
                return false;
            }

            if (string.IsNullOrWhiteSpace(cmbJlptLevel.Text))
            {
                MessageBox.Show("Vui lòng chọn cấp độ JLPT!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbJlptLevel.Focus();
                return false;
            }

            if (!decimal.TryParse(txtTotalMarks.Text, out decimal marks) || marks <= 0)
            {
                MessageBox.Show("Vui lòng nhập tổng điểm hợp lệ!", "Thông báo",
                               MessageBoxButton.OK, MessageBoxImage.Warning);
                txtTotalMarks.Focus();
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
