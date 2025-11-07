using SharedLib.Extensions;

namespace SharedLib.Tests
{
    [TestFixture]
    public class StringExtensionsTests
    {
        [TestFixture]
        public class AllIndexesOfTests
        {
            [Test]
            public void AllIndexesOf_WithValidStringAndCharacter_ReturnsCorrectIndexes()
            {
                // Arrange
                string text = "hello world";
                char character = 'l';

                // Act
                var result = text.AllIndexesOf(character).ToList();

                // Assert
                Assert.That(result, Is.EqualTo(new[] { 2, 3, 9 }));
            }

            [Test]
            public void AllIndexesOf_WithCharacterNotInString_ReturnsEmpty()
            {
                // Arrange
                string text = "hello world";
                char character = 'z';

                // Act
                var result = text.AllIndexesOf(character).ToList();

                // Assert
                Assert.That(result, Is.Empty);
            }

            [Test]
            public void AllIndexesOf_WithEmptyString_ReturnsEmpty()
            {
                // Arrange
                string text = "";
                char character = 'a';

                // Act
                var result = text.AllIndexesOf(character).ToList();

                // Assert
                Assert.That(result, Is.Empty);
            }

            [Test]
            public void AllIndexesOf_WithNullString_ReturnsEmpty()
            {
                // Arrange
                string? text = null;
                char character = 'a';

                // Act
                var result = text!.AllIndexesOf(character).ToList();

                // Assert
                Assert.That(result, Is.Empty);
            }

            [Test]
            public void AllIndexesOf_WithSingleCharacterString_ReturnsCorrectIndex()
            {
                // Arrange
                string text = "a";
                char character = 'a';

                // Act
                var result = text.AllIndexesOf(character).ToList();

                // Assert
                Assert.That(result, Is.EqualTo(new[] { 0 }));
            }

            [Test]
            public void AllIndexesOf_WithRepeatedCharacters_ReturnsAllIndexes()
            {
                // Arrange
                string text = "aaaaa";
                char character = 'a';

                // Act
                var result = text.AllIndexesOf(character).ToList();

                // Assert
                Assert.That(result, Is.EqualTo(new[] { 0, 1, 2, 3, 4 }));
            }
        }

        [TestFixture]
        public class StripExtensionFromImageFileTests
        {
            [Test]
            public void StripExtensionFromImageFile_WithJpgExtension_RemovesExtension()
            {
                // Arrange
                string fileName = "image.jpg";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("image"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithJpegExtension_RemovesExtension()
            {
                // Arrange
                string fileName = "document.jpeg";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("document"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithPngExtension_RemovesExtension()
            {
                // Arrange
                string fileName = "photo.png";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("photo"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithPdfExtension_RemovesExtension()
            {
                // Arrange
                string fileName = "document.pdf";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("document"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithUpperCaseExtension_RemovesExtension()
            {
                // Arrange
                string fileName = "image.JPG";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("image"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithMixedCaseExtension_RemovesExtension()
            {
                // Arrange
                string fileName = "image.JpG";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("image"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithUnsupportedExtension_ReturnsOriginal()
            {
                // Arrange
                string fileName = "document.txt";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("document.txt"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithNoExtension_ReturnsOriginal()
            {
                // Arrange
                string fileName = "document";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("document"));
            }

            [Test]
            public void StripExtensionFromImageFile_WithEmptyString_ReturnsEmpty()
            {
                // Arrange
                string fileName = "";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(""));
            }

            [Test]
            public void StripExtensionFromImageFile_WithNullString_ReturnsNull()
            {
                // Arrange
                string? fileName = null;

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName!);

                // Assert
                Assert.That(result, Is.Null);
            }

            [Test]
            public void StripExtensionFromImageFile_WithComplexFileName_RemovesOnlyExtension()
            {
                // Arrange
                string fileName = "my.document.with.dots.jpg";

                // Act
                string result = StringExtensions.StripExtensionFromImageFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo("my.document.with.dots"));
            }
        }

        [TestFixture]
        public class GetAllowedExtensionFromFileTests
        {
            [Test]
            public void GetAllowedExtensionFromFile_WithJpgExtension_ReturnsExtension()
            {
                // Arrange
                string fileName = "image.jpg";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(".jpg"));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithJpegExtension_ReturnsExtension()
            {
                // Arrange
                string fileName = "document.jpeg";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(".jpeg"));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithPngExtension_ReturnsExtension()
            {
                // Arrange
                string fileName = "photo.png";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(".png"));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithPdfExtension_ReturnsExtension()
            {
                // Arrange
                string fileName = "document.pdf";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(".pdf"));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithUpperCaseExtension_ReturnsLowerCase()
            {
                // Arrange
                string fileName = "image.JPG";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(".jpg"));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithMixedCaseExtension_ReturnsLowerCase()
            {
                // Arrange
                string fileName = "image.JpG";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(".jpg"));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithUnsupportedExtension_ReturnsEmpty()
            {
                // Arrange
                string fileName = "document.txt";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(""));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithNoExtension_ReturnsEmpty()
            {
                // Arrange
                string fileName = "document";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(""));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithEmptyString_ReturnsEmpty()
            {
                // Arrange
                string fileName = "";

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                // Assert
                Assert.That(result, Is.EqualTo(""));
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithNullString_ReturnsNull()
            {
                // Arrange
                string? fileName = null;

                // Act
                string result = StringExtensions.GetAllowedExtensionFromFile(fileName!);

                // Assert
                Assert.That(result, Is.Null);
            }

            [Test]
            public void GetAllowedExtensionFromFile_WithAllSupportedExtensions_ReturnsCorrectExtensions()
            {
                // Test all supported extensions from the enum
                var testCases = new[]
                {
                    ("file.jpg", ".jpg"),
                    ("file.jpeg", ".jpeg"),
                    ("file.png", ".png"),
                    ("file.bmp", ".bmp"),
                    ("file.tiff", ".tiff"),
                    ("file.tif", ".tif"),
                    ("file.gif", ".gif"),
                    ("file.pdf", ".pdf")
                };

                foreach (var (fileName, expectedExtension) in testCases)
                {
                    // Act
                    string result = StringExtensions.GetAllowedExtensionFromFile(fileName);

                    // Assert
                    Assert.That(result, Is.EqualTo(expectedExtension),
                        $"Failed for file: {fileName}");
                }
            }
        }
    }
}