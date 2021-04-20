using System.Collections.Generic;
using Shouldly;
using Xunit;

namespace Parme.Net.Tests
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

        [Fact]
        public void KeyNotFoundException_When_Property_Is_Not_Registered()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(5);
            
            Assert.Throws<KeyNotFoundException>(() => reservation.GetPropertyValues<float>("Something"));
        }

        [Fact]
        public void KeyNotFoundException_When_Property_Registered_For_Different_Type()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(5);
            allocator.RegisterProperty<float>("Something");

            Assert.Throws<KeyNotFoundException>(() => reservation.GetPropertyValues<bool>("Something"));
        }

        [Fact]
        public void Can_Set_Value_For_Registered_Property()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(5);
            allocator.RegisterProperty<float>("Something");

            var values = reservation.GetPropertyValues<float>("Something");
            
            values.Length.ShouldBe(5);
            values[0] = 1.1f;
            values[1] = 2.2f;
            values[2] = 3.3f;
            values[3] = 4.4f;
            values[4] = 5.5f;
            
            var values2 = reservation.GetPropertyValues<float>("Something");
            
            values2[0].ShouldBe(values[0]);
            values2[1].ShouldBe(values[1]);
            values2[2].ShouldBe(values[2]);
            values2[3].ShouldBe(values[3]);
            values2[4].ShouldBe(values[4]);
        }

        [Fact]
        public void Properties_Of_Different_Types_Can_Have_The_Same_Name()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(5);
            allocator.RegisterProperty<bool>("Something");
            allocator.RegisterProperty<float>("Something");

            var boolValues = reservation.GetPropertyValues<bool>("Something");
            var floatValues = reservation.GetPropertyValues<float>("Something");
        }

        [Fact]
        public void Property_Values_Remain_After_A_Defrag()
        {
            var allocator = new ParticleAllocator(15);
            allocator.RegisterProperty<float>("Something");
            
            var first = allocator.Reserve(3);
            var second = allocator.Reserve(2);
            var third = allocator.Reserve(3);
            var fourth = allocator.Reserve(2);
            var fifth = allocator.Reserve(3);

            var thirdStartIndex = third.StartIndex;
            {
                var values = third.GetPropertyValues<float>("Something");
                values[0] = 1.1f;
                values[1] = 2.2f;
                values[2] = 3.3f;
            }
        
            second.Dispose();
            fourth.Dispose();

            var sixth = allocator.Reserve(3);
        
            allocator.Capacity.ShouldBe(15); // No increase in capacity in a defrag
            VerifyAllAreConsecutive(first, third, fifth, sixth); // defrag actually occured

            third.StartIndex.ShouldNotBe(thirdStartIndex, "Expected 'third' to have moved start indexes");
            var newValues = third.GetPropertyValues<float>("Something");
            newValues[0].ShouldBe(1.1f);
            newValues[1].ShouldBe(2.2f);
            newValues[2].ShouldBe(3.3f);
        }

        [Fact]
        public void Property_Values_Remain_After_Expansion()
        {
            var allocator = new ParticleAllocator(5);
            allocator.RegisterProperty<float>("Something");

            var first = allocator.Reserve(3);
            {
                var values = first.GetPropertyValues<float>("Something");
                values[0] = 1.1f;
                values[1] = 2.2f;
                values[2] = 3.3f;
            }

            allocator.Reserve(3);
            
            allocator.Capacity.ShouldBeGreaterThan(5, "Expected capacity to grow");

            var newValues = first.GetPropertyValues<float>("Something");
            newValues[0].ShouldBe(1.1f);
            newValues[1].ShouldBe(2.2f);
            newValues[2].ShouldBe(3.3f);
        }

        [Fact]
        public void Multiple_Reservations_Have_Distinct_Property_Values()
        {
            var allocator = new ParticleAllocator(8);
            allocator.RegisterProperty<float>("Something");

            var first = allocator.Reserve(3);
            var second = allocator.Reserve(2);
            var third = allocator.Reserve(3);
            allocator.Capacity.ShouldBe(8, "Allocator capacity shouldn't have changed yet");

            {
                var values = first.GetPropertyValues<float>("Something");
                values[0] = 1.1f;
                values[1] = 2.2f;
                values[2] = 3.3f;
            }

            {
                var values = third.GetPropertyValues<float>("Something");
                values[0] = 4.4f;
                values[1] = 5.5f;
                values[2] = 6.6f;
            }
            
            second.Dispose();
            allocator.Reserve(3); // cause defrag or expansion, don't care which for this test

            {
                var values = first.GetPropertyValues<float>("Something");
                values[0].ShouldBe(1.1f);
                values[1].ShouldBe(2.2f);
                values[2].ShouldBe(3.3f);
            }

            {
                var values = third.GetPropertyValues<float>("Something");
                values[0].ShouldBe(4.4f);
                values[1].ShouldBe(5.5f);
                values[2].ShouldBe(6.6f);
            }
        }

        [Fact]
        public void Can_Expand_Reservation_When_Its_The_Only_Reservation()
        {
            var allocator = new ParticleAllocator(10);
            var reservation = allocator.Reserve(5);
            reservation.Expand(3);
            
            reservation.Length.ShouldBe(8);
            reservation.StartIndex.ShouldBe(0);
            reservation.LastUsedIndex.ShouldBe(7);
        }

        [Fact]
        public void Can_Expand_Reservation_Into_Disposed_Gap()
        {
            var allocator = new ParticleAllocator(10);
            var first = allocator.Reserve(5);
            var second = allocator.Reserve(3);
            allocator.Reserve(2);
            
            second.Dispose();
            first.Expand(3);
            
            first.Length.ShouldBe(8);
            first.StartIndex.ShouldBe(0);
            first.LastUsedIndex.ShouldBe(7);
        }

        [Fact]
        public void Can_Expand_When_Not_Enough_Free_Space_Exists()
        {
            var allocator = new ParticleAllocator(10);
            allocator.Reserve(3);
            var reservation = allocator.Reserve(5);
            reservation.Expand(10);
            
            reservation.Length.ShouldBe(15);
        }

        [Fact]
        public void Can_Expand_When_Free_Space_Exists_But_No_Big_Enough_Gaps()
        {
            var allocator = new ParticleAllocator(10);
            var first = allocator.Reserve(3);
            var second = allocator.Reserve(2);
            allocator.Reserve(3);
            second.Dispose();
            
            first.Expand(3);
            
            first.Length.ShouldBe(6);
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