using System;
using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests
{
    public class ParticleCollectionTests
    {
        [Fact]
        public void Properties_Marked_As_Valid_For_Writing_Can_Be_Retrieved()
        {
            var property = new ParticleProperty(typeof(bool), "Something");
            var allocator = new ParticleAllocator(10);
            allocator.RegisterProperty(property.Type, property.Name);

            var reservation = allocator.Reserve(5);
            var collection = new ParticleCollection(reservation)
            {
                ValidPropertiesToSet = new HashSet<ParticleProperty>(new[] {property})
            };

            var result = collection.GetPropertyValues<bool>(property);
            result.Length.ShouldBe(reservation.Length);
        }

        [Fact]
        public void Cannot_Get_Property_Not_In_Valid_Writable_Hash_Set()
        {
            var property1 = new ParticleProperty(typeof(bool), "Something");
            var property2 = new ParticleProperty(typeof(bool), "Something2");
            var allocator = new ParticleAllocator(10);
            allocator.RegisterProperty(property1.Type, property1.Name);
            allocator.RegisterProperty(property2.Type, property2.Name);

            var reservation = allocator.Reserve(5);
            var collection = new ParticleCollection(reservation)
            {
                ValidPropertiesToSet = new HashSet<ParticleProperty>(new[] {property1})
            };

            Assert.ThrowsAny<Exception>(() => collection.GetPropertyValues<bool>(property2));
        }
        
        [Fact]
        public void Properties_Marked_As_Valid_For_Reading_Can_Be_Retrieved_As_Read_Only()
        {
            var property = new ParticleProperty(typeof(bool), "Something");
            var allocator = new ParticleAllocator(10);
            allocator.RegisterProperty(property.Type, property.Name);

            var reservation = allocator.Reserve(5);
            var collection = new ParticleCollection(reservation)
            {
                ValidPropertiesToRead = new HashSet<ParticleProperty>(new[] {property})
            };

            var result = collection.GetReadOnlyPropertyValues<bool>(property);
            result.Length.ShouldBe(reservation.Length);
        }

        [Fact]
        public void Cannot_Get_Property_Not_In_Valid_Readable_Hash_Set()
        {
            var property1 = new ParticleProperty(typeof(bool), "Something");
            var property2 = new ParticleProperty(typeof(bool), "Something2");
            var allocator = new ParticleAllocator(10);
            allocator.RegisterProperty(property1.Type, property1.Name);
            allocator.RegisterProperty(property2.Type, property2.Name);

            var reservation = allocator.Reserve(5);
            var collection = new ParticleCollection(reservation)
            {
                ValidPropertiesToSet = new HashSet<ParticleProperty>(new[] {property1})
            };

            Assert.ThrowsAny<Exception>(() => collection.GetReadOnlyPropertyValues<bool>(property2));
        }
    }
}