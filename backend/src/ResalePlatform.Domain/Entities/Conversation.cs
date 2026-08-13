namespace ResalePlatform.Domain.Entities;

/// <summary>Диалог покупателя и продавца по конкретному объявлению (этап 2).</summary>
public class Conversation
{
    public Guid Id { get; set; }

    public Guid ListingId { get; set; }
    public Listing Listing { get; set; } = null!;

    public Guid BuyerId { get; set; }
    public Guid SellerId { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>Время последнего сообщения — для сортировки списка диалогов.</summary>
    public DateTimeOffset LastMessageAt { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
