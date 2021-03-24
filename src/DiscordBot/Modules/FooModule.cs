using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using DiscordBot.Services;

namespace DiscordBot.Modules
{
    public class FooModule : ModuleBase
    {
        private readonly FooService _fooService;
        private readonly HelpService _helpService;

        public FooModule(FooService fooService,
            HelpService helpService)
        {
            _fooService = fooService;
            _helpService = helpService;
        }

        [Command("add")]
        [Alias("a")]
        [Summary("Add two numbers.")]
        public Task Add([Summary("first argument")] int a, [Summary("second argument")] int b)
        {
            var result = _fooService.Add(a, b);

            return ReplyAsync($"{a} + {b} = {result}");
        }

        [Command("man")]
        [Summary("Command manual page.")]
        public Task Man(string command)
        {
            return ReplyAsync(_helpService.Man(command));
        }

        [Command("help")]
        [Summary("Show all command and simple usage.")]
        public Task Help()
        {
            return ReplyAsync(_helpService.Help());
        }

        [Command("version")]
        [Summary("Show bot version.")]
        public Task Version()
        {
            var sb = new StringBuilder().Append("Version : ").Append(GetType().Assembly.GetName().Version);

            return ReplyAsync(sb.ToString());
        }
    }
}
