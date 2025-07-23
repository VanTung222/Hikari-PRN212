using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System.Collections.Generic;

namespace HikariBusiness.TeacherService
{
    public class QuestionService
    {
        private readonly QuestionReponsitory _questionRepository;
        public QuestionService()
        {
            _questionRepository = new QuestionReponsitory();
        }
        public List<Question> GetQuestionsByTestId(int testId)
        {
            return _questionRepository.GetQuestionsByTestId(testId);
        }
    }
} 