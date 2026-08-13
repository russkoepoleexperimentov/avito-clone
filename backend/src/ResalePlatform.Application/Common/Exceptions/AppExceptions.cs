namespace ResalePlatform.Application.Common.Exceptions;

/// <summary>Ошибки валидации входных данных (400).</summary>
public class ValidationException : Exception
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Одно или несколько полей заполнены неверно.")
    {
        Errors = errors;
    }
}

/// <summary>Конфликт состояния, напр. email уже занят (409).</summary>
public class ConflictException : Exception
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>Неверные учётные данные или невалидный токен (401).</summary>
public class UnauthorizedException : Exception
{
    public UnauthorizedException(string message) : base(message) { }
}

/// <summary>Сущность не найдена (404).</summary>
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message) { }
}
