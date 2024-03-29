using System;
using HotelBooking.Core;
using HotelBooking.UnitTests.Fakes;
using Xunit;
using Moq;
using System.Collections.Generic;

namespace HotelBooking.UnitTests
{
    public class BookingManagerTests
    {
        private IBookingManager bookingManager;
        private IRepository<Booking> bookingRepository;

        public BookingManagerTests()
        {
            DateTime start = DateTime.Today.AddDays(10);
            DateTime end = DateTime.Today.AddDays(20);
            bookingRepository = new FakeBookingRepository(start, end);
            IRepository<Room> roomRepository = new FakeRoomRepository();
            bookingManager = new BookingManager(bookingRepository, roomRepository);
        }

        [Fact]
        public void FindAvailableRoom_StartDateNotInTheFuture_ThrowsArgumentException()
        {
            DateTime date = DateTime.Today;
            Assert.Throws<ArgumentException>(() => bookingManager.FindAvailableRoom(date, date));
        }

        [Fact]
        public void FindAvailableRoom_RoomAvailable_RoomIdNotMinusOne()
        {
            // Arrange
            DateTime date = DateTime.Today.AddDays(1);
            // Act
            int roomId = bookingManager.FindAvailableRoom(date, date);
            // Assert
            Assert.NotEqual(-1, roomId);
        }

        /**************/
        [Fact]
        public void FindAvailableRoom_EndDate_Before_StartDate_ThrowsArgumentException()
        {
            //Arrange
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today;

            //Assert
            Assert.Throws<ArgumentException>(() => bookingManager.FindAvailableRoom(startDate, endDate));
        }

        [Fact]
        public void FindAvailableRoomTest()
        {
            //Arrange
            List<Booking> bookings = new List<Booking>()
            {
                new Booking
                {
                    Id = 1,
                    IsActive = true,
                    StartDate = DateTime.Today.AddDays(1),
                    EndDate = DateTime.Today.AddDays(5),
                    RoomId = 1
                },
            };

            Mock<IRepository<Booking>> mockRepo = new Mock<IRepository<Booking>>();
            mockRepo.Setup(x => x.GetAll()).Returns(bookings);

            //Act
            bookingManager = new BookingManager(mockRepo.Object, new FakeRoomRepository());

            //Assert
            var availRoomId = bookingManager.FindAvailableRoom(DateTime.Today.AddDays(1), DateTime.Today.AddDays(5));
            Assert.InRange(availRoomId, 1, 2);
        }

        [Fact]
        public void GetFullyOccupiedDates_EndDate_Before_StartDate_ThrowsArgumentException()
        {
            //Arrange
            DateTime startDate = DateTime.Today.AddDays(1);
            DateTime endDate = DateTime.Today;

            //Assert
            Assert.Throws<ArgumentException>(() => bookingManager.GetFullyOccupiedDates(startDate, endDate));
        }

        [Fact]
        public void GetFullyOccupiedDates_Returns_Valid()
        {
            DateTime start = DateTime.Today.AddDays(1);
            DateTime end = DateTime.Today.AddDays(5);

            var act = bookingManager.GetFullyOccupiedDates(start, end);

            Assert.IsType<List<DateTime>>(act);
        }

        [Fact]
        public void CreateBooking_With_Invalid_Data()
        {
            Booking bk = new Booking
            {
                CustomerId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                Id = 55,
                IsActive = true,
                RoomId = -1
            };

            Assert.False(bookingManager.CreateBooking(bk));
        }

        [Fact]
        public void CreateBooking_With_Valid_Data()
        {
            Booking bk = new Booking
            {
                CustomerId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                Id = 55,
                IsActive = true,
                RoomId = 1
            };

            Assert.True(bookingManager.CreateBooking(bk));
        }

        [Fact]
        public void CreateBooking_With_Mock_Repo()
        {
            List<Booking> bookings = new List<Booking>()
            {
                new Booking
            {
                CustomerId = 1,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(5),
                Id = 55,
                IsActive = true,
                RoomId = 1
            }
            };


            Mock<IRepository<Booking>> mockRepo = new Mock<IRepository<Booking>>();
            mockRepo.Setup(x => x.GetAll()).Returns(bookings);

            bookingManager = new BookingManager(mockRepo.Object, new FakeRoomRepository());

            Booking bk = new Booking()
            {
                StartDate = DateTime.Today.AddDays(3),
                EndDate = DateTime.Today.AddDays(5),
                RoomId = 5
            };

            Assert.True(bookingManager.CreateBooking(bk));
        }
    }
}
