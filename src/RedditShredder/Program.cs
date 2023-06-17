using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Autofac.Features.ResolveAnything;
using CommandLine;
using MediatR.Extensions.Autofac.DependencyInjection;
using MediatR.Extensions.Autofac.DependencyInjection.Builder;
using NLog;
using NLog.Config;
using NLog.Targets;
using Reddit;
using RedditShredder.Generation;
using RedditShredder.Notifications;

namespace RedditShredder
{
    internal class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static async Task Main(string[] args)
        {
            try
            {
                await Parser.Default.ParseArguments<Options>(args)
                            .WithParsedAsync(x => MainImpl(x));
            }
            finally
            {
                LogManager.Shutdown();
            }
        }

        private static async Task MainImpl(Options options, CancellationToken cancellationToken = default)
        {
            LogManager.Configuration = BuildLoggingConfiguration(options);
            var container = BuildContainer(options);

            var workerCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            var workerCount = Environment.ProcessorCount;
            var workers = Enumerable.Range(1, workerCount)
                                    .Select(x => container.Resolve<Worker>().Run(x, workerCancellationTokenSource.Token))
                                    .ToArray();
            var queue = container.Resolve<WorkQueue>();

            Logger.Info($"Starting work with {workerCount} workers...");
            await queue.Push(new Started(), cancellationToken);

            await queue.WaitForCompletion(cancellationToken);

            Logger.Debug("Work appears complete, shutting down workers...");
            workerCancellationTokenSource.Cancel();
            await Task.WhenAll(workers);

            Logger.Info("All done, bye.");
        }

        private static LoggingConfiguration BuildLoggingConfiguration(Options options)
        {
            var configuration = new LoggingConfiguration();

            var level = options.Verbose ? LogLevel.Trace : LogLevel.Info;

            var target = new ColoredConsoleTarget
            {
                Layout = "${level:uppercase=true}: ${message:withexception=true}"
            };
            configuration.AddRule(level, LogLevel.Fatal, target);
            return configuration;
        }

        private static IContainer BuildContainer(Options options)
        {
            var builder = new ContainerBuilder();

            var configuration = MediatRConfigurationBuilder
                                .Create(typeof(Program).Assembly)
                                .WithAllOpenGenericHandlerTypesRegistered()
                                .Build();

            builder.RegisterMediatR(configuration);

            builder.RegisterInstance(options).AsSelf().SingleInstance();
            builder.Register(context =>
                   {
                       var resolvedOptions = context.Resolve<Options>();
                       return new RedditClient(
                           appId: resolvedOptions.ClientId,
                           appSecret: resolvedOptions.ClientSecret,
                           refreshToken: resolvedOptions.RefreshToken
                       );
                   })
                   .AsSelf()
                   .SingleInstance();

            builder.RegisterType<WorkQueue>().AsSelf().SingleInstance();
            builder.RegisterType<WordList>().AsSelf().SingleInstance();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            var container = builder.Build();
            return container;
        }
    }
}
