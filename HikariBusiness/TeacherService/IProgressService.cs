using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public interface IProgressService
    {
        public void AddProgress(Progress progress);
        public void UpdateProgress(Progress progress);
        public List<Progress> GetProgressByStudent(string studentId);
        public List<Progress> GetAllProgresses();

    }
}
