using AutoMapper;
using HR.LeaveManagement.Application.DTOs.LeaveAllocation.Validators;
using HR.LeaveManagement.Application.Features.LeaveAllocations.Requests.Commands;
using HR.LeaveManagement.Application.Contracts.Persistence;
using HR.LeaveManagement.Application.Responses;
using HR.LeaveManagement.Domain;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HR.LeaveManagement.Application.Contracts.Identity;
using System;
using System.Collections.Generic;

namespace HR.LeaveManagement.Application.Features.LeaveAllocations.Handlers.Commands
{
    public class CreateLeaveAllocationCommandHandler : IRequestHandler<CreateLeaveAllocationCommand, BaseCommandResponse>
    {
        private readonly ILeaveAllocationRepository _leaveAllocationRepository;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        public CreateLeaveAllocationCommandHandler(ILeaveAllocationRepository leaveAllocationRepository,
            ILeaveTypeRepository leaveTypeRepository, IMapper mapper,
            IUserService userService)
        {
            _leaveAllocationRepository = leaveAllocationRepository;
            _leaveTypeRepository = leaveTypeRepository;
            _userService = userService;
            _mapper = mapper;
        }
        public async Task<BaseCommandResponse> Handle(CreateLeaveAllocationCommand request, CancellationToken cancellationToken)
        {
            var response = new BaseCommandResponse();
            var validator = new CreateLeaveAllocationDtoValidator(_leaveTypeRepository);
            var validationResult = await validator.ValidateAsync(request.LeaveAllocationDto);

            if (validationResult.IsValid == false)
            {
                response.Success = false;
                response.Message = "Creation Failed";
                response.Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            }
            else
            {
                var leaveType = await _leaveTypeRepository.Get(request.LeaveAllocationDto.LeaveTypeId);
                var employees = await _userService.GetEmployees();
                var period = DateTime.Now.Year;
                var allocations = new List<LeaveAllocation>();

                foreach (var employee in employees)
                {
                    if (await _leaveAllocationRepository.AllocationExists(employee.Id, leaveType.Id, period))
                        continue;
                    allocations.Add(new LeaveAllocation
                    {
                        EmployeeId = employee.Id,
                        LeaveTypeId = leaveType.Id,
                        NumberOfDays = leaveType.DefaultDays,
                        Period = period
                    });
                }
                
                await _leaveAllocationRepository.AddAllocations(allocations);

                response.Success = true;
                response.Message = "Allocations created successfully";
                
            }
            return response;
        }
    }
}
