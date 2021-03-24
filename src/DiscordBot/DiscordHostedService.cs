using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using DiscordBot.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DiscordBot
{
    public class DiscordHostedService : IHostedService
    {
        private readonly DiscordShardedClient _discordClient;
        private readonly CommandService _commandService;
        private readonly IOptions<DiscordOptions> _discordOptions;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<DiscordShardedClient> _discordLogger;
        private readonly ILogger<DiscordHostedService> _logger;

        public DiscordHostedService(
            DiscordShardedClient discordClient,
            CommandService commandService,
            IOptions<DiscordOptions> discordOptions,
            IServiceProvider serviceProvider,
            ILogger<DiscordShardedClient> discordLogger,
            ILogger<DiscordHostedService> logger)
        {
            _discordClient = discordClient;
            _commandService = commandService;
            _discordOptions = discordOptions;
            _serviceProvider = serviceProvider;
            _discordLogger = discordLogger;
            _logger = logger;
        }

        private readonly Dictionary<LogSeverity, LogLevel> _logLevel = new Dictionary<LogSeverity, LogLevel>
        {
            [LogSeverity.Critical] = LogLevel.Critical,
            [LogSeverity.Debug] = LogLevel.Debug,
            [LogSeverity.Error] = LogLevel.Error,
            [LogSeverity.Info] = LogLevel.Information,
            [LogSeverity.Verbose] = LogLevel.Trace,
            [LogSeverity.Warning] = LogLevel.Warning
        };

        private Task DiscordLog(LogMessage message)
        {
            _discordLogger.Log(_logLevel[message.Severity], message.Exception, message.Message);
            return Task.CompletedTask;
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            var message = messageParam as SocketUserMessage;
            if (message == null) return;

            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasCharPrefix(_discordOptions.Value.CommandPrefix, ref argPos) ||
                message.HasMentionPrefix(_discordClient.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            // Create a Shared command context based on the message
            var context = new ShardedCommandContext(_discordClient, message);

            // Execute the command with the command context we just
            // created, along with the service provider for precondition checks.
            await _commandService.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _serviceProvider);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Discord Hosted Service : Starting ...");

            var token = _discordOptions.Value.Token;
            if (string.IsNullOrWhiteSpace(token))
            {
                const string missingToken = "Discord Token cannot be null or empty.";
                _logger.LogError(missingToken);
                throw new Exception(missingToken);
            }

            _discordClient.Log += DiscordLog;
            _discordClient.MessageReceived += HandleCommandAsync;

            await _discordClient.LoginAsync(TokenType.Bot, token);
            await _discordClient.StartAsync();

            await _commandService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);

            _logger.LogDebug("Discord Hosted Service : Started");
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Discord Hosted Service : Stopping ...");

            _discordClient.MessageReceived -= HandleCommandAsync;

            await _discordClient.LogoutAsync();

            _discordClient.Log -= DiscordLog;

            _logger.LogDebug("Discord Hosted Service : Stopped");
        }
    }
}
