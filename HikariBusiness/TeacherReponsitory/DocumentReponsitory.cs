using DataAccessLayer.Entities;
using HikariDataAccess.TeacherDAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherReponsitory
{
    public class DocumentReponsitory : IDocumentReponsitory
    {
        private DocumentDAO dao = new DocumentDAO();

        public void AddDocument(Document document) => dao.AddDocument(document);

        public void DeleteDocument(int documentId) => dao.DeleteDocument(documentId);

        public List<Document> GetAllDocuments() => dao.GetDocuments();

        public Document GetDocumentById(int documentId) => dao.GetDocumentById(documentId);

        public void UpdateDocument(Document document) => dao.UpdateDocument(document);
    }
}
