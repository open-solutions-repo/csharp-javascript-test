using Lamar;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Data.Common;
using RestSharp;
using Newtonsoft.Json.Serialization;
using System.Text.RegularExpressions;
using System.Net;

namespace CSharpJs.Test.Api.Infraestructure
{
    public class DatabaseInfo
    {
        public string Server { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public bool Trusted { get; set; }
        public string Version { get; set; }
        public string Company { get; set; }
        public string Provider { get; set; }
    }
    public interface IConnectionInfoBuilder
    {
        string Key { get; set; }
        bool Encrypted { get; set; }
        void Database(Action<DatabaseInfo> action);
        void BusinessOne(Action<BusinessOneInfo> action);
    }

    public class ConnectionInfoBuilder : IConnectionInfoBuilder
    {
        private readonly DatabaseInfo _databaseInfo = new DatabaseInfo();
        private readonly BusinessOneInfo _businessOneInfo = new BusinessOneInfo();

        public string Key { get; set; }
        public bool Encrypted { get; set; }
        public void Database(Action<DatabaseInfo> action)
        {
            action(_databaseInfo);
        }

        public void BusinessOne(Action<BusinessOneInfo> action)
        {
            action(_businessOneInfo);
        }

        public ConnectionInfo Build()
        {
            return new ConnectionInfo
            {
                Key = Key,
                Encrypted = Encrypted,
                BusinessOne = _businessOneInfo,
                Database = _databaseInfo
            };
        }
    }

    public class BusinessOneInfo
    {
        public string LicenceServer { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string SlAddress { get; set; }
        public string Server { get; set; }
        public ConnectionModeEnum Method { get; set; }
        public string CrystalReportsOdbc { get; set; }
    }
    public enum ConnectionModeEnum
    {
        cm_DIAPI = 0,
        cm_ServiceLayer = 1,
        cm_Both = 2
    }
    public class ConnectionInfo
    {
        public string Key { get; set; }
        public bool Encrypted { get; set; }
        public DatabaseInfo Database { get; set; }
        public BusinessOneInfo BusinessOne { get; set; }
        public string Language { get; set; }
        public string Prefix { get; set; }
    }
    public class Connection : IDbConnection
    {
        private Guid _connectionId;
        private int _CreatedConnections;
        private ServiceLayer _slCompany;
        private ConnectionInfo _connectionInfo;
        private static string _nolock;
        private IDbConnection _databaseConnection;

#pragma warning disable CS0649 // Campo "Connection._dbTransaction" nunca é atribuído e sempre terá seu valor padrão null
        private DbTransaction _dbTransaction;
#pragma warning restore CS0649 // Campo "Connection._dbTransaction" nunca é atribuído e sempre terá seu valor padrão null

#pragma warning disable CS0649 // Campo "Connection._parameterChar" nunca é atribuído e sempre terá seu valor padrão null
        private string _parameterChar;
#pragma warning restore CS0649 // Campo "Connection._parameterChar" nunca é atribuído e sempre terá seu valor padrão null

#pragma warning disable CS0649 // Campo "Connection._likeStatementChar" nunca é atribuído e sempre terá seu valor padrão null
        private string _likeStatementChar;
#pragma warning restore CS0649 // Campo "Connection._likeStatementChar" nunca é atribuído e sempre terá seu valor padrão null

        public ConnectionInfo Info => _connectionInfo;

        private DbProviderFactory _factory;

        public int TimeOut
        {
            private get;
            set;
        }

        private IDbDataParameter CreateParameter()
        {
            if (this._factory != null)
            {
                return this._factory.CreateParameter();
            }
            IDbDataParameter result;
            using (IDbCommand command = this.GetCommand())
            {
                result = command.CreateParameter();
            }
            return result;
        }

