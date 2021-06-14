using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Threading.Tasks;

namespace Discord.NET_Sharded_Template.Modules
{
    public class Example : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<Example> logger;

        public Example(ILogger<Example> logger)
        {
            this.logger = logger;
        }

        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
            this.logger.LogInformation($"{Context.User.Username} ran the ping command!");
        }

        [Command("shard")]
        public async Task ShardsAsync()
        {
            await this.ReplyAsync($"Running on shard {this.Context.Client.ShardId}.");
        }

        [Command("echo")]
        public async Task EchoAsync([Remainder] string text)
        {
            await this.ReplyAsync(text);
            this.logger.LogInformation($"{Context.User.Username} ran the echo command!");
        }

        [Command("math")]
        public async Task MathAsync([Remainder] string math)
        {
            var dt = new DataTable();
            var result = dt.Compute(math, null);

            await this.ReplyAsync($"Result: {result}");
            this.logger.LogInformation($"{Context.User.Username} ran the math command!");
        }
    }
}
