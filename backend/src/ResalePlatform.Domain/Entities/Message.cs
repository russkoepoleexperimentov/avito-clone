namespace ResalePlatform.Domain.Entities;

/// <summary>Сообщение внутри диалога (этап 2).</summary>
public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; } = null!;

    public Guid SenderId { get; set; }

    public string Text { get; set; } = null!;

    public bool IsRead { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
