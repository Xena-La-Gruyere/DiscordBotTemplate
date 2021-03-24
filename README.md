# Simple C# Discord Bot
Basis to create your .net framework 5.0 Discord Bot.

### This bot include :
*  [Discord.Net](https://www.nuget.org/packages/Discord.Net/) (see documentation [here](https://docs.stillu.cc/guides/introduction/intro.html))
*  Sample code to organize services and modules
*  Dockerization (``Dockerfile`` and ``docker-compose.yml``)
*  Logging with [Serilog](https://www.nuget.org/packages/Serilog.AspNetCore/4.0.1-dev-00219)

# Usage

## Bot Creation

Go [here](https://discord.com/login?redirect_to=%2Fdevelopers) and create your application with a bot.

Once your bot created, you should copy its token and use it to configure your bot service (see below).

## Using docker

Edit ``docker-compose.yml`` or create a ``.env`` file to configure your bot.

Then simply use
```bash
docker-compose up
```

Use a docker service with linux container (use [WSL 2](https://docs.microsoft.com/en-us/windows/wsl/install-win10) if you are operating a Windows system).

## Manually

Edit ``appsettings.json`` to configure your bot.

Build discord bot with
```bash
dotnet build src/DiscordBot/DiscordBot.csproj --configuration Release --framework net5.0
```
then launch it with
```bash
dotnet run ./src/DiscordBot/bin/Release/net5.0/DiscordBot.dll
```
or for Windows
```cmd
.\src\DiscordBot\bin\Release\net5.0\DiscordBot.exe
```
