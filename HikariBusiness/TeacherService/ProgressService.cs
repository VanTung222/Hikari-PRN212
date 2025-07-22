using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public class ProgressService : IProgressService
    {
        private readonly IProgressReponsitory _progressRepository;

        public ProgressService()
        {
            _progressRepository = new ProgressReponsitory();
        }
        public void AddProgress(Progress progress) => _progressRepository.AddProgress(progress);

        public List<Progress> GetAllProgresses() => _progressRepository.GetAllProgresses();

        public List<Progress> GetProgressByStudent(string studentId) => _progressRepository.GetProgressByStudent(studentId);

        public void UpdateProgress(Progress progress) => _progressRepository.UpdateProgress(progress);
    }
}
