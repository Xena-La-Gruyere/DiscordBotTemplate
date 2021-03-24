using System;
using System.Linq;
using System.Text;
using Discord.Commands;
using DiscordBot.Models;
using Microsoft.Extensions.Options;

namespace DiscordBot.Services
{
    public class HelpService
    {
        private readonly CommandService _commandService;
        private readonly IOptions<DiscordOptions> _options;

        public HelpService(CommandService commandService,
            IOptions<DiscordOptions> options)
        {
            _commandService = commandService;
            _options = options;
        }

        public string Help()
        {
            var sb = new StringBuilder();

            var prefix = _options.Value.CommandPrefix;
            foreach (var command in _commandService.Commands)
            {
                sb.AppendFormat("{0}{1} ", prefix, command.Name);

                foreach (var parameter in command.Parameters)
                {
                    sb.AppendFormat("<{0}> ", parameter.Name);
                }
                sb.AppendFormat(": {0}", command.Summary).AppendLine();
            }

            return sb.ToString();
        }

        public string Man(string commandName)
        {
            var command = _commandService.Commands.FirstOrDefault(c =>
                c.Name.Equals(commandName, StringComparison.InvariantCultureIgnoreCase));

            if (command is null)
            {
                return $"Unknown command : {commandName}";
            }

            var sb = new StringBuilder();

            sb.Append("Aliases : ")
                .AppendLine(string.Join(", ", command.Aliases))
                .AppendLine("Summary : ")
                .AppendLine(command.Summary);

            if (command.Parameters.Any())
            {
                sb.Append("Parameters : ");

                foreach (var parameter in command.Parameters)
                {
                    sb.AppendFormat("<{0}:{1}> {2}", parameter.Type.Name, parameter.Name, parameter.Summary)
                        .AppendLine();
                }
            }
            
            return sb.ToString();
        }
    }
}
