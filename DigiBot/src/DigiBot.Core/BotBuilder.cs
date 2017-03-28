using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace DigiBot
{
    public class BotBuilder : IBotBuilder
    {
        private readonly List<Action<Registry>> _configureServicesDelegates;

        private bool _isBuilt = false;

        public BotBuilder()
        {
            _configureServicesDelegates = new List<Action<Registry>>();
        }
        
        public IDigiBot Build()
        {
            if(_isBuilt)
            {
                throw new InvalidOperationException("Bot has already been built.");
            }
            _isBuilt = true;

            var services = BuildServices();


            Console.WriteLine("Creating Bot...");
            var bot = new DigiBot(services, new Container(services));

            Console.WriteLine("Initializing...");
            bot.Initialize();

            return bot;
        }

        public IBotBuilder UseInitializer<TInit>() where TInit : class
        {
            return ConfigureServices(services =>
            {
                Console.WriteLine($"Setting up initializer: {typeof(TInit).FullName}");
                if (typeof(IInitializer).GetTypeInfo().IsAssignableFrom(typeof(TInit)))
                {
                    services.ForSingletonOf(typeof(IInitializer)).Use(typeof(TInit));
                }
                else
                {
                    services.ForSingletonOf(typeof(IInitializer)).Use(context => new ConventionBasedInitializer(InitializerLoader.LoadMethods(context, typeof(TInit))));
                }
            });
        }

        public IBotBuilder ConfigureServices(Action<Registry> configureServices)
        {
            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        private Registry BuildServices()
        {
            Console.WriteLine("Building Services...");
            var services = new Registry();

            services.For<IBotAppBuilderFactory>().Use<BotAppBuilderFactory>();

            foreach (var config in _configureServicesDelegates)
            {
                config(services);
            }

            services.Scan(scanner =>
            {
                scanner.AssembliesAndExecutablesFromApplicationBaseDirectory();
                scanner.AddAllTypesOf<IBotFilter>();
            });

            services.Scan(scanner =>
            {
                scanner.TheCallingAssembly();
                scanner.AddAllTypesOf<IMiddleware>();
            });

            //services.For<IMiddleware>().ContainerScoped();

            return services;
        }

    }
}
