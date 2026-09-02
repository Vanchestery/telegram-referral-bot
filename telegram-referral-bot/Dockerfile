FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY telegram-referral-bot.slnx ./
COPY ReferralBot.Core/ReferralBot.Core.csproj ReferralBot.Core/
COPY ReferralBot.Db/ReferralBot.Db.csproj ReferralBot.Db/
COPY telegram-referral-bot/telegram-referral-bot.csproj telegram-referral-bot/
COPY ReferralBot.Tests/ReferralBot.Tests.csproj ReferralBot.Tests/

RUN dotnet restore telegram-referral-bot.slnx

COPY . .
RUN dotnet publish telegram-referral-bot/telegram-referral-bot.csproj -c Release -o /app --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app .
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080
ENTRYPOINT ["dotnet", "telegram-referral-bot.dll"]
