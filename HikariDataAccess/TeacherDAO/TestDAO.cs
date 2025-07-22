using DataAccessLayer;
using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HikariDataAccess.TeacherDAO
{
    public class TestDAO
    {
        // Lấy tất cả bài kiểm tra
        public List<Test> GetAllTests()
        {
            using (var context = new HikariContext())
            {
                return context.Tests.ToList();
            }
        }

        // Thêm bài kiểm tra
        public void AddTest(Test test)
        {
            using (var context = new HikariContext())
            {
                context.Tests.Add(test);
                context.SaveChanges();
            }
        }

        // Cập nhật bài kiểm tra
        public void UpdateTest(Test test)
        {
            using (var context = new HikariContext())
            {
                var existingTest = context.Tests.FirstOrDefault(t => t.Id == test.Id);
                if (existingTest != null)
                {
                    existingTest.Title = test.Title;
                    existingTest.Description = test.Description;
                    existingTest.JlptLevel = test.JlptLevel;
                    existingTest.TotalMarks = test.TotalMarks;
                    existingTest.TotalQuestions = test.TotalQuestions;
                    existingTest.IsActive = test.IsActive;

                    context.SaveChanges();
                }
            }
        }

        // Xóa bài kiểm tra
        public void DeleteTest(int testId)
        {
            using (var context = new HikariContext())
            {
                var test = context.Tests.FirstOrDefault(t => t.Id == testId);
                if (test != null)
                {
                    context.Tests.Remove(test);
                    context.SaveChanges();
                }
            }
        }

        // Lấy bài kiểm tra theo ID
        public Test GetTestById(int testId)
        {
            using (var context = new HikariContext())
            {
                return context.Tests.FirstOrDefault(t => t.Id == testId);
            }
        }
    }
}
