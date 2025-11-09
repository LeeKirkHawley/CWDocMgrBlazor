using CWDocMgrBlazor.Models;
using CWDocMgrBlazor.Services;
using DocMgrLib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;

namespace CWDocMgrBlazor.Tests
{
    public class OCRServiceTests
    {
        [Test]
        public void Constructor_WithoutParameters_CreatesInstance()
        {
            // Act
            var ocrService = new OCRService();

            // Assert
            Assert.That(ocrService, Is.Not.Null);
        }

        [Test]
        public void Constructor_WithParameters_CreatesInstance()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            // Act
            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            // Assert
            Assert.That(ocrService, Is.Not.Null);
        }

        [Test]
        public void IsOCRable_WithPdfDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document.pdf",
                OriginalDocumentName = "test-document.pdf",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithUpperCasePdfDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document.PDF",
                OriginalDocumentName = "test-document.PDF",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithMixedCasePdfDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document.Pdf",
                OriginalDocumentName = "test-document.Pdf",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithJpegDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-image.jpg",
                OriginalDocumentName = "test-image.jpg",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithPngDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-image.png",
                OriginalDocumentName = "test-image.png",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithTiffDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-image.tiff",
                OriginalDocumentName = "test-image.tiff",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithBmpDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-image.bmp",
                OriginalDocumentName = "test-image.bmp",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithUnsupportedExtension_ReturnsFalse()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document.txt",
                OriginalDocumentName = "test-document.txt",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsOCRable_WithDocumentWithoutExtension_ReturnsFalse()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document",
                OriginalDocumentName = "test-document",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsOCRable_WithEmptyDocumentName_ReturnsFalse()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "",
                OriginalDocumentName = "",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsOCRable_WithNullDocumentName_ThrowsException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = null,
                OriginalDocumentName = null,
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act & Assert
            Assert.Throws<NullReferenceException>(() => ocrService.IsOCRable(document));
        }

        [Test]
        public void IsOCRable_WithGifDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-image.gif",
                OriginalDocumentName = "test-image.gif",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithWebpDocument_ReturnsTrue()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-image.webp",
                OriginalDocumentName = "test-image.webp",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsOCRable_WithDocxDocument_ReturnsFalse()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document.docx",
                OriginalDocumentName = "test-document.docx",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsOCRable_WithXlsxDocument_ReturnsFalse()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-spreadsheet.xlsx",
                OriginalDocumentName = "test-spreadsheet.xlsx",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void IsOCRable_WithComplexFileName_HandlesCorrectly()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test-document_v2.1(final).PDF",
                OriginalDocumentName = "test-document_v2.1(final).PDF",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void IsOCRable_WithMultipleDots_HandlesCorrectly()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<OCRService>>();
            var mockPathService = new Mock<PathService>(
                mockConfig.Object,
                Mock.Of<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>(),
                Mock.Of<IWebHostEnvironment>());

            var ocrService = new OCRService(mockConfig.Object, mockLogger.Object, mockPathService.Object);

            var document = new DocumentModel
            {
                Id = 1,
                DocumentName = "test.document.with.dots.pdf",
                OriginalDocumentName = "test.document.with.dots.pdf",
                UserId = "test-user",
                DocumentDate = DateTime.Now
            };

            // Act
            bool result = ocrService.IsOCRable(document);

            // Assert
            Assert.That(result, Is.True);
        }
    }
}