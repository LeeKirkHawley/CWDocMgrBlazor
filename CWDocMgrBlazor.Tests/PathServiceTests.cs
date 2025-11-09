using CWDocMgrBlazor.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace SharedLib.Tests
{
    public class Tests
    {
        [Test]
        public void GetUploadFilePath_WithValidFilename_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "test-document.pdf";
            string expected = Path.Combine("C:\\TestApp", "uploads", filename);

            // Act
            string result = pathService.GetUploadFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUploadFilePath_WithFilenameContainingSpaces_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "test document with spaces.pdf";
            string expected = Path.Combine("C:\\TestApp", "uploads", filename);

            // Act
            string result = pathService.GetUploadFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUploadFolderPath_WithValidConfiguration_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string expected = Path.Combine("C:\\TestApp", "uploads");

            // Act
            string result = pathService.GetUploadFolderPath();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUploadFolderPath_WithNullConfiguration_ThrowsException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns((string)null);

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);

            // Act & Assert
            var ex = Assert.Throws<System.ArgumentNullException>(() => pathService.GetUploadFolderPath());
            Assert.That(ex.Message, Is.EqualTo("Value cannot be null. (Parameter 'path2')"));

            // Verify critical log was called
            //mockLogger.Verify(
            //    x => x.Log(
            //        LogLevel.Critical,
            //        It.IsAny<EventId>(),
            //        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No configured upload folder.")),
            //        It.IsAny<Exception>(),
            //        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            //    Times.Once);
        }

        [Test]
        public void GetOCRFilePath_WithValidFilename_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("ocr-output");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "processed-document.txt";
            string expected = Path.Combine("C:\\TestApp", "ocr-output", filename);

            // Act
            string result = pathService.GetOCRFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetOCRFolderPath_WithValidConfiguration_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("ocr-output");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string expected = Path.Combine("C:\\TestApp", "ocr-output");

            // Act
            string result = pathService.GetOCRFolderPath();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetOCRFolderPath_WithValidConfiguration_LogsInformation()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("ocr-output");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);

            // Act
            pathService.GetOCRFolderPath();

            // Assert
            mockLogger.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("In GetOCRFolderPath() - _env.ContentRootPath: C:\\TestApp")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }

        [Test]
        public void GetOCRFolderPath_WithNullConfiguration_ThrowsException()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns((string)null);

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);

            // Act & Assert
            var ex = Assert.Throws<Exception>(() => pathService.GetOCRFolderPath());
            Assert.That(ex.Message, Is.EqualTo("No configured OCR folder."));

            // Verify critical log was called
            //mockLogger.Verify(
            //    x => x.Log(
            //        LogLevel.Critical,
            //        It.IsAny<EventId>(),
            //        It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("NValue cannot be null. (Parameter 'path2')")),
            //        It.IsAny<Exception>(),
            //        It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            //    Times.Once);
        }

        [Test]
        public void GetUploadFilePath_WithEmptyFilename_ReturnsPathToFolder()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "";
            string expected = Path.Combine("C:\\TestApp", "uploads", "");

            // Act
            string result = pathService.GetUploadFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetOCRFilePath_WithEmptyFilename_ReturnsPathToFolder()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("ocr-output");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "";
            string expected = Path.Combine("C:\\TestApp", "ocr-output", "");

            // Act
            string result = pathService.GetOCRFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUploadFolderPath_WithDifferentContentRootPath_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("/var/www/myapp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string expected = Path.Combine("/var/www/myapp", "uploads");

            // Act
            string result = pathService.GetUploadFolderPath();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetOCRFolderPath_WithDifferentContentRootPath_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("/var/www/myapp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("ocr-output");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string expected = Path.Combine("/var/www/myapp", "ocr-output");

            // Act
            string result = pathService.GetOCRFolderPath();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUploadFolderPath_WithCustomUploadsFolderName_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("custom-uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string expected = Path.Combine("C:\\TestApp", "custom-uploads");

            // Act
            string result = pathService.GetUploadFolderPath();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetOCRFolderPath_WithCustomOCRFolderName_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("custom-ocr");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string expected = Path.Combine("C:\\TestApp", "custom-ocr");

            // Act
            string result = pathService.GetOCRFolderPath();

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetUploadFilePath_WithSpecialCharactersInFilename_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["UploadsFolder"]).Returns("uploads");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "test-document_v2.1(final).pdf";
            string expected = Path.Combine("C:\\TestApp", "uploads", filename);

            // Act
            string result = pathService.GetUploadFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }

        [Test]
        public void GetOCRFilePath_WithSpecialCharactersInFilename_ReturnsCorrectPath()
        {
            // Arrange
            var mockConfig = new Mock<IConfiguration>();
            var mockLogger = new Mock<ILogger<CWDocMgrBlazor.Controllers.DocumentsController>>();
            var mockEnv = new Mock<IWebHostEnvironment>();

            mockEnv.Setup(x => x.ContentRootPath).Returns("C:\\TestApp");
            mockConfig.Setup(x => x["OCROutputFolder"]).Returns("ocr-output");

            var pathService = new PathService(mockConfig.Object, mockLogger.Object, mockEnv.Object);
            string filename = "processed-document_2024-01-15.txt";
            string expected = Path.Combine("C:\\TestApp", "ocr-output", filename);

            // Act
            string result = pathService.GetOCRFilePath(filename);

            // Assert
            Assert.That(result, Is.EqualTo(expected));
        }
    }
}