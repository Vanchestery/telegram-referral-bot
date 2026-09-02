using ReferralBot.Core.Models;
using ReferralBot.Models;
using ReferralBot.Pages;

namespace ReferralBot.Services;

/// <summary>
/// Converts between TelegramUserContext (in-memory bot object)
/// and TelegramBotUserState (domain model persisted in the DB).
///
/// Called from BotService on every update:
///   load: State (DB) → Context (memory)
///   save: Context (memory) → State (DB)
/// </summary>
public class TelegramUserContextConverter(
    PageStackConverter pageStackConverter,
    ILogger<TelegramUserContextConverter> logger)
{
    public TelegramUserContext ToContext(TelegramBotUserState? state)
    {
        if (state is null)
            return new TelegramUserContext { TelegramId = 0 };

        try
        {
            return new TelegramUserContext
            {
                TelegramId = state.TelegramUserId,
                Pages = pageStackConverter.ToStack(state.PageNames),
                LastMessage = new TelegramBotMessageDto(state.CurrentMessageId, state.IsMediaContent),
                IsWelcomeMessageSent = state.IsWelcomeMessageSent,
                SelectedCourseId = state.SelectedCourseId
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to convert state to context for TelegramUserId: {Id}", state.TelegramUserId);
            return new TelegramUserContext { TelegramId = state.TelegramUserId };
        }
    }

    public TelegramBotUserState ToState(TelegramUserContext context)
    {
        return new TelegramBotUserState
        {
            TelegramUserId = context.TelegramId,
            PageNames = pageStackConverter.ToIds(context.Pages),
            CurrentMessageId = context.LastMessage?.TelegramMessageId ?? 0,
            IsMediaContent = context.LastMessage?.IsMedia ?? false,
            IsWelcomeMessageSent = context.IsWelcomeMessageSent,
            SelectedCourseId = context.SelectedCourseId
        };
    }
}
