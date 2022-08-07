using HR.LeaveManagement.Application.Contracts.Persistence;
using Moq;

namespace HR.LeaveManagementApplication.UnitTests.Mocks
{
    public static class MockUnitOfWork
    {
        public static Mock<IUnitOfWork> GetUnitOfWork()
        {
            var mockUow = new Mock<IUnitOfWork>();
            var mockLeaveRepo = MockLeaveTypeRepository.GetLeaveTypeRepository();

            mockUow.Setup(r => r.LeaveTypeRepository).Returns(mockLeaveRepo.Object);

            return mockUow;
        }
    }
}
