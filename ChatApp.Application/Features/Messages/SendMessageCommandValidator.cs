using FluentValidation;

namespace ChatApp.Application.Features.Messages;

public class SendMessageCommandValidator : AbstractValidator<SendMessageCommand>
{
    public SendMessageCommandValidator()
    {
        RuleFor(v => v.Content)
            .NotEmpty().WithMessage("Message content cannot be empty.")
            .MaximumLength(500).WithMessage("Message cannot exceed 500 characters.");

        RuleFor(v => v.ChatRoomId)
            .NotEmpty().WithMessage("A valid chat room must be specified.");
            
        RuleFor(v => v.UserId)
            .NotEmpty().WithMessage("A valid user must be specified.");
    }
}