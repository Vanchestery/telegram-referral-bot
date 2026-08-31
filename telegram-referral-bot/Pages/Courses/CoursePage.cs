using ReferralBot.Models;
using ReferralBot.Services;

using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages.Courses;

/// <summary>
/// Карточка курса: обложка + название/описание/цена + ссылки на Stepik.
/// Персональный промокод партнёра подключится в payment-api.
/// </summary>
public class CoursePage(
    PageCreator pageCreator,
    ICourseService courseService,
    ITelegramBotClient botClient,
    ILogger<CoursePage> logger) : CallbackQueryPageBase
{
    protected override async Task<string> GetRawContentAsync(TelegramUserContext context)
    {
        var course = await courseService.GetCourseInfoAsync(context.SelectedCourseId);
        if (course is null)
            return "Не удалось загрузить информацию о курсе. Попробуйте позже.";

        var summary = course.Summary.Length > 600
            ? course.Summary[..600].TrimEnd() + "…"
            : course.Summary;

        var price = course.Price > 0
            ? $"{course.Price:N0} ₽"
            : "цена не указана";

        return $"{course.Title}\n\n{summary}\n\nЦена: {price}";
    }

    protected override async Task<InputFile?> GetMediaContentAsync(TelegramUserContext context)
    {
        try
        {
            await botClient.SendChatAction(context.TelegramId, ChatAction.UploadPhoto);
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex, "Could not send upload-photo action for course {CourseId}", context.SelectedCourseId);
        }

        var course = await courseService.GetCourseInfoAsync(context.SelectedCourseId);
        if (!string.IsNullOrEmpty(course?.CoverUrl))
            return InputFile.FromUri(course.CoverUrl);

        var logo = await courseService.GetCourseLogoAsync(context.SelectedCourseId);
        return logo is { Length: > 0 }
            ? InputFile.FromStream(new MemoryStream(logo), $"course_{context.SelectedCourseId}.png")
            : null;
    }

    public override Task<ButtonLinqPage[][]> GetKeyboardAsync(TelegramUserContext context)
    {
        var courseId = context.SelectedCourseId;
        var courseUrl = $"https://stepik.org/a/{courseId}";
        var payUrl = $"https://stepik.org/a/{courseId}/pay";

        return Task.FromResult<ButtonLinqPage[][]>(
        [
            [new ButtonLinqPage(InlineKeyboardButton.WithUrl("Посмотреть курс", courseUrl))],
            [new ButtonLinqPage(InlineKeyboardButton.WithUrl("🛒 Купить со скидкой", payUrl))],
            [new ButtonLinqPage(
                InlineKeyboardButton.WithCallbackData("Назад ⬅️"),
                pageCreator.CreatePage<BackwardDummyPage>())],
        ]);
    }
}
