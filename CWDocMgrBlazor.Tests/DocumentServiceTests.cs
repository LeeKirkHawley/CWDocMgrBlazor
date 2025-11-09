using CWDocMgrBlazor.Data;
using CWDocMgrBlazor.Models;
using CWDocMgrBlazor.Services;
using DocMgrLib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SharedLib.DTOs;

namespace CWDocMgrBlazor.Tests
{
    public class DocumentServiceTests
    {
        [Test]
        public async Task GetAllDocuments_WithDocumentsInDatabase_ReturnsAllDocuments()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documents = new List<DocumentModel>
                {
                    new DocumentModel { Id = 1, UserId = "user1", DocumentName = "doc1-guid.pdf", OriginalDocumentName = "doc1.pdf", DocumentDate = DateTime.Now },
                    new DocumentModel { Id = 2, UserId = "user2", DocumentName = "doc2-guid.pdf", OriginalDocumentName = "doc2.pdf", DocumentDate = DateTime.Now },
                    new DocumentModel { Id = 3, UserId = "user3", DocumentName = "doc3-guid.pdf", OriginalDocumentName = "doc3.pdf", DocumentDate = DateTime.Now }
                };

            context.Documents.AddRange(documents);
            await context.SaveChangesAsync();

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Act
            var result = await documentService.GetAllDocuments();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result[0].OriginalDocumentName, Is.EqualTo("doc1.pdf"));
            Assert.That(result[1].OriginalDocumentName, Is.EqualTo("doc2.pdf"));
            Assert.That(result[2].OriginalDocumentName, Is.EqualTo("doc3.pdf"));

