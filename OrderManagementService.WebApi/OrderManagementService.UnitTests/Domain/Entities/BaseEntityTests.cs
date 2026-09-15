using FluentAssertions;
using OrderManagementService.Domain.Entities.Base.Entity;

namespace OrderManagementService.UnitTests.Domain.Entities
{
    public class BaseEntityTests
    {
        private class TestEntity : BaseEntity<Guid>
        {
            public TestEntity(Guid id)
            {
                Id = id;
            }

            protected override Guid InitialId() => Guid.NewGuid();
        }

        [Fact]
        public void Equals_Returns_True_For_Same_Reference()
        {
            // Arrange
            var sut = new TestEntity(Guid.NewGuid());

            // Act & Assert
            sut.Equals(sut).Should().BeTrue();
        }

        [Fact]
        public void Equals_Returns_False_For_Null_Or_Different_Type()
        {
            // Arrange
            var sut = new TestEntity(Guid.NewGuid());

            // Act & Assert
            sut.Equals((object?)null).Should().BeFalse();
            sut.Equals("not an entity").Should().BeFalse();
        }

        [Fact]
        public void Equals_Returns_False_When_Either_Entity_Has_Default_Id()
        {
            // Arrange
            var entityWithDefaultId = new TestEntity(Guid.Empty);
            var entityWithValidId = new TestEntity(Guid.NewGuid());

            // Act & Assert
            entityWithDefaultId.Equals(entityWithValidId).Should().BeFalse();
            entityWithValidId.Equals(entityWithDefaultId).Should().BeFalse();
        }

        [Fact]
        public void Equals_Returns_True_When_Both_Entities_Have_Matching_Id()
        {
            // Arrange
            var id = Guid.NewGuid();
            var sut = new TestEntity(id);
            var other = new TestEntity(id);

            // Act & Assert
            sut.Equals(other).Should().BeTrue();
            (sut == other).Should().BeTrue();
            (sut != other).Should().BeFalse();
            sut.GetHashCode().Should().Be(other.GetHashCode());
        }

        [Fact]
        public void Equality_Operators_Handle_Null_Operands_Correctly()
        {
            // Arrange
            TestEntity? nullEntity1 = null;
            TestEntity? nullEntity2 = null;
            var nonNullEntity = new TestEntity(Guid.NewGuid());

            // Act & Assert
            (nullEntity1 == nullEntity2).Should().BeTrue();
            (nullEntity1 == nonNullEntity).Should().BeFalse();
            (nonNullEntity == nullEntity1).Should().BeFalse();
            (nullEntity1 != nonNullEntity).Should().BeTrue();
        }
    }
}
