using Discord.Addons.Hosting;
using Discord.Addons.Hosting.Util;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Discord.NET_Sharded_Template.Services
{
    class CommandHandler : DiscordShardedClientService
    {
        private readonly IServiceProvider provider;
        private readonly CommandService commandService;
        private readonly IConfiguration configuration;

        public CommandHandler(DiscordShardedClient client, ILogger<CommandHandler> logger, IServiceProvider provider, CommandService commandService, IConfiguration configuration)
            : base(client, logger)
        {
            this.provider = provider;
            this.commandService = commandService;
            this.configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Client.MessageReceived += OnMessageReceived;
            this.commandService.CommandExecuted += OnCommandExecuted;
            await this.commandService.AddModulesAsync(Assembly.GetEntryAssembly(), this.provider);

            await this.Client.WaitForReadyAsync(stoppingToken);
            Logger.LogInformation($"Client is ready, running on {this.Client.Shards} shard(s).");
        }

        private async Task OnMessageReceived(SocketMessage socketMessage)
        {
            if (socketMessage is not SocketUserMessage message || message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;
            if (!message.HasStringPrefix(this.configuration["prefix"], ref argPos) && !message.HasMentionPrefix(this.Client.CurrentUser, ref argPos)) return;

            var context = new ShardedCommandContext(this.Client, message);
            await this.commandService.ExecuteAsync(context, argPos, this.provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> commandInfo, ICommandContext commandContext, IResult result)
        {
            if (!commandInfo.IsSpecified || result.IsSuccess)
            {
                return;
            }

            await commandContext.Channel.SendMessageAsync($"Error: {result}");
        }
    }
}
