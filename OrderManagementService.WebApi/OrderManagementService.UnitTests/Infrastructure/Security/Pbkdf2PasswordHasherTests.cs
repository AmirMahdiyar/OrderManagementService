using FluentAssertions;
using OrderManagementService.Infrastructure.Security;

namespace OrderManagementService.UnitTests.Infrastructure.Security
{
    public class Pbkdf2PasswordHasherTests
    {
        private readonly Pbkdf2PasswordHasher _sut = new();

        [Fact]
        public void Hash_Generates_Valid_Base64_Salted_Hash_Format()
        {
            // Arrange
            var password = "StrongPassword#2026";

            // Act
            var hash = _sut.Hash(password);

            // Assert
            hash.Should().NotBeNullOrWhiteSpace();
            var parts = hash.Split('.');
            parts.Should().HaveCount(2);
            Convert.FromBase64String(parts[0]).Length.Should().Be(16); // 16 bytes salt
            Convert.FromBase64String(parts[1]).Length.Should().Be(32); // 32 bytes key
        }

        [Fact]
        public void Verify_Returns_True_For_Matching_Password()
        {
            // Arrange
            var password = "CorrectHorseBatteryStaple";
            var hash = _sut.Hash(password);

            // Act
            var result = _sut.Verify(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void Verify_Returns_False_For_Wrong_Password()
        {
            // Arrange
            var password = "CorrectPassword";
            var hash = _sut.Hash(password);

            // Act
            var result = _sut.Verify("IncorrectPassword", hash);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("malformed-hash-without-dot")]
        [InlineData("part1.part2.part3")]
        [InlineData("")]
        public void Verify_Returns_False_For_Malformed_Hash(string malformedHash)
        {
            // Arrange & Act
            var result = _sut.Verify("anyPassword", malformedHash);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Hash_Produces_Different_Hashes_For_Same_Password_Due_To_Cryptographic_Salt()
        {
            // Arrange
            var password = "IdenticalPassword123";

            // Act
            var hash1 = _sut.Hash(password);
            var hash2 = _sut.Hash(password);

            // Assert
            hash1.Should().NotBe(hash2);
            _sut.Verify(password, hash1).Should().BeTrue();
            _sut.Verify(password, hash2).Should().BeTrue();
        }
    }
}
