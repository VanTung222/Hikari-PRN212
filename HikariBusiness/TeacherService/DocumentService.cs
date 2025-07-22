using DataAccessLayer.Entities;
using HikariBusiness.TeacherReponsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HikariBusiness.TeacherService
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentReponsitory _documentRepository;

        public DocumentService()
        {
            _documentRepository = new DocumentReponsitory(); 
        }
        public void AddDocument(Document document) => _documentRepository.AddDocument(document);
        public void DeleteDocument(int documentId) => _documentRepository.DeleteDocument(documentId);

        public List<Document> GetAllDocuments() => _documentRepository.GetAllDocuments();

        public Document GetDocumentById(int documentId) => _documentRepository.GetDocumentById(documentId);



        public void UpdateDocument(Document document) => _documentRepository.UpdateDocument(document);
    }
}