            // Verify logging
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieving all documents from the database.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Got 3 documents.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetAllDocuments_WithEmptyDatabase_ReturnsEmptyList()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Act
            var result = await documentService.GetAllDocuments();

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(0));

            // Verify logging
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Got 0 documents.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetDocumentById_WithValidId_ReturnsDocument()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var document = new DocumentModel
            {
                Id = 1,
                UserId = "test-user",
                DocumentName = "test-doc-guid.pdf",
                OriginalDocumentName = "test-doc.pdf",
                DocumentDate = DateTime.Now
            };

            context.Documents.Add(document);
            await context.SaveChangesAsync();

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Act
            var result = await documentService.GetDocumentById(1);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.That(result.OriginalDocumentName, Is.EqualTo("test-doc.pdf"));

            // Verify logging
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieving document with ID 1 from the database.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetDocumentById_WithInvalidId_ReturnsNull()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Act
            var result = await documentService.GetDocumentById(999);

            // Assert
            Assert.That(result, Is.Null);

            // Verify logging
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Debug,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Retrieving document with ID 999 from the database.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);

            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Warning,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Document with ID 999 not found.")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public async Task GetDocumentFileContent_WithNonExistentFile_ReturnsNull()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Act
            var result = await documentService.GetDocumentFileContent("non-existent-file.pdf");

            // Assert
            Assert.That(result, Is.Null);
        }

        [Test]
        public async Task GetDocumentFileContent_WithExistingPdfFile_ReturnsBase64Content()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create a temporary test file
            var tempFile = Path.GetTempFileName();
            var pdfFile = Path.ChangeExtension(tempFile, ".pdf");
            var testContent = "Test PDF content"u8.ToArray();
            await File.WriteAllBytesAsync(pdfFile, testContent);

            try
            {
                // Act
                var result = await documentService.GetDocumentFileContent(pdfFile);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("data:application/pdf;base64,"));

                var base64Part = result.Substring("data:application/pdf;base64,".Length);
                var decodedBytes = Convert.FromBase64String(base64Part);
                Assert.That(decodedBytes, Is.EqualTo(testContent));
            }
            finally
            {
                // Cleanup
                if (File.Exists(pdfFile))
                    File.Delete(pdfFile);
            }
        }

        [Test]
        public async Task GetDocumentFileContent_WithExistingJpegFile_ReturnsBase64Content()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create a temporary test file
            var tempFile = Path.GetTempFileName();
            var jpegFile = Path.ChangeExtension(tempFile, ".jpg");
            var testContent = "Test JPEG content"u8.ToArray();
            await File.WriteAllBytesAsync(jpegFile, testContent);

            try
            {
                // Act
                var result = await documentService.GetDocumentFileContent(jpegFile);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("data:image/jpeg;base64,"));

                var base64Part = result.Substring("data:image/jpeg;base64,".Length);
                var decodedBytes = Convert.FromBase64String(base64Part);
                Assert.That(decodedBytes, Is.EqualTo(testContent));
            }
            finally
            {
                // Cleanup
                if (File.Exists(jpegFile))
                    File.Delete(jpegFile);
            }
        }

        [Test]
        public async Task GetDocumentFileContent_WithExistingPngFile_ReturnsBase64Content()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create a temporary test file
            var tempFile = Path.GetTempFileName();
            var pngFile = Path.ChangeExtension(tempFile, ".png");
            var testContent = "Test PNG content"u8.ToArray();
            await File.WriteAllBytesAsync(pngFile, testContent);

            try
            {
                // Act
                var result = await documentService.GetDocumentFileContent(pngFile);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("data:image/png;base64,"));
            }
            finally
            {
                // Cleanup
                if (File.Exists(pngFile))
                    File.Delete(pngFile);
            }
        }

        [Test]
        public async Task GetDocumentFileContent_WithUnknownFileExtension_ReturnsOctetStreamContent()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create a temporary test file
            var tempFile = Path.GetTempFileName();
            var unknownFile = Path.ChangeExtension(tempFile, ".xyz");
            var testContent = "Test unknown content"u8.ToArray();
            await File.WriteAllBytesAsync(unknownFile, testContent);

            try
            {
                // Act
                var result = await documentService.GetDocumentFileContent(unknownFile);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Does.StartWith("data:application/octet-stream;base64,"));
            }
            finally
            {
                // Cleanup
                if (File.Exists(unknownFile))
                    File.Delete(unknownFile);
            }
        }

        [Test]
        public async Task UploadFile_WithValidDto_CreatesDirectoryAndReturnsDocument()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create mock IFormFile
            var fileContent = "Test file content"u8.ToArray();
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    return stream.WriteAsync(fileContent, 0, fileContent.Length, token);
                });

            var dto = new DocumentUploadDto
            {
                OriginalDocumentName = "test-document.pdf",
                File = mockFormFile.Object
            };

            var tempUploadsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            string userId = "test-user-123";

            try
            {
                // Act
                var result = await documentService.UploadFile(dto, tempUploadsFolder, userId);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.OriginalDocumentName, Is.EqualTo("test-document.pdf"));
                Assert.That(result.UserId, Is.EqualTo(userId));
                Assert.That(Directory.Exists(tempUploadsFolder), Is.True);

                // Verify a file was created with GUID name + original extension
                var files = Directory.GetFiles(tempUploadsFolder, "*.pdf");
                Assert.That(files.Length, Is.EqualTo(1));

                // Verify file content
                var actualFileContent = await File.ReadAllBytesAsync(files[0]);
                Assert.That(actualFileContent, Is.EqualTo(fileContent));

                // Verify logging
                mockLogger.Verify(
                    x => x.Log(
                        LogLevel.Information,
                        It.IsAny<EventId>(),
                        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Uploading file to:")),
                        It.IsAny<Exception>(),
                        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                    Times.Once);
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempUploadsFolder))
                    Directory.Delete(tempUploadsFolder, true);
            }
        }

        [Test]
        public async Task UploadFile_WithNullUserId_UsesDefaultUserId()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create mock IFormFile
            var fileContent = "Test file content"u8.ToArray();
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    return stream.WriteAsync(fileContent, 0, fileContent.Length, token);
                });

            var dto = new DocumentUploadDto
            {
                OriginalDocumentName = "test-document.pdf",
                File = mockFormFile.Object
            };

            var tempUploadsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                // Act
                var result = await documentService.UploadFile(dto, tempUploadsFolder, null);

                // Assert
                Assert.That(result, Is.Not.Null);
                Assert.That(result.OriginalDocumentName, Is.EqualTo("test-document.pdf"));
                Assert.That(result.UserId, Is.EqualTo("0")); // Default value when userId is null
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempUploadsFolder))
                    Directory.Delete(tempUploadsFolder, true);
            }
        }

        [Test]
        public async Task UploadFile_WithDifferentFileExtensions_PreservesExtension()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create mock IFormFile
            var fileContent = "Test file content"u8.ToArray();
            var mockFormFile = new Mock<IFormFile>();
            mockFormFile.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
                .Returns((Stream stream, CancellationToken token) =>
                {
                    return stream.WriteAsync(fileContent, 0, fileContent.Length, token);
                });

            var dto = new DocumentUploadDto
            {
                OriginalDocumentName = "test-image.png",
                File = mockFormFile.Object
            };

            var tempUploadsFolder = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());

            try
            {
                // Act
                var result = await documentService.UploadFile(dto, tempUploadsFolder, "user123");

                // Assert
                Assert.That(result, Is.Not.Null);

                // Verify a PNG file was created
                var files = Directory.GetFiles(tempUploadsFolder, "*.png");
                Assert.That(files.Length, Is.EqualTo(1));
            }
            finally
            {
                // Cleanup
                if (Directory.Exists(tempUploadsFolder))
                    Directory.Delete(tempUploadsFolder, true);
            }
        }

        [Test]
        public async Task DeleteDocument_WithExistingFile_RemovesFileAndDatabaseEntry()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var document = new DocumentModel
            {
                Id = 1,
                UserId = "test-user",
                DocumentName = "test-doc-guid.pdf",
                OriginalDocumentName = "test-doc.pdf",
                DocumentDate = DateTime.Now
            };

            context.Documents.Add(document);
            await context.SaveChangesAsync();

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Create a temporary file
            var tempFile = Path.GetTempFileName();
            var testContent = "Test file content"u8.ToArray();
            await File.WriteAllBytesAsync(tempFile, testContent);

            try
            {
                // Act
                await documentService.DeleteDocument(document, tempFile);

                // Assert
                Assert.That(File.Exists(tempFile), Is.False);

                var deletedDocument = await context.Documents.FindAsync(1);
                Assert.That(deletedDocument, Is.Null);
            }
            finally
            {
                // Cleanup
                if (File.Exists(tempFile))
                    File.Delete(tempFile);
            }
        }

        [Test]
        public async Task DeleteDocument_WithNonExistentFile_RemovesDatabaseEntryOnly()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<DocumentService>>();
            var mockConfig = new Mock<IConfiguration>();
            var mockOcrService = new Mock<OCRService>(mockConfig.Object, Mock.Of<ILogger<OCRService>>(), Mock.Of<PathService>());
            var mockPathService = new Mock<PathService>(mockConfig.Object, Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(), Mock.Of<IWebHostEnvironment>());

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new ApplicationDbContext(options);

            var document = new DocumentModel
            {
                Id = 1,
                UserId = "test-user",
                DocumentName = "test-doc-guid.pdf",
                OriginalDocumentName = "test-doc.pdf",
                DocumentDate = DateTime.Now
            };

            context.Documents.Add(document);
            await context.SaveChangesAsync();

            var documentService = new DocumentService(mockLogger.Object, mockConfig.Object, context,
                mockOcrService.Object, mockPathService.Object);

            // Act
            await documentService.DeleteDocument(document, "non-existent-file.pdf");

            // Assert
            var deletedDocument = await context.Documents.FindAsync(1);
            Assert.That(deletedDocument, Is.Null);
        }
    }
}