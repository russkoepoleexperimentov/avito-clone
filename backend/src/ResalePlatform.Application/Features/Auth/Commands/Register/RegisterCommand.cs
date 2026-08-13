using MediatR;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Commands.Register;

public record RegisterCommand(string Email, string Password, string DisplayName)
    : IRequest<AuthResponse>;
