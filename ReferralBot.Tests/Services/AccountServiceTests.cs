using AutoMapper;

using Microsoft.Extensions.Logging.Abstractions;

using ReferralBot.Core.Mappings;
using ReferralBot.Core.Services;
using ReferralBot.Db.Entities;
using ReferralBot.Db.Interfaces;

namespace ReferralBot.Tests.Services;

public class AccountServiceTests
{
    private static AccountService CreateSut(IAccountsStorage storage)
    {
        var mapper = new MapperConfiguration(cfg => cfg.AddProfile<AccountProfile>()).CreateMapper();
        return new AccountService(storage, mapper, NullLogger<AccountService>.Instance);
    }

    [Fact]
    public async Task AddReferrerId_WhenMissing_SetsAndReturnsTrue()
    {
        var telegramUserId = 1001L;
        var referrerId = Guid.NewGuid();
        var entity = new AccountEntity { Id = Guid.NewGuid(), TelegramUserId = telegramUserId };

        var storage = new Mock<IAccountsStorage>();
        storage.Setup(s => s.GetByTelegramUserIdAsync(telegramUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var sut = CreateSut(storage.Object);

        var added = await sut.AddReferrerIdByTelegramIdAsync(telegramUserId, referrerId);

        added.Should().BeTrue();
        entity.ReferrerId.Should().Be(referrerId);
        storage.Verify(s => s.UpsertAsync(entity, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddReferrerId_WhenAlreadySet_DoesNotOverwrite()
    {
        var telegramUserId = 1001L;
        var original = Guid.NewGuid();
        var entity = new AccountEntity
        {
            Id = Guid.NewGuid(),
            TelegramUserId = telegramUserId,
            ReferrerId = original
        };

        var storage = new Mock<IAccountsStorage>();
        storage.Setup(s => s.GetByTelegramUserIdAsync(telegramUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(entity);

        var sut = CreateSut(storage.Object);

        var added = await sut.AddReferrerIdByTelegramIdAsync(telegramUserId, Guid.NewGuid());

        added.Should().BeFalse();
        entity.ReferrerId.Should().Be(original);
        storage.Verify(s => s.UpsertAsync(It.IsAny<AccountEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AddReferrerId_AccountMissing_ReturnsFalse()
    {
        var storage = new Mock<IAccountsStorage>();
        storage.Setup(s => s.GetByTelegramUserIdAsync(It.IsAny<long>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccountEntity?)null);

        var sut = CreateSut(storage.Object);

        var added = await sut.AddReferrerIdByTelegramIdAsync(1001, Guid.NewGuid());

        added.Should().BeFalse();
        storage.Verify(s => s.UpsertAsync(It.IsAny<AccountEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