        private IDbDataParameter BaseParameter(string name, DbType dbType, int size, object value)
        {
            IDbDataParameter dbParameter = this.CreateParameter();
            dbParameter.ParameterName = this._parameterChar + name;
            dbParameter.DbType = dbType;
            dbParameter.Size = size;
            if (value != null)
            {
                dbParameter.Value = value;
            }
            return dbParameter;
        }


        public static IEnumerable<ConnectionInfo> Configurations()
        {
            try
            {
                var configFilePath = ConfigurationManager.AppSettings["configFilePath"] ?? "configurations";
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                };

                var configContent = File.ReadAllText(Path.Combine(configFilePath, "connections.json"));

                if (string.IsNullOrWhiteSpace(configContent) || !configContent.StartsWith("["))
                    throw new Exception(string.Format("Arquivo de configuração localizado em {0} é inválido",
                        Path.Combine(configFilePath, "connections.json")));

                return JsonConvert.DeserializeObject<ConnectionInfo[]>(configContent);
            }
            catch (Exception ex)
            {
                throw new BusinessOneException(-65001, string.Format("Falha ao carregar arquivo de configuração: {0}", ex.Message));
            }
        }

        private static ConnectionInfo LoadConfigFile(string key)
        {
            try
            {
                var configFilePath = ConfigurationManager.AppSettings["configFilePath"] ?? "configurations";
                JsonConvert.DefaultSettings = () => new JsonSerializerSettings
                {
                    ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
                };

                var configContent = File.ReadAllText(Path.Combine(configFilePath, "connections.json"));

                if (string.IsNullOrWhiteSpace(configContent) || !configContent.StartsWith("["))
                    throw new Exception(string.Format("Arquivo de configuração localizado em {0} é inválido",
                        Path.Combine(configFilePath, "connections.json")));

                return JsonConvert.DeserializeObject<ConnectionInfo[]>(configContent)
                    .FirstOrDefault(x => x.Key == key);
            }
            catch (Exception ex)
            {
                throw new BusinessOneException(-65001, string.Format("Falha ao carregar arquivo de configuração: {0}", ex.Message));
            }
        }

        public Connection()
        {
            switch (_connectionInfo.BusinessOne.Method)
            {
                case ConnectionModeEnum.cm_ServiceLayer:
                    OpenServiceLayer();
                    break;
                case ConnectionModeEnum.cm_Both:
                    OpenServiceLayer();
                    OpenDiApi();
                    break;
                case ConnectionModeEnum.cm_DIAPI:
                default:
                    OpenDiApi();
                    break;
            }
        }
        private void OpenDiApi()
        {
            //if (!_instances.Any())
            //    throw new BusinessOneException(-65000, "Não há instâncias disponíveis para operação. Utilize o metodo New para criar uma");

            Open();

            //_instances.Add(_instances.Count);
        }

        private void OpenServiceLayer()
        {
            _slCompany.Login();
        }

        private void CreateServiceLayerConnection(Uri uri)
        {
            if (!(_slCompany is null))
                return;

            _slCompany = new ServiceLayer(uri, new ServiceLayerCredentials
            {
                CompanyDB = _connectionInfo.Database.Company,
                Password = _connectionInfo.BusinessOne.Password,
                UserName = _connectionInfo.BusinessOne.User,
                Language = "29"
            });

            _factory = DbProviderFactories.GetFactory(_connectionInfo.Database.Provider);

            _databaseConnection = _factory.CreateConnection();
            _databaseConnection.ConnectionString = ConnectionString;

            _slCompany.Login();
        }

        public T Request<T>(string endpoint) where T : new()
        {
            return (T)_slCompany.Execute<T>(endpoint, Method.GET);
        }

        public T Request<T>(string endpoint, dynamic body, Method verb, bool replaceCollectionsOnPatch = false) where T : new()
        {
            return (T)_slCompany.Execute<T>(endpoint, body, verb, replaceCollectionsOnPatch);
        }

        public ServiceLayerContext Login(string userName, string password)
        {
            return _slCompany.Login(userName, password);
        }

