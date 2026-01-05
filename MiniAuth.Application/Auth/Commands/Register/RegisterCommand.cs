using MediatR;
using MiniAuth.Domain.Entities;

namespace MiniAuth.Application.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password): IRequest<User>;