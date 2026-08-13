using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResalePlatform.Application.Features.Chat.Commands.SendMessage;
using ResalePlatform.Application.Features.Chat.Commands.StartConversation;
using ResalePlatform.Application.Features.Chat.Dtos;
using ResalePlatform.Application.Features.Chat.Queries.GetMessages;
using ResalePlatform.Application.Features.Chat.Queries.GetMyConversations;

namespace ResalePlatform.API.Controllers;

[ApiController]
[Route("api/chat")]
[Authorize]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;

    public ChatController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>Открыть/найти диалог с продавцом по объявлению.</summary>
    [HttpPost("conversations")]
    public async Task<ActionResult> Start(StartConversationCommand command)
    {
        var id = await _mediator.Send(command);
        return Ok(new { conversationId = id });
    }

    /// <summary>Диалоги текущего пользователя.</summary>
    [HttpGet("conversations")]
    public async Task<ActionResult<IReadOnlyList<ConversationDto>>> GetConversations()
        => Ok(await _mediator.Send(new GetMyConversationsQuery()));

    /// <summary>История сообщений диалога.</summary>
    [HttpGet("conversations/{id:guid}/messages")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessages(Guid id)
        => Ok(await _mediator.Send(new GetMessagesQuery(id)));

    /// <summary>Отправить сообщение в диалог.</summary>
    [HttpPost("conversations/{id:guid}/messages")]
    public async Task<ActionResult<MessageDto>> Send(Guid id, [FromBody] SendMessageBody body)
        => Ok(await _mediator.Send(new SendMessageCommand(id, body.Text)));

    public record SendMessageBody(string Text);
}
