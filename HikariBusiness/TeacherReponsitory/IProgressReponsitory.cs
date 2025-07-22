using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public interface IProgressReponsitory
    {
        public void AddProgress(Progress progress);
        public void UpdateProgress(Progress progress);
        public List<Progress> GetProgressByStudent(String studentId);
        public List<Progress> GetAllProgresses();
    }
}
