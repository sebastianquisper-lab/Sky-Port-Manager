namespace Sky.Application.DTOs;

public sealed record LoginResponseDto(
    string AccessToken,
    DateTime ExpiresAtUtc,
    int UserId,
    string Username,
    string Role,
    int? EmployeeId);
