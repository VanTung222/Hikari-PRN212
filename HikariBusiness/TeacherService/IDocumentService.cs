using DataAccessLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public interface IDocumentService
    {
        public void AddDocument(Document document);
        public void UpdateDocument(Document document);
        public void DeleteDocument(int documentId);
        public Document GetDocumentById(int documentId);
        public List<Document> GetAllDocuments();
    }
}
