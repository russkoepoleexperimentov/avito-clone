using MediatR;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
