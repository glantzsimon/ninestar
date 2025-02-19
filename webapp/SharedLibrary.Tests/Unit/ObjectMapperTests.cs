using K9.SharedLibrary.Exceptions;
using K9.SharedLibrary.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace K9.SharedLibrary.Tests.Unit
{
    public class ObjectMapperTests
    {

        public class TestClass1
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class TestClass2
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class TestClass3
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
            public DateTime CreatedOn { get; set; }
        }

        public class TestClass4
        {
            public Guid Id { get; set; }
            public string Type { get; set; }
            public string Email { get; set; }
            public DateTime UpdatedOn { get; set; }
            public List<TestClass3> Items { get; set; }
        }

        public class TestClass5
        {
            public Guid Id { get; set; }
            public string Type { get; set; }
            public string Email { get; set; }
            public List<TestClass4> Items { get; set; }

            public DateTime UpdatedOn => DateTime.Now.Date;
        }

        public class ArrayClass1
        {
            public TestClass4[] Items { get; set; }
        }

        public class ArrayClass2
        {
            public TestClass3[] Items { get; set; }
        }

        readonly TestClass1 _item1 = new TestClass1
        {
            Id = 1,
            Name = "John",
            Email = "test@test.com",
            CreatedOn = new DateTime(2017, 1, 1),
        };

        readonly TestClass2 _item2 = new TestClass2
        {
            Id = 1,
            Name = "John",
            CreatedOn = new DateTime(2017, 1, 1),
        };

        readonly TestClass3 _item3 = new TestClass3
        {
            Id = Guid.NewGuid(),
            CreatedOn = new DateTime(2017, 1, 1),
            Email = "test@test.com",
            Name = "John"
        };

        readonly TestClass4 _item4 = new TestClass4
        {
            Id = Guid.NewGuid(),
            UpdatedOn = new DateTime(2017, 1, 1),
            Email = "test@test.com",
            Type = "Teapot",
            Items = new List<TestClass3>
            {
                new TestClass3
                {
                    CreatedOn = new DateTime(2017, 1, 1),
                    Email = "test@test.com"
                },
                new TestClass3
                {
                    CreatedOn = new DateTime(2017, 1, 1),
                    Email = "test@test.com"
                }
            }
        };

        public class CustomMapper : CustomMapBase<TestClass3, TestClass4>
        {
            public override TestClass4 MapFrom(TestClass3 item)
            {
                return new TestClass4
                {
                    Type = item.Name,
                    UpdatedOn = item.CreatedOn
                };
            }
        }

        [Fact]
        public void ObjectMapper_ShouldMapPropertiesBasedOnName()
        {
            var item2 = _item1.MapTo<TestClass2>();
            var item1 = _item2.MapTo<TestClass1>();

            Assert.Equal(_item1.Id, item2.Id);
            Assert.Equal(_item1.Name, item2.Name);
            Assert.Equal(_item1.CreatedOn, item2.CreatedOn);
            Assert.Equal(_item2.Id, item1.Id);
            Assert.Equal(_item2.Name, item1.Name);
            Assert.Equal(_item2.CreatedOn, item1.CreatedOn);
        }

        [Fact]
        public void ObjectMapper_ShouldThrow_FailedToMapPropertyException_When_PropertyTypesDontMatch()
        {
            Assert.Throws<FailedToMapPropertyException>(() => _item1.MapTo<TestClass3>());
        }

        [Fact]
        public void CustomMap_ShouldOnlyMapThosePropertiesThatAreSpecified()
        {
            var item = _item3.MapTo<TestClass4>();

            Assert.Equal(_item3.Name, item.Type);
            Assert.Equal(_item3.CreatedOn, item.UpdatedOn);
            Assert.Equal(null, item.Email);
            Assert.Equal(Guid.Empty, item.Id);
        }

        [Fact]
        public void Mapper_ShouldNotMapReadOnlyProperties()
        {
            var entity = _item4.MapTo<TestClass5>();

            // In case the test runs at midnight...
            Assert.True(entity.UpdatedOn == DateTime.Today || entity.UpdatedOn == DateTime.Today.AddDays(-1));
            Assert.Equal(2, entity.Items.Count);

            Assert.Equal(_item4.Items.First().Email, entity.Items.First().Email);
        }

        [Fact]
        public void Mapper_ShouldMapArrayProperties()
        {
            var entity = new ArrayClass1
            {
                Items = new[]
                {
                    new TestClass4
                    {
                        Email = "joebloggs@test.com"
                    }
                }
            };

            var mapped = entity.MapTo<ArrayClass2>();

            Assert.Equal(entity.Items.Length, mapped.Items.Length);
            Assert.Equal(entity.Items.First().Email, mapped.Items.First().Email);
        }
    }
}
