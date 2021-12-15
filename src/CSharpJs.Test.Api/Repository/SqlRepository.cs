using Dapper;
using RestSharp;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CSharpJs.Test.Api.Model;
using CSharpJs.Test.Api.Infraestructure;

namespace CSharpJs.Test.Api.Repository
{
    /// <summary>
    /// Repository
    /// </summary>
    public class SqlRepository
    {
        private readonly Connection _connection;
        private string _dbversion;
        /// <param name="connection"></param>
        public SqlRepository(Connection connection)
        {
            _connection = connection;
        }

        internal IEnumerable<dynamic> GetAuthUsers()
        {
            return new[]
            {
                new { Username = (object)"manager"},
                new { Username = (object)"admin"},
            };
        }

        internal object GetUserBranchAssignment(string user)
        {
            return _connection.Request<dynamic>(
                    $"/b1s/v1/Users({GetUserID(user)})");
        }


        // 1 = admin, gerente, supervisor | 2 = operador, vendedor | 0 = não possui cargo  
        internal dynamic ValidateCollaboratorPosition(string code)
        {
            var response = _connection.Query<dynamic>($@"select t1.""name"" from ""HEM6"" t0 inner join ""OHTY"" t1 on t1.""typeID"" = t0.""roleID"" inner join ""OHEM"" t2 on t2.""empID"" =  t0.""empID"" inner join ""OUSR"" t3 on t2.""userId"" =  t3.""USERID"" where t3.""USER_CODE"" = '{code}'");

            var positionCollaborator = 0;

            foreach (var position in response.ToList())
            {
                var descriptio = (string)position.name;

                if (descriptio.ToLower().Contains("vendedor") ||
                    descriptio.ToLower().Contains("operador"))
                {
                    positionCollaborator = 2;
                }
                else if (descriptio.ToLower().Contains("inspetor") ||
                         descriptio.ToLower().Contains("supervisor") ||
                         descriptio.ToLower().Contains("gerente") ||
                         descriptio.ToLower().Contains("adm"))
                {
                    positionCollaborator = 1;
                    break;
                }
            }

            return new { value = positionCollaborator };
        }
        private int GetUserID(string user)
        {
            return _connection.ExecuteEscalarFromText<int>($@"select ""USERID"" from ""OUSR"" where ""USER_CODE"" = '{user}'");
        }

        internal dynamic GetSellers()
        {
            return GetAll("SalesPersons");
        }

        private string GetStatement(string statementName)
        {
            _dbversion = _connection.Info.Database.Version.Contains("HANA") ?
                "HanaStatements" : "SqlServerStatements";
            string sqlStatement = string.Empty;
            string namespacePart = $"CSharpJs.Test.Api.Repository.{_dbversion}";
            string resourceName = namespacePart + "." + statementName;
            using (var stm = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
                if (stm != null) sqlStatement = new StreamReader(stm).ReadToEnd();
            return sqlStatement;
        }

        internal dynamic GetAllowed(string user)
        {
            return _connection.ExecuteEscalarFromText<string>($@"select T0.""USERID"",  CASE T1.""Permission"" when 'N' then 0  else 1 END as ""IsAllowed"", T2.""Name"", T0.""SUPERUSER"" from ""OUSR"" T0 left join ""USR3"" T1 on T0.USERID = T1.""UserLink"" AND T1.""PermId"" like '%XNET_OM_OPEN%' left join ""OUPT"" T2 on T2.""AbsId"" = T1.""PermId""  AND T2.""Name"" <> 'XNET_OM_OPEN' where T0.""USER_CODE"" = '{user}' ");
        }

        public object IsFirstAcess(string user)
        {
            var query = GetStatement("IsFirstAcess.sql");

            return _connection.Query(query, new { @UserName = user }, null, true, 60000).FirstOrDefault();
        }

        public bool GetUserPermission(string user, string authorizationId)
        {
            var query = GetStatement("GetUserPermission.sql");

            return _connection.Query<bool>(query, new { @AuthorizationId = authorizationId, @UserName = user }, null, true, 60000).FirstOrDefault();
        }

        internal Requested GetTerminal(int? code, int? top, int? skip)
        {
            if (code.HasValue)
            {
                return _connection.Request<Requested>($"/b1s/v1/OPEN_MD_TRML('{code}')");
            }
            else if (top.HasValue && skip.HasValue)
            {
                return _connection.Request<Requested>($"/b1s/v1/OPEN_MD_TRML?$top={top}&$skip={skip}&$filter=startswith(Canceled, 'N')");
            }
            else
            {
                return GetTerminal();
            }
        }

        private dynamic GetTerminal()
        {
            return GetAll("OPEN_MD_TRML", true);
        }

        internal void CreateTerminal(List<int> branchesCode)
        {
            foreach (var code in branchesCode)
            {
                _connection.Request<dynamic>(
                                            "/b1s/v1/OPEN_MD_TRML",
                                            ObjBranchCashier(code),
                                            Method.POST);
            }
        }

        internal dynamic ObjBranchCashier(int code)
        {
            var count = _connection.ExecuteEscalarFromText<int>($@"select Count(*) from ""@OPEN_MD_TRML""");

            return new
            {
                Code = count + 1,
                Name = count + 1,
                U_OPEN_BPLId = code,
                U_OPEN_TerminalCode = $@"{GetBrancheName(code)} - {GetCountBranche(code) + 1}"
            };
        }

        internal void CancelTerminal(List<int> branchCashierCode)
        {
            branchCashierCode.ForEach(code =>
            {
                _connection.Request<dynamic>(
                                        $"/b1s/v1/OPEN_MD_TRML('{code}')/Cancel",
                                        new { },
                                        Method.POST);
            });
        }

        private int GetCountBranche(int code)
        {
            return _connection.ExecuteEscalarFromText<int>($@"SELECT COUNT(*) FROM ""@OPEN_MD_TRML"" WHERE ""U_OPEN_BPLId"" = {code}");
        }
        private string GetBrancheName(int code)
        {
            return _connection.ExecuteEscalarFromText<string>($@"SELECT ""BPLName"" FROM OBPL WHERE ""BPLId"" = {code}");
        }

        internal dynamic GetBusinessPlace(int? code, int? top, int? skip)
        {
            if (code.HasValue)
            {
                return _connection.Request<dynamic>($"/b1s/v1/BusinessPlaces({code})");
            }
            else if (top.HasValue && skip.HasValue)
            {
                return _connection.Request<dynamic>($"/b1s/v1/BusinessPlaces?$top={top}&$skip={skip}");
            }
            else
            {
                return GetBusinessPlace();
            }
        }

        internal dynamic GetBusinessPlace()
        {
            return GetAll("BusinessPlaces");
        }

        internal dynamic GetAll(string route, bool getCanceled = false)
        {
            Requested ListReturn = _connection.Request<Requested>($"/b1s/v1/{route}{(getCanceled ? "?$filter=startswith(Canceled, 'N')" : "" )}");

            var NextLink = ListReturn.NextLink != null ? ListReturn.NextLink : null;

            while (NextLink != null)
            {
                var next = _connection.Request<Requested>($@"/b1s/v1/{NextLink}");

                ListReturn.Values.AddRange(next.Values);

                NextLink = next.NextLink != null ? next.NextLink : null;
            }

            return ListReturn;
        }
    }
}