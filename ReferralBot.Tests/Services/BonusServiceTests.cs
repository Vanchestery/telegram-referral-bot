using Microsoft.Extensions.Logging.Abstractions;

using ReferralBot.Core.Interfaces;
using ReferralBot.Core.Models;
using ReferralBot.Core.Services;
using ReferralBot.Db.Entities;
using ReferralBot.Db.Interfaces;

namespace ReferralBot.Tests.Services;

public class BonusServiceTests
{
    private static BonusService CreateSut(
        IAccountsStorage accounts,
        IBonusTransactionStorage transactions,
        IPartnerService? partners = null) =>
        new(
            accounts,
            transactions,
            partners ?? Mock.Of<IPartnerService>(),
            NullLogger<BonusService>.Instance);

    private static PaymentNotification Payment(int transactionId = 42) => new()
    {
        TransactionId = transactionId,
        CourseId = 107,
        UserId = 1001,
        Amount = 10_000,
        PaymentTime = DateTime.UtcNow
    };

    [Fact]
    public async Task ProcessPayment_DuplicateTransaction_ReturnsTrue_DoesNotAccrue()
    {
        var transactions = new Mock<IBonusTransactionStorage>();
        transactions
            .Setup(s => s.GetByPaymentTransactionIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new BonusTransactionEntity { PaymentTransactionId = 42 });

        var accounts = new Mock<IAccountsStorage>();
        var sut = CreateSut(accounts.Object, transactions.Object);

        var result = await sut.ProcessPaymentNotificationAsync(Payment());

        result.Should().BeTrue();
        transactions.Verify(
            s => s.AddAsync(It.IsAny<BonusTransactionEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
        accounts.Verify(
            s => s.UpsertAsync(It.IsAny<AccountEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ProcessPayment_WithReferrer_AccruesInternRate()
    {
        var referrerId = Guid.NewGuid();
        var buyer = new AccountEntity
        {
            Id = Guid.NewGuid(),
            TelegramUserId = 1001,
            ReferrerId = referrerId
        };
        var referrer = new AccountEntity
        {
            Id = referrerId,
            TelegramUserId = 2002,
            BonusBalance = 0,
            InvitedPurchasesCount = 0,
            TotalBonusEarned = 0
        };

        var accounts = new Mock<IAccountsStorage>();
        accounts.Setup(s => s.GetByTelegramUserIdAsync(1001, It.IsAny<CancellationToken>()))
            .ReturnsAsync(buyer);
        accounts.Setup(s => s.GetByIdAsync(referrerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(referrer);

        var transactions = new Mock<IBonusTransactionStorage>();
        transactions
            .Setup(s => s.GetByPaymentTransactionIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BonusTransactionEntity?)null);

        var partners = new Mock<IPartnerService>();
        var sut = CreateSut(accounts.Object, transactions.Object, partners.Object);

        var result = await sut.ProcessPaymentNotificationAsync(Payment());

        result.Should().BeTrue();
        referrer.BonusBalance.Should().Be(1500);
        referrer.InvitedPurchasesCount.Should().Be(1);
        transactions.Verify(
            s => s.AddAsync(It.Is<BonusTransactionEntity>(t => t.Amount == 1500 && t.PaymentTransactionId == 42),
                It.IsAny<CancellationToken>()),
            Times.Once);
        partners.Verify(p => p.InvalidateProfileCache(2002), Times.Once);
    }

    [Fact]
    public async Task ProcessPayment_NoReferrer_ReturnsTrue_DoesNotAccrue()
    {
        var buyer = new AccountEntity
        {
            Id = Guid.NewGuid(),
            TelegramUserId = 1001,
            ReferrerId = null
        };

        var accounts = new Mock<IAccountsStorage>();
        accounts.Setup(s => s.GetByTelegramUserIdAsync(1001, It.IsAny<CancellationToken>()))
            .ReturnsAsync(buyer);

        var transactions = new Mock<IBonusTransactionStorage>();
        transactions
            .Setup(s => s.GetByPaymentTransactionIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((BonusTransactionEntity?)null);

        var sut = CreateSut(accounts.Object, transactions.Object);

        var result = await sut.ProcessPaymentNotificationAsync(Payment());

        result.Should().BeTrue();
        transactions.Verify(
            s => s.AddAsync(It.IsAny<BonusTransactionEntity>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }
}