        public Connection(string key)
        {
            _connectionInfo = LoadConfigFile(key);

            switch (_connectionInfo.BusinessOne.Method)
            {
                case ConnectionModeEnum.cm_ServiceLayer:
                    CreateServiceLayerConnection(new Uri(_connectionInfo.BusinessOne.SlAddress));
                    break;
                case ConnectionModeEnum.cm_Both:
                    {
                        CreateServiceLayerConnection(new Uri(_connectionInfo.BusinessOne.SlAddress));
                    }
                    break;
            }
        }


        public void Dispose()
        {
            Close();
        }

        public void Close()
        {
            if (_databaseConnection != null)
                if (_databaseConnection.State == ConnectionState.Open)
                    _databaseConnection.Close();
        }

        public void Open()
        {
            if (_databaseConnection != null)
                if (_databaseConnection.State != ConnectionState.Open)
                    _databaseConnection.Open();
        }

        public string ConnectionString
        {
            get
            {

                if (_connectionInfo == null) return string.Empty;

                switch (_connectionInfo.Database.Provider)
                {
                    case "System.Data.SqlClient":
                        return (!_connectionInfo.Database.Trusted ?
                                    string.Format("Server={0};Database={1};User Id={2};Password={3};MultipleActiveResultSets=True"
                                        ,  _connectionInfo.Database.Server
                                        , _connectionInfo.Database.Company
                                        , _connectionInfo.Database.User
                                        , _connectionInfo.Database.Password)
                                : string.Format("Server={0};Database={1};Trusted_Connection=True;MultipleActiveResultSets=True"
                                        , _connectionInfo.Database.Server
                                        , _connectionInfo.Database.Company));
                    case "Sap.Data.Hana":
                        return string.Format("Server={0};Current Schema={1}; UserID={2}; Password={3}; Pooling=true; Max Pool Size=1000; Min Pool Size=0;"
                                        , _connectionInfo.Database.Server
                                        , _connectionInfo.Database.Company
                                        , _connectionInfo.Database.User
                                        , _connectionInfo.Database.Password);
                    case "System.Data.SQLite":
                        return string.Format(@"Data Source={0}", Path.Combine(_connectionInfo.Database.Server, _connectionInfo.Database.Company));
                    default:
                        return string.Empty;
                }
            }
            set { throw new NotImplementedException(); }
        }

        /// <summary>
        /// 
        /// </summary>
        public int ConnectionTimeout => _databaseConnection.ConnectionTimeout;

        /// <summary>
        /// 
        /// </summary>
        public string Database => _databaseConnection.Database;

        /// <summary>
        /// 
        /// </summary>
        public ConnectionState State => _databaseConnection.State; 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IDbCommand GetTextCommand(string sqlCommand, IEnumerable<DbParameter> parameters = null)
        {
            var dbCommand = _databaseConnection.CreateCommand();

            dbCommand.CommandType = CommandType.Text;
            dbCommand.CommandText = sqlCommand;

            if (this.TimeOut != 0)
                dbCommand.CommandTimeout = this.TimeOut;

            if (parameters != null)
                if (parameters.Any())
                    parameters.ForEach(current => dbCommand.Parameters.Add(current));

            return dbCommand;
        }

        public IDbCommand GetCommand()
        {
            return this.GetTextCommand(string.Empty, null);
        }

        public T ExecuteEscalarFromText<T>(string sqlCommand, IEnumerable<DbParameter> parameters = null)
        {
            T result;
            using (IDbCommand textCommand = this.GetTextCommand(sqlCommand, parameters))
            {
                if (textCommand.Connection.State != ConnectionState.Open)
                {
                    textCommand.Connection.Open();
                }
                var obj = textCommand.ExecuteScalar();

                if (obj is T)
                    result = (T)obj;
                else
                {
                    try
                    {
                        result = (T)Convert.ChangeType(obj, typeof(T));
                    }
                    catch (InvalidCastException)
                    {
                        result = default(T);
                    }
                }
            }
            return result;
        }

