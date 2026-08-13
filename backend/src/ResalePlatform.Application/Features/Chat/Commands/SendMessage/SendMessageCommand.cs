using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ResalePlatform.Application.Common.Exceptions;
using ResalePlatform.Application.Common.Interfaces;
using ResalePlatform.Application.Features.Chat.Dtos;
using ResalePlatform.Domain.Entities;

namespace ResalePlatform.Application.Features.Chat.Commands.SendMessage;

public record SendMessageCommand(Guid ConversationId, string Text) : IRequest<MessageDto>;

public class SendMessageValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Text).NotEmpty().MaximumLength(2000);
    }
}

public class SendMessageHandler : IRequestHandler<SendMessageCommand, MessageDto>
{
    private readonly IApplicationDbContext _db;
    private readonly ICurrentUser _currentUser;
    private readonly IChatNotifier _notifier;

    public SendMessageHandler(
        IApplicationDbContext db, ICurrentUser currentUser, IChatNotifier notifier)
    {
        _db = db;
        _currentUser = currentUser;
        _notifier = notifier;
    }

    public async Task<MessageDto> Handle(SendMessageCommand request, CancellationToken ct)
    {
        var userId = _currentUser.UserId
            ?? throw new UnauthorizedException("Требуется авторизация.");

        var conversation = await _db.Conversations
            .FirstOrDefaultAsync(c => c.Id == request.ConversationId, ct)
            ?? throw new NotFoundException("Диалог не найден.");

        if (conversation.BuyerId != userId && conversation.SellerId != userId)
            throw new ForbiddenException("Вы не участник этого диалога.");

        var now = DateTimeOffset.UtcNow;
        var message = new Message
        {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = userId,
            Text = request.Text,
            IsRead = false,
            CreatedAt = now,
        };
        _db.Messages.Add(message);
        conversation.LastMessageAt = now;
        await _db.SaveChangesAsync(ct);

        var dto = new MessageDto
        {
            Id = message.Id,
            ConversationId = message.ConversationId,
            SenderId = message.SenderId,
            Text = message.Text,
            IsRead = message.IsRead,
            CreatedAt = message.CreatedAt,
        };

        // Реалтайм-доставка второму участнику.
        var recipientId = conversation.BuyerId == userId
            ? conversation.SellerId
            : conversation.BuyerId;
        await _notifier.MessageReceivedAsync(recipientId, dto, ct);

        return dto;
    }
}
