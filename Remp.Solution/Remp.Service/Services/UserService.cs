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
    private readonly AppDbContext _appDbContext;
    private readonly ILoggerService _loggerService;

    public UserService(
        IUserRepository userRepository,
        IMapper mapper,
        UserManager<User> userManager,
        AppDbContext appDbContext,
        ILoggerService loggerService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _userManager = userManager;
        _appDbContext = appDbContext;
        _loggerService = loggerService;
    }

    public async Task<Agent?> AddAgentByIdAsync(string agentId, string photographyCompanyId)
    {
        // Check if the photography company exists
        var photographyCompany = await _userRepository.FindPhotographyCompanyByIdAsync(photographyCompanyId);
        if (photographyCompany is null)
        {
            throw new UnauthorizedException(message: $"Photography company {photographyCompanyId} does not exist", title: "Unauthenticated");
        }

        // Check if the agent exists
        var agent = await _userRepository.FindAgentByIdAsync(agentId);
        if (agent is null)
        {
            throw new NotFoundException(message: $"Agent {agentId} does not exist", title: "Agent does not exist");
        }

        // Check if the photography company already added the agent
        if (await _userRepository.IsAgentAddedToPhotographyCompanyAsync(agentId, photographyCompanyId))
        {
            throw new ArgumentErrorException(message: $"Photography company {photographyCompanyId} already added the agent {agentId}", title: "Photography company already added the agent");
        }

        // Add agent to photography company
        AgentPhotographyCompany agentPhotographyCompany = new AgentPhotographyCompany() { AgentId = agentId, PhotographyCompanyId = photographyCompanyId };
        var addedAgent = await _userRepository.AddAgentToPhotographyCompanyAsync(agentPhotographyCompany);

        return addedAgent;
    }

    public async Task<CreateAgentAccountResponseDto?> CreateAgentAccountAsync(CreateAgentAccountRequestDto createAgentAccountRequestDto, string photographyCompanyId)
    {
        // Check if the email already exists
        var emailExists = await _userRepository.FindUserByEmailAsync(createAgentAccountRequestDto.Email);
        if (emailExists != null)
        {
            // Log
            await _loggerService.LogCreateAgentAccount(
                photographyCompanyId: photographyCompanyId,
                createdAgentId: null,
                createdAgentEmail: createAgentAccountRequestDto.Email,
                error: "Email already exists"
                );

            throw new ArgumentErrorException(message: "Email already exists", title: "Email already exists");
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
                await _loggerService.LogCreateAgentAccount(
                    photographyCompanyId: photographyCompanyId,
                    createdAgentId: null,
                    createdAgentEmail: createAgentAccountRequestDto.Email,
                    error: errors
                    );

                throw new DbErrorException(
                    message: $"Failed to create Identity user with errors: {errors}",
                    title: "Failed to create agent account");
            }

            // Add role to Role table
            var roleResult = await _userManager.AddToRoleAsync(user, RoleNames.Agent);
            if (!roleResult.Succeeded)
            {
                var errors = string.Join("| ", roleResult.Errors.Select(x => x.Description));

                // Log
                await _loggerService.LogCreateAgentAccount(
                    photographyCompanyId: photographyCompanyId,
                    createdAgentId: null,
                    createdAgentEmail: createAgentAccountRequestDto.Email,
                    error: errors
                    );

                throw new DbErrorException(
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
            await _loggerService.LogCreateAgentAccount(
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

        if (agent is null)
        {
            throw new NotFoundException(message: $"Agent with email {email} does not exist", title: "Agent does not exist");
        }

        return _mapper.Map<SearchAgentResponseDto>(agent);
    }

    public async Task<PagedResult<SearchAgentResponseDto>> GetAgentsAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
        {
            throw new ArgumentErrorException(message: "Page number and page size must be greater than 0.", title: "Page number and page size must be greater than 0.");
        }

        var agents = await _userRepository.GetAgentsAsync(pageNumber, pageSize);

        if (agents == null || !agents.Any())
        {
            throw new NotFoundException(message: $"No agents found. Page number: {pageNumber}, Page size: {pageSize}", title: "No agents found.");
        }

        var totalCount = await _userRepository.GetTotalCountAsync();

        var agentsDto = _mapper.Map<IEnumerable<SearchAgentResponseDto>>(agents);

        return new PagedResult<SearchAgentResponseDto>(pageNumber, pageSize, totalCount, agentsDto);
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

    public async Task<IEnumerable<int>> GetUserListingCaseIdsAsync(string currentUserId, string userRole)
    {
        // If the user is a photographyCompany
        // Check if the photography company exists
        if (userRole == RoleNames.PhotographyCompany)
        {
            var photographyCompany = await _userRepository.FindPhotographyCompanyByIdAsync(currentUserId);
            if (photographyCompany is null)
            {
                throw new NotFoundException(message: $"Photography company {currentUserId} does not exist", title: "Photography company does not exist");
            }

            return await _userRepository.GetPhotographyCompanyListingCaseIdsAsync(currentUserId);
        }

        // If the user is an agent
        // Check if the agent exists
        var agent = await _userRepository.FindAgentByIdAsync(currentUserId);
        if (agent is null)
        {
            throw new NotFoundException(message: $"Agent {currentUserId} does not exist", title: "Agent does not exist");
        }

        return await _userRepository.GetAgentListingCaseIdsAsync(currentUserId);
    }

    public async Task UpdatePasswordAsync(UpdatePasswordRequestDto updatePasswordRequestDto, string userId)
    {
        // Check if the user exists
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            throw new NotFoundException(message: $"User {userId} does not exist", title: "User does not exist");
        }

        // Check if the old password is correct
        var checkPasswordResult = await _userManager.CheckPasswordAsync(user, updatePasswordRequestDto.OldPassword);
        if (!checkPasswordResult)
        {
            throw new UnauthorizedException(message: $"Old password is incorrect", title: "Old password is incorrect");
        }

        // Update password
        var result = await _userManager.ChangePasswordAsync(user, updatePasswordRequestDto.OldPassword, updatePasswordRequestDto.NewPassword);
        if (!result.Succeeded)
        {
            var errors = string.Join("| ", result.Errors.Select(x => x.Description));

            throw new DbErrorException(message: $"Failed to update password with errors: {errors}", title: "Failed to update password");
        }
    }
}
