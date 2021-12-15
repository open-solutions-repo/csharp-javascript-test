using CSharpJs.Test.Api.Infraestructure;
using CSharpJs.Test.Api.Repository;
using Microsoft.Owin.Security.OAuth;
using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CSharpJs.Test.Api.Providers
{
    /// <summary>
    /// 
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        private string _company;
        private string _clientId;
        private string _clientSecret;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            _company = context.Parameters.Get("company");

            switch(context.Parameters["grant_type"])
            {
                case "password":
                    await Task.Run(() => PasswordAuth(context));
                    break;
                case "client_credentials":
                    await Task.Run(() => ClientIdAuth(context));
                    break;
                default:
                    context.SetError("invalid_grant", "Invalid grant_type parameter");
                    context.Rejected();
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task ClientIdAuth(OAuthValidateClientAuthenticationContext context)
        {
            Guid clientIdGuid;

            if (!context.TryGetBasicCredentials(out _clientId, out _clientSecret))
            {
                context.TryGetFormCredentials(out _clientId, out _clientSecret);
            }
            if (null == context.ClientId || null == _clientSecret || !Guid.TryParse(_clientId, out clientIdGuid))
            {
                context.SetError("invalid_credentials", "A valid client_Id and client_Secret must be provided.");
                context.Rejected();
                return;
            }
            //validate aginstdb or config: GetClient(clientIdGuid, clientSecret);  
            bool isValidClient = "E2887E58-0374-4043-8D82-AC218CDF0016" == _clientId && "e0UyODg3RTU4LTAzNzQtNDA0My04RDgyLUFDMjE4Q0RGMDAxNn0=" == _clientSecret;
            if (!isValidClient)
            {
                context.SetError("invalid_credentials", "A valid client_Id and client_Secret must be provided.");
                context.Rejected();
                return;
            }

            await Task.Run(() => context.Validated(_clientId));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task PasswordAuth(OAuthValidateClientAuthenticationContext context)
        {
            await Task.Run(() => context.Validated());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantClientCredentials(OAuthGrantClientCredentialsContext context)
        {
            Guid.TryParse(context.ClientId, out Guid clientId);

            bool client = _clientId == clientId.ToString().ToUpper();

            if (!client)
            {
                context.SetError("invalid_grant", "Invaild client.");
                context.Rejected();
                return;
            }
            var claimsIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
            claimsIdentity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
            await Task.Run(() => context.Validated(claimsIdentity));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        { 
            if (string.IsNullOrEmpty(_company))
            {
                context.SetError("invalid_grant", "Verifique a empresa");
                return;
            }

            if (string.IsNullOrWhiteSpace(context.UserName) || string.IsNullOrWhiteSpace(context.Password))
            {
                context.SetError("invalid_grant", "Usuario ou senha inválido");
                return;
            }

            if (!Autentication(context.UserName, context.Password))
            {
                context.SetError("invalid_grant", "Verifique suas credenciais");
                return;

            }
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));
            identity.AddClaim(new Claim("role", "user"));

            context.Validated(identity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private bool Autentication(string userName, string password)
        {
            var c = new Connection(_company);
            var r = new SqlRepository(c);

            var authUser = from u in r.GetAuthUsers()
                           where u.Username == userName
                           select u;

            if (authUser.Any())
                return false;

            byte[] data = Convert.FromBase64String(password);
            string decodedString = Encoding.UTF8.GetString(data);

            var response = c.Login(userName, decodedString);

            return response.Exception != null ? false : true;
        }
    }
}
