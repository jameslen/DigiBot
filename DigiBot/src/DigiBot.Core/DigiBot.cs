using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StructureMap;
using System.Threading;
using StructureMap.Graph.Scanning;

namespace DigiBot
{
    public class DigiBot : IDigiBot
    {
        private readonly Registry _botServiceRegistery;
        private IInitializer _initializer;

        private readonly IContainer _hostServices;

        private IContainer _botServices;
        private MessageDelegate _botMain;

        private IBotSource Source;

        public DigiBot(Registry botServices, IContainer hostServices)
        {
            _botServiceRegistery = botServices;
            _hostServices = hostServices;
        }

        public void Dispose()
        {
            
        }

        internal void Initialize()
        {
            if(_botMain == null)
            {
                _botMain = BuildBotApplication();
            }

        }

        public virtual void Start()
        {
            //throw new NotImplementedException();
            Console.WriteLine("Starting Bot...");

            Initialize();

            //IMessageFactory messageFactory = _botServices.GetInstance<IMessageFactory>();

            Source.Start(new BotHostApplication(_botMain, _botServices, null /*messageFactory*/)); // Access to things the Source might need

        }

        private MessageDelegate BuildBotApplication()
        {
            EnsureBotServices();
            EnsureBotSource();

            var builderFactory = _hostServices.GetInstance<IBotAppBuilderFactory>();
            var builder = builderFactory.Create(_botServices);

            var filters = _botServices.GetAllInstances<IBotFilter>();
            Action<IBotAppBuilder> configure = _initializer.Configure;

            foreach(var filter in filters.Reverse())
            {
                configure = filter.Configure(configure);
            }

            configure(builder);

            return builder.Build();
        }

        private void EnsureBotSource()
        {
            if(Source == null)
            {
                Source = _botServices.GetInstance<IBotSource>();
            }
        }

        private void EnsureBotServices()
        {
            if(_botServices == null)
            {
                EnsureStartUp();

                _botServices = _initializer.ConfigureServices(_botServiceRegistery);
            }
        }

        private void EnsureStartUp()
        {
            if(_initializer == null)
            {
                try
                {
                    _initializer = _hostServices.GetInstance<IInitializer>();
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"{ex.Message}");
                }
            }
        }
    }

    public static class DigiBotExtentions
    {
        public static void Run(this IDigiBot bot)
        {
            var complete = new ManualResetEventSlim(false);

            using (var tokenSource = new CancellationTokenSource())
            {
                Action close = () =>
                {
                    if (!tokenSource.IsCancellationRequested)
                    {
                        tokenSource.Cancel();
                    }

                    complete.Wait();
                };

                Console.CancelKeyPress += (sender, args) =>
                {
                    close();
                    args.Cancel = true;
                    Console.WriteLine("Closing...");
                };

                bot.Run(tokenSource.Token);

                complete.Set();
            }
        }

        private static void Run(this IDigiBot bot, CancellationToken token)
        {
            using (bot)
            {
                bot.Start();

                token.WaitHandle.WaitOne();
            }
        }
    }
}
