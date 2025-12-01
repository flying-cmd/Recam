using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Remp.Common.Exceptions;
using Remp.Common.Helpers;
using Remp.DataAccess.Data;
using Remp.Models.Constants;
using Remp.Models.Entities;
using Remp.Repository.Interfaces;
using Remp.Service.DTOs;
using Remp.Service.Interfaces;

namespace Remp.Service.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly AppDbContext _appDbContext;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        AppDbContext appDbContext)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userManager = userManager;
        _roleManager = roleManager;
        _appDbContext = appDbContext;
    }

    public async Task<bool> AddAgentByIdAsync(string agentId, string photographyCompanyId)
    {
        // Check if the agent exists
        var agent = await _userRepository.FindAgentByIdAsync(agentId);
        if (agent is null)
        {
            throw new NotFoundException(message: $"Agent {agentId} does not exist", title: "Agent does not exist");
        }

        // Check if the photography company already added the agent
        if (await _userRepository.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId))
        {
            return false;
        }

        await _userRepository.AddAgentToPhotographyCompanyAsync(agentId, photographyCompanyId);

        return true;
    }

    public async Task<CreateAgentAccountResponseDto?> CreateAgentAccountAsync(CreateAgentAccountRequestDto createAgentAccountRequestDto, string photographyCompanyId)
    {
        // Check if the email already exists
        var emailExists = await _userRepository.FindByEmailAsync(createAgentAccountRequestDto.Email);
        if (emailExists != null)
        {
            // Log
            UserActivityLog.LogCreateAgentAccount(
                photographyCompanyId: photographyCompanyId,
                createdAgentId: null,
                createdAgentEmail: createAgentAccountRequestDto.Email,
                description: "Failed to create agent account because the email already exists"
                );

            throw new RegisterException(message: "Email already exists", title: "Email already exists");
        }

        // Generate random password
        var password = PasswordHelper.GenerateRandomPassword();

        // Create agent account
        var user = new User
        {
            Email = createAgentAccountRequestDto.Email,
            UserName = createAgentAccountRequestDto.Email
        };

        await using var transaction = await _appDbContext.Database.BeginTransactionAsync();

        try
        {
            var result = await _userManager.CreateAsync(user, password);

            // Add user to User table
            if (!result.Succeeded)
            {
                var errors = string.Join("| ", result.Errors.Select(x => x.Description));

                // Log
                UserActivityLog.LogCreateAgentAccount(
                    photographyCompanyId: photographyCompanyId,
                    createdAgentId: null,
                    createdAgentEmail: createAgentAccountRequestDto.Email,
                    description: $"Failed to create agent account with errors: {errors}"
                    );

                throw new RegisterException(
                    message: $"Failed to create Identity user with errors: {errors}",
                    title: "Failed to create agent account");
            }

            // Add role to Role table
            var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Agent);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("| ", roleResult.Errors.Select(x => x.Description));

                // Log
                UserActivityLog.LogCreateAgentAccount(
                    photographyCompanyId: photographyCompanyId,
                    createdAgentId: null,
                    createdAgentEmail: createAgentAccountRequestDto.Email,
                    description: $"Failed to assign {RoleNames.Agent} role to Identity user with errors: {errors}"
                    );

                throw new RegisterException(
                    message: $"Failed to assign {RoleNames.Agent} role to Identity user with errors: {errors}",
                    title: "Failed to create agent account");
            }

            // Add agent to Agent table
            var agent = new Agent
            {
                Id = user.Id,
            };

            await _userRepository.AddAgentAsync(agent);

            // Commit transaction
            await transaction.CommitAsync();

            // Log
            UserActivityLog.LogCreateAgentAccount(
                photographyCompanyId: photographyCompanyId,
                createdAgentId: user.Id.ToString(),
                createdAgentEmail: user.Email
            );

            return new CreateAgentAccountResponseDto(user.Id, createAgentAccountRequestDto.Email, password);
        }
        catch (Exception)
        {
            // Rollback transaction
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<SearchAgentResponseDto?> GetAgentByEmailAsync(string email)
    {
        var agent = await _userRepository.GetAgentByEmailAsync(email);
        return _mapper.Map<SearchAgentResponseDto>(agent);
    }

    public async Task<PagedResult<CreateAgentAccountResponseDto>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentErrorException(message: "Page number and page size must be greater than 0.", title: "Page number and page size must be greater than 0.");
        }

        var totalCount = await _userRepository.GetTotalCountAsync();

        var agents = await _userRepository.GetAgentsAsync(pageNumber, pageSize);

        if (agents == null || !agents.Any())
        {
            throw new NotFoundException(message: $"No agents found. Page number: {pageNumber}, Page size: {pageSize}", title: "No agents found.");
        }

        var agentsDto = _mapper.Map<IEnumerable<CreateAgentAccountResponseDto>>(agents);

        return new PagedResult<CreateAgentAccountResponseDto>(pageNumber, pageSize, totalCount, agentsDto);
    }

    public async Task<IEnumerable<SearchAgentResponseDto>> GetAgentsUnderPhotographyCompanyAsync(string photographyCompanyId)
    {
        // Check if the photography company exists
        var photographyCompany = await _userRepository.FindPhotographyCompanyByIdAsync(photographyCompanyId);
        if (photographyCompany is null)
        {
            throw new NotFoundException(message: $"Photography company {photographyCompanyId} does not exist", title: "Photography company does not exist");
        }

        var agents = await _userRepository.GetAgentsUnderPhotographyCompanyAsync(photographyCompanyId);

        return _mapper.Map<IEnumerable<SearchAgentResponseDto>>(agents);
    }

    public async Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId)
    {
        return await _userRepository.GetUserListingCaseIdsAsync(currentUserId);
    }

    public async Task<UpdateApiResponse> UpdatePasswordAsync(UpdatePasswordRequestDto updatePasswordRequestDto, string userId)
    {
        // Check if the user exists
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException(message: $"User {userId} does not exist", title: "User does not exist");
        }

        // Update password
        var result = await _userManager.ChangePasswordAsync(user, updatePasswordRequestDto.OldPassword, updatePasswordRequestDto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("| ", result.Errors.Select(x => x.Description));

            return new UpdateApiResponse(
                false,
                message: "Failed to update password.",
                errors);
        }

        return new UpdateApiResponse(
            true,
            message: "Password updated successfully.");
    }
}
