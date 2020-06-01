using System;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using MMA.ADS.ADSProvider;
using System.Runtime.CompilerServices;

namespace MMA.ADS.CSS.DLParsingService
{
    public class ParsingServiceProviderConfiguration : ADSProviderConfiguration
    {
    }

    public class ParsingServiceProviderManager : ADSProviderManager
    {
        // intentionally hide base implmentation 
        public static void InstantiateProviders(ProviderSettingsCollection pProviderSettings, ProviderCollection pProviders, Type pType)
        {
            myLogger.Debug("InstantiateProviders - entry");
            try
            {
                foreach (ProviderSettings settings in pProviderSettings)
                {
                    myLogger.Debug("Adding - adding provider");
                    pProviders.Add(ADSProviderManager.InstantiateProvider(settings, pType));
                    myLogger.Debug("InstantiateProviders - added provider");
                }
            }
            catch (Exception ex)
            {
                myLogger.Debug("InstantiateProviders caught exception - " + ex.Message);
                throw;
            }

            myLogger.Debug("InstantiateProviders - exit");
        }

    }

    public class ParsingServiceProviderCollection : ADSProviderCollection
    {

    }


    public class ParsingServiceProviderBase : ADSProviderBase
    {
        Dictionary<string, object> handledMessages;

        public override void Initialize(string name, NameValueCollection config)
        {
            log.Debug("Initialize() - Entry");
            base.Initialize(name, config);

            ConnectionStringsSection cs =
                (ConnectionStringsSection)ConfigurationManager.GetSection("connectionStrings");

            connectionString = string.Empty;
            connectionStringName = string.Empty;
            providerInvariantName = string.Empty;

            handledMessages = new Dictionary<string, object>();
            log.Debug("Initialize() - Exit");
        }

        protected String providerInvariantName { get; set; } // intentionally hide base implmentation 

        protected static bool isInitialized = false; // intentionally hide base implmentation 
        private static ParsingServiceProviderBase m_provider = null;
        private static ParsingServiceProviderCollection providerCollection;


        public void LogInfo(string infoStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0 )
        {
            log.Info(infoStr, memberName, sourceFilePath, sourceLineNumber );
        }

        public void LogDebug(string debugStr,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0 )
        {
            log.Debug(debugStr, memberName, sourceFilePath, sourceLineNumber );
        }

        public void LogError(string errorStr, 
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            log.Error(errorStr, memberName, sourceFilePath, sourceLineNumber );
        }

        public void LogError(string errorStr, Exception exception,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0
            )
        {
            log.Error(errorStr, exception, memberName, sourceFilePath, sourceLineNumber );
        }

        static ParsingServiceProviderBase() 
        {
            Initialize();
        } 
         /// <summary>
         /// Initialize the config and reading its contain and based on the setting loading the appropriate 
         /// provider.
         /// </summary>
        private static void Initialize()
        {
            log.Debug("ParsingServiceProviderBase.Initialize() - entry");
            try
            {

                ParsingServiceProviderConfiguration qc =
                    (ParsingServiceProviderConfiguration)ConfigurationManager.GetSection(ADSProviderConstants.CONFIG_GROUPSECTION_PROVIDER_NAME);

                providerCollection = new ParsingServiceProviderCollection();

                ParsingServiceProviderManager.InstantiateProviders(qc.Providers,
                providerCollection, typeof(ParsingServiceProviderBase));

                providerCollection.SetReadOnly();

                isInitialized = true; //error-free initialization 
                m_provider = providerCollection[qc.DefaultProvider] as ParsingServiceProviderBase;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                log.Error("Initialize Error: " + ex.Message.ToString());
            }
            log.Debug("MembershipProvider.Initialize() - exit");
        }

        /// <summary>
        /// 
        /// </summary>
        public static ParsingServiceProviderCollection Providers // intentionally hide base implmentation 
        {
            get { return providerCollection; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual ParsingServiceProviderConfiguration ParsingServiceProviderConfiguration
        {
            get;
            set;
        }

        /// <summary>
        ///returning Default provider object
        /// </summary>
        public static ParsingServiceProviderBase
            DefaultProvider  // intentionally hide base implmentation 
        {
            get
            {
                Initialize();
                return m_provider;
            }
        }

        public override bool CanHandle(string request)
        {
            return handledMessages.ContainsKey(request);
        }

        // Retrieves a list of items
        public override string GetItem(string request)
        {

            throw new NotImplementedException();
        }

        // creates/updates an item
        public override string PutItem(string request)
        {
            throw new NotImplementedException();
        }

        // Retrieves a list of items
        public override string GetItem(string request, dynamic requestPackage)
        {

            throw new NotImplementedException();
        }

        // creates/updates an item
        public override string PutItem(string request, dynamic requestPackage)
        {
            throw new NotImplementedException();
        }

        // Retrieves a list of items
        public override string GetItem(string request, params object[] p)
        {

            throw new NotImplementedException();
        }

        // creates/updates an item
        public override string PutItem(string request, params object[] p)
        {
            throw new NotImplementedException();
        }

        public override int GetStatus()
        {
            return 0;
        }

    }
}
