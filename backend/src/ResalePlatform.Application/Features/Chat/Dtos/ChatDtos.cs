namespace ResalePlatform.Application.Features.Chat.Dtos;

public class MessageDto
{
    public Guid Id { get; init; }
    public Guid ConversationId { get; init; }
    public Guid SenderId { get; init; }
    public string Text { get; init; } = null!;
    public bool IsRead { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
}

public class ConversationDto
{
    public Guid Id { get; init; }
    public Guid ListingId { get; init; }
    public string ListingTitle { get; init; } = null!;
    public string? ListingImageUrl { get; init; }
    public Guid OtherUserId { get; set; }
    public string OtherUserName { get; set; } = "—";
    public string? LastMessageText { get; init; }
    public DateTimeOffset LastMessageAt { get; init; }
}
