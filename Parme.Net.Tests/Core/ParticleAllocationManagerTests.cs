using System.Collections.Generic;
using Parme.Net.Core;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests.Core
{
    public class ParticleAllocationManagerTests
    {
        [Fact]
        public void Can_Reserve_Block_Under_Capacity()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(7);
            
            reservation.Length.ShouldBe(7);
            reservation.StartIndex.ShouldBe(0);
            reservation.LastUsedIndex.ShouldBe(6);
        }

        [Fact]
        public void Can_Reserve_Block_Over_Capacity()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(11);
            
            allocator.Capacity.ShouldBeGreaterThan(10);
            reservation.Length.ShouldBe(11);
            reservation.StartIndex.ShouldBe(0);
            reservation.LastUsedIndex.ShouldBe(10);
        }

        [Fact]
        public void Two_Consecutive_Reservations_Are_Adjacent()
        {
            var allocator = new ParticleAllocator(10);
            var first = allocator.Reserve(5);
            var second = allocator.Reserve(3);
            
            first.Length.ShouldBe(5);
            first.StartIndex.ShouldBe(0);
            first.LastUsedIndex.ShouldBe(4);
            
            second.Length.ShouldBe(3);
            second.StartIndex.ShouldBe(5);
            second.LastUsedIndex.ShouldBe(7);
        }

        [Fact]
        public void Disposing_Reservation_Allows_Indices_To_Be_Reused()
        {
            var allocator = new ParticleAllocator(10);
            using (var first = allocator.Reserve(5))
            {
                first.Length.ShouldBe(5);
                first.StartIndex.ShouldBe(0);
                first.LastUsedIndex.ShouldBe(4);
            }

            var second = allocator.Reserve(5);
            second.StartIndex.ShouldBe(0);
            second.LastUsedIndex.ShouldBe(4);
        }

        [Fact]
        public void Can_Reserve_When_Free_Space_Exists_But_No_Wide_Enough_Gaps()
        {
            var allocator = new ParticleAllocator(10);
            allocator.Reserve(3);
            var second = allocator.Reserve(1);
            allocator.Reserve(3);
            
            second.Dispose();

            var fourth = allocator.Reserve(3);
            fourth.Length.ShouldBe(3);
        }

        [Fact]
        public void Defrag_Occurs_When_2x_Free_Space_Exists_But_No_Wide_Enough_Gaps()
        {
            var allocator = new ParticleAllocator(15);
            var first = allocator.Reserve(3);
            var second = allocator.Reserve(2);
            var third = allocator.Reserve(3);
            var fourth = allocator.Reserve(2);
            var fifth = allocator.Reserve(3);
            
            second.Dispose();
            fourth.Dispose();

            var sixth = allocator.Reserve(3);
            
            allocator.Capacity.ShouldBe(15); // No increase in capacity in a defrag
            VerifyAllAreConsecutive(first, third, fifth, sixth);
        }

        [Fact]
        public void Can_Reserve_When_Not_Enough_Free_Space_Exists()
        {
            var allocator = new ParticleAllocator(10);
            allocator.Reserve(3);
            allocator.Reserve(2);
            allocator.Reserve(3);
            var fourth = allocator.Reserve(3);

            fourth.Length.ShouldBe(3);
            allocator.Capacity.ShouldBeGreaterThan(10);
        }

        private static void VerifyAllAreConsecutive(params ParticleAllocator.Reservation[] reservations)
        {
            var sortedSet = new SortedSet<ParticleAllocator.Reservation>(reservations, new ParticleAllocator.ReservationComparer());

            var prevIndex = -1;
            var idx = 0;
            foreach (var reservation in sortedSet)
            {
                reservation.StartIndex.ShouldBe(prevIndex + 1, $"Reservation #{idx + 1} had an unexpected start index");
                prevIndex = reservation.LastUsedIndex;
                idx++;
            }
        }
    }
}