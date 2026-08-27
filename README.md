# telegram-referral-bot

Telegram-бот партнёрской программы для онлайн-курсов: каталог, реферальные ссылки, бонусы и личный кабинет партнёра.

> Work in progress.

**Стек:** .NET 10 (LTS) · ASP.NET Core Web API · EF Core · PostgreSQL · Telegram.Bot · Polly · Serilog · xUnit

## Архитектура

| Проект | Назначение |
|--------|------------|
| `ReferralBot.Core` | Доменные модели, интерфейсы, сервисы |
| `ReferralBot.Db` | EF Core, entities, storage, миграции |
| `telegram-referral-bot` | Web host: webhook, REST, page engine |
| `ReferralBot.Tests` | Unit-тесты |

## License

MIT — см. [LICENSE](LICENSE).
