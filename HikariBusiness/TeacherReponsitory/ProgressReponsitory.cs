using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public class ProgressReponsitory : IProgressReponsitory
    {
        private ProgressDAO dao = new ProgressDAO();
        public void AddProgress(Progress progress) => dao.AddProgress(progress);
        public List<Progress> GetAllProgresses() => dao.GetAllProgress();

        public List<Progress> GetProgressByStudent(string studentId) => dao.GetProgressByStudentId(studentId);

        public void UpdateProgress(Progress progress) => dao.UpdateProgress(progress);
    }
}
