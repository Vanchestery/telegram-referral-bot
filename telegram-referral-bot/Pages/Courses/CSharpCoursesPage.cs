using ReferralBot.Models;
using ReferralBot.Pages.PageResults;
using ReferralBot.Services;

using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace ReferralBot.Pages.Courses;

public class CSharpCoursesPage(
    PageCreator pageCreator,
    ICourseService courseService) : CallbackQueryPageBase
{
    protected override async Task<string> GetRawContentAsync(TelegramUserContext context)
    {
        var courses = await courseService.GetCoursesIdTitleAsync();
        return courses.Count == 0
            ? "Курсы пока недоступны. Попробуйте позже."
            : "Выберите интересующий курс:";
    }

    public override async Task<ButtonLinqPage[][]> GetKeyboardAsync(TelegramUserContext context)
    {
        var courses = await courseService.GetCoursesIdTitleAsync();

        var rows = courses
            .Select(c => new ButtonLinqPage(
                InlineKeyboardButton.WithCallbackData(ToButtonText(c.Title), c.Id.ToString()),
                pageCreator.CreatePage<CoursePage>()))
            .Chunk(2)
            .ToList();

        rows.Add(
        [
            new ButtonLinqPage(
                InlineKeyboardButton.WithCallbackData("Назад ⬅️"),
                pageCreator.CreatePage<BackwardDummyPage>())
        ]);

        return [.. rows];
    }

    /// <summary>
    /// Клик по курсу: id в CallbackData. Сохраняем выбор в контекст,
    /// кладём карточку в стек и возвращаем её View напрямую.
    /// «Назад» (нечисловой CallbackData) уходит в базовую обработку.
    /// </summary>
    public override async Task<PageResultBase> HandleAsync(Update update, TelegramUserContext context)
    {
        if (update.CallbackQuery?.Data is not string data || !int.TryParse(data, out var courseId))
            return await base.HandleAsync(update, context);

        context.SelectedCourseId = courseId;

        var coursePage = pageCreator.CreatePage<CoursePage>();
        context.AddPage(coursePage);
        return await coursePage.ViewAsync(update, context);
    }

    /// <summary>
    /// Telegram отклоняет inline-кнопку длиннее 64 символов — обрезаем название курса.
    /// </summary>
    private static string ToButtonText(string title)
    {
        const int max = 64;
        if (string.IsNullOrWhiteSpace(title))
            return "Курс";

        return title.Length <= max ? title : title[..(max - 1)] + "…";
    }
}