        public IDbDataParameter GetInParameter(string name, DbType dbType, int size, object value)
        {
            if (value != null)
            {
                if (value.GetType().Name.ToUpper() == "STRING" && value.ToString().Contains("*"))
                {
                    value = value.ToString().Replace("*", this._likeStatementChar);
                }
            }
            else
            {
                value = DBNull.Value;
            }
            IDbDataParameter dbParameter = this.BaseParameter(name, dbType, size, value);
            dbParameter.Direction = ParameterDirection.Input;
            return dbParameter;
        }

        public IDbTransaction BeginTransaction()
        {
            return _databaseConnection.BeginTransaction();
        }
        public static void Init(IContainer container)
        {
            ServiceLocator.Initialize(container);
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _databaseConnection.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _databaseConnection.ChangeDatabase(databaseName);
        }

        public IDbCommand CreateCommand()
        {
            return _databaseConnection.CreateCommand();
        }
    }
    public class ServiceLayer
    {
        private readonly RestClient _client;
        private readonly ServiceLayerCredentials _credentials;
        private ServiceLayerContext _context;
        private int _attempts = 0;
        public ServiceLayer(Uri uri, ServiceLayerCredentials credentials)
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            _client = new RestClient(uri);
            _credentials = credentials;
            _client.Timeout = 1200000;
            _client.ReadWriteTimeout = 1200000;

            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy()
                }
            };
        }

        public ServiceLayerContext Context { get { return _context; } }

        public void SetSlTimeout(int timeout)
        {
            _client.Timeout = timeout;
            _client.ReadWriteTimeout = timeout;
        }

        private List<T> GetAllMatches<T>(Match match, List<T> currentList) where T : new()
        {
            if (match != null && match.Value.Contains("error"))
            {
                var error = JsonConvert.DeserializeObject<SLError>(match.Value);

                if (error != null && error.Error != null)
                    throw new BusinessOneException(error.Error.Code, error.Error.Message.Value);
                else
                    currentList.Add(JsonConvert.DeserializeObject<T>(match.Value));
            }
            else
                currentList.Add(JsonConvert.DeserializeObject<T>(match.Value));

            var next = match.NextMatch();
            if (next != null && !string.IsNullOrWhiteSpace(next.Value))
            {
                GetAllMatches(next, currentList);
            }

            return currentList;
        }

        public void Execute(string endpoint, object body, Method verb, bool replaceCollectionsOnPatch = false)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            var request = new RestRequest(endpoint, verb);
            if (!string.IsNullOrEmpty(_context.SessionId) && !_context.Expired)
                request.AddCookie("B1SESSION", _context.SessionId);

            if (replaceCollectionsOnPatch && verb == Method.PATCH)
                request.AddHeader("B1S-ReplaceCollectionsOnPatch", "true");

            if (body != null)
            {
                var obj = JsonConvert.SerializeObject(body, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new DefaultNamingStrategy()
                    }
                });

                request.AddJsonBody(obj);
            }
            IRestResponse response = null;
            if (_context.Expired)
            {
                _context = null;
                Login();
                Execute(endpoint, body, verb, replaceCollectionsOnPatch);
                _context.Created = DateTime.Now;
            }
            else
            {
                response = _client.Execute(request);
                _context.Created = DateTime.Now;
            }

            if (IsError(response))
                throw TreatError(response);
        }

        public T Execute<T>(string endpoint, Method verb) where T : new()
        {
            return Execute<T>(endpoint, null, verb);
        }

        public T Execute<T>(string endpoint, object body, Method verb, bool replaceCollectionsOnPatch = false) where T : new()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            var request = new RestRequest(endpoint, verb);
            if (!string.IsNullOrEmpty(_context.SessionId) && !_context.Expired)
                request.AddCookie("B1SESSION", _context.SessionId);

            if (replaceCollectionsOnPatch && verb == Method.PATCH)
                request.AddHeader("B1S-ReplaceCollectionsOnPatch", "true");

            if (body != null)
            {
                var obj = JsonConvert.SerializeObject(body, new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new DefaultNamingStrategy()
                    }
                });

                request.AddJsonBody(obj);
            }

            IRestResponse response = null;
            if (_context.Expired)
            {
                _context = null;
                Login();
                Execute<T>(endpoint, body, verb, replaceCollectionsOnPatch);
                _context.Created = DateTime.Now;
            }
            else
            {
                response = _client.Execute<T>(request);
                _context.Created = DateTime.Now;
            }

            if (IsError(response))
                throw TreatError(response);

            //var stringContent = new StringContent(JsonConvert.SerializeObject(response.Data), Encoding.UTF8, "application/json");
            return JsonConvert.DeserializeObject<T>(response.Content);
            //return data;
        }
        /// <summary>
        /// Realiza o login com o usuário e senha informados.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public ServiceLayerContext Login(string userName, string password)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            var obj = JsonConvert.SerializeObject(new
            {
                CompanyDB = _credentials.CompanyDB,
                UserName = userName,
                Password = password,
                Language = _credentials.Language
            }, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy()
                }
            });
            var request = new RestRequest("b1s/v1/Login", Method.POST).AddHeader("Content-Type", "text/plain");
            request.AddJsonBody(obj);
            var response = _client.Execute<ServiceLayerContext>(request);

            return JsonConvert.DeserializeObject<ServiceLayerContext>(response.Content);
        }

        public void Login()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;
            if (_context != null) if (!string.IsNullOrWhiteSpace(_context.SessionId) && !_context.Expired) return;

            var obj = JsonConvert.SerializeObject(_credentials, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new DefaultNamingStrategy()
                }
            });
            var request =
                new RestRequest("b1s/v1/Login", Method.POST)
                    .AddHeader("Content-Type", "text/plain");

            request.AddJsonBody(obj);

            var response = _client.Execute<ServiceLayerContext>(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                if (_attempts < 15)
                {
                    _attempts++;
                    Login();
                    _context.Created = DateTime.Now;
                }

                if (IsError(response))
                    throw TreatError(response);

                Console.WriteLine($"Houve um problema no login da service layer! Obj => {obj}");
                throw new BusinessOneException(98, "Houve algum problema no login da ServiceLayer!");
            }

            _context = response.Data;
            _context.Created = DateTime.Now;
        }

        public bool IsError(IRestResponse response)
        {
            if (!response.IsSuccessful)
                return true;
            if (response.ErrorException != null)
            {
                if (!response.ErrorMessage.ToLower().Contains("a cadeia de caracteres de entrada não estava em um formato correto."))
                {
                    return true;
                }
            }
            return false;
        }

        private Exception TreatError(IRestResponse response)
        {
            if (string.IsNullOrWhiteSpace(response.Content) && !string.IsNullOrWhiteSpace(response.ErrorMessage))
            {
                return new BusinessOneException(9832, response.ErrorMessage);
            }
            else
            {
                var erro = JsonConvert.DeserializeObject<SLError>(response.Content)?.Error;

                if (erro != null)
                    return new BusinessOneException(erro.Code, erro.Message.Value);
                else
                    return new Exception("Problema não identificado!" + response.ErrorMessage ?? "");
            }

        }
    }
    public class BusinessOneException : Exception
    {
        private readonly int _errorCode;
        private readonly string _errorDescription;

        public BusinessOneException(int errorCode, string errorDescription) : base(errorDescription)
        {
            _errorCode = errorCode;
            _errorDescription = errorDescription;
        }

        public int ErroCode { get { return _errorCode; } }
        public string ErroDescription { get { return _errorDescription; } }
    }
    public class ServiceLayerContext
    {
        [JsonIgnore]
        public DateTime Created { get; set; }
        [JsonProperty("@odata.context")]
        public string OdataContext { get; set; }
        [JsonProperty("SessionId")]
        public string SessionId { get; set; }
        [JsonProperty("Version")]
        public string Version { get; set; }
        [JsonProperty("SessionTimeout")]
        public int SessionTimeout { get; set; }
        [JsonProperty("error")]
        public ServiceLayerException Exception { get; set; }


        [JsonIgnore]
        public bool Expired
        {
            get
            {
                return Created.AddMinutes(SessionTimeout).CompareTo(DateTime.Now) <= 0;
            }
        }
    }
    public class ServiceLayerException
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public ServiceLayerExceptionMessage ServiceLayerMessage;
    }
    public class ServiceLayerExceptionMessage
    {
        [JsonProperty("lang")]
        public string Language { get; set; }
        [JsonProperty("value")]
        public string Message { get; set; }
    }
    public class SLError
    {
        [JsonProperty("error")]
        public Error_Model Error { get; set; }
    }

    public class Error_Model
    {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public Error_Message_Model Message { get; set; }
    }

    public class Error_Message_Model
    {
        [JsonProperty("lag")]
        public string Lang { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
    }
    public class ServiceLayerCredentials
    {
        [JsonProperty("CompanyDB")]
        public string CompanyDB { get; set; }
        [JsonProperty("UserName")]
        public string UserName { get; set; }
        [JsonProperty("Password")]
        public string Password { get; set; }
        [JsonProperty("Language")]
        public string Language { get; set; }
    }
    public enum BoDataServerTypes
    {
        dst_MSSQL = 1,
        dst_DB_2 = 2,
        dst_SYBASE = 3,
        dst_MSSQL2005 = 4,
        dst_MAXDB = 5,
        dst_MSSQL2008 = 6,
        dst_MSSQL2012 = 7,
        dst_MSSQL2014 = 8,
        dst_HANADB = 9,
        dst_MSSQL2016 = 10,
        dst_MSSQL2017 = 11,
        dst_MSSQL2019 = 15
    }

    public class ServiceLocator : IDisposable
    {
        private static bool _isInitialize = false;
        private static Dictionary<string, object> _implementedObjet;
        private static string _p;

        private static IContainer _kernel;
        public static void Initialize(IContainer kernel)
        {
            _kernel = kernel;
            _isInitialize = _kernel != null;
        }

        public static bool IsInitialize { get { return _isInitialize; } }

        public static ServiceLocator AddObject<T>(T obj)
        {
            if (_implementedObjet == null)
                _implementedObjet = new Dictionary<string, object>();

            _implementedObjet.Add(_p + typeof(T).Name, obj);
            _isInitialize = true;
            return new ServiceLocator();
        }

        public static void Prefix(string p)
        {
            _p = p;
        }

        public static IDisposable SetUpTest()
        {
            return new ServiceLocator();
        }

        public static T Get<T>()
        {
            return _implementedObjet != null
                ? (T)_implementedObjet[_p + typeof(T).Name]
                : typeof(T).IsClass && !typeof(T).IsAbstract
                    ? _kernel.GetInstance<T>()
                    : _kernel.TryGetInstance<T>();
        }

        public static object Get(Type type)
        {
            return (type.IsClass && !type.IsAbstract ? _kernel.GetInstance(type) : _kernel.TryGetInstance(type)) ?? _implementedObjet?[type.Name];
        }

        public static object Get(Type type, string typeName)
        {
            return _implementedObjet[_p + type.Name] ?? (type.IsClass && !type.IsAbstract ? _kernel.GetInstance(type, typeName) : _kernel.TryGetInstance(type, typeName));
        }

        public void Dispose()
        {
            _implementedObjet = null;
        }
    }
}
