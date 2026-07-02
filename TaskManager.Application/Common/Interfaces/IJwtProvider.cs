using TaskManager.Domain.Entities;

namespace TaskManager.Application.Common.Interfaces;

public interface IJwtProvider
{
    string Generate(User user);
}