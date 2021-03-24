FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src

# copy restore and publish app and libraries
COPY src/discordbot/ .
RUN dotnet publish DiscordBot/DiscordBot.csproj --configuration Release --framework net5.0 --output /app --runtime linux-x64 /property:PublishSingleFile=True /property:DebugType=None /property:DebugSymbols=False

# final stage/image
FROM mcr.microsoft.com/dotnet/runtime:5.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["./DiscordBot"]