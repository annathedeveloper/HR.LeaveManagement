
using AutoMapper;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.DTOs.LeaveType;
using HR.LeaveManagement.Application.Features.LeaveTypes.Handlers.Commands;
using HR.LeaveManagement.Application.Features.LeaveTypes.Requests.Commands;
using HR.LeaveManagement.Application.Profiles;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagementApplication.UnitTests.Mocks;
using Moq;
using Shouldly;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace HR.LeaveManagementApplication.UnitTests.LeaveTypes.Commands
{
    public class CreateLeaveTypeCommandHandlerTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILeaveTypeRepository> _mockRepo;
        private readonly CreateLeaveTypeDto _leaveTypeDto;

        public CreateLeaveTypeCommandHandlerTests()
        {
            _mockRepo = MockLeaveTypeRepository.GetLeaveTypeRepository();

            var mapperConfig = new MapperConfiguration(c => 
            {
                c.AddProfile<MappingProfile>();
            });

            _mapper = mapperConfig.CreateMapper();

            _leaveTypeDto = new CreateLeaveTypeDto
            {
                DefaultDays = 15,
                Name = "Test DTO"
            };
        }

        [Fact]
        public async Task Valid_LeaveType_Added()
        {
            var handler = new CreateLeaveTypeCommandHandler(_mockRepo.Object, _mapper);

            var result = await handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = _leaveTypeDto }, CancellationToken.None);            

            var leaveTypes = await _mockRepo.Object.GetAll();

            //Assert Type
            result.ShouldBeOfType<BaseCommandResponse>();

            leaveTypes.Count.ShouldBe(3); 
        }

        [Fact]
        public async Task Invalid_LeaveType_Added()
        {
            var handler = new CreateLeaveTypeCommandHandler(_mockRepo.Object, _mapper);

            _leaveTypeDto.DefaultDays = -1;

            /*ValidationException ex = await Should.ThrowAsync<ValidationException>
                ( async () =>
                    await handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = _leaveTypeDto }, CancellationToken.None)
                );*/
            var result = await handler.Handle(new CreateLeaveTypeCommand() { LeaveTypeDto = _leaveTypeDto }, CancellationToken.None);

            var leaveTypes = await _mockRepo.Object.GetAll();

            leaveTypes.Count.ShouldBe(2);

            result.ShouldBeOfType<BaseCommandResponse>();

           // ex.ShouldNotBeNull();
        }
    }
}
