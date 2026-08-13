using MediatR;
using ResalePlatform.Application.Features.Auth.Dtos;

namespace ResalePlatform.Application.Features.Auth.Commands.Refresh;

public record RefreshCommand(string RefreshToken) : IRequest<AuthResponse>;
