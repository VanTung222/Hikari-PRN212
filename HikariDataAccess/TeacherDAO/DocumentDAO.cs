using DataAccessLayer;
using DataAccessLayer.Entities;
using HikariDataAccess;

public class DocumentDAO
{
    private List<Document> documents = new List<Document>();

    public DocumentDAO()
    {
        using (var context = new HikariContext())
        {
            documents = context.Documents.ToList(); // lấy từ DB
        }
    }

    public void AddDocument(Document document)
    {
        using (var context = new HikariContext())
        {
            context.Documents.Add(document);
            context.SaveChanges();
        }
        documents.Add(document); // cập nhật RAM
    }

    public List<Document> GetDocuments() => documents;

    public void UpdateDocument(Document document)
    {
        var existingDocument = documents.FirstOrDefault(d => d.Id == document.Id);
        if (existingDocument != null)
        {
            using (var context = new HikariContext())
            {
                context.Documents.Update(document);
                context.SaveChanges();
            }

            // Cập nhật bản sao RAM
            existingDocument.Title = document.Title;
            existingDocument.Description = document.Description;
            existingDocument.FileUrl = document.FileUrl;
            existingDocument.UploadDate = document.UploadDate;
            existingDocument.UploadedBy = document.UploadedBy;
            existingDocument.UploadedByNavigation = document.UploadedByNavigation;
        }
    }

    public void DeleteDocument(int documentId)
    {
        var document = documents.FirstOrDefault(d => d.Id == documentId);
        if (document != null)
        {
            using (var context = new HikariContext())
            {
                context.Documents.Remove(document);
                context.SaveChanges();
            }
            documents.Remove(document);
        }
    }

    public Document GetDocumentById(int documentId) =>
        documents.FirstOrDefault(d => d.Id == documentId);
}
