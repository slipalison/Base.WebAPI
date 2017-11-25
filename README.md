# LEIA-ME #

## **Sml.WebAPI** ##

### O que contempla esse repositório? ###
* **SEGURANÇA**
* * Autenticação [OAuth 2.0](https://oauth.net/2/)
* * Acesso a dados **básicos** de usuário através de [Claims](https://msdn.microsoft.com/en-us/library/system.security.claims.claim(v=vs.110).aspx)   
* **LOG**
* * Engine **SIMPLES** para log de exceção não tratada, salvando toda informação necessária em um arquivo estruturado no formato JSON. 
* **FACILIDADE**
* * BaseController, a simples classe contendo o padrão de resultado "Task<HttpResponseMessage>".
* * JsonCompression, Componente para compressão do resultado json.
* **Extensão**
* * NewtonsoftExtension, estende a clase JToken, podendo converter JToken para Objetos ou lista "tipadas".   
* * SqlCommanderExtension, estenção da interface IDbConnection, facilitando a execução de comandos SQL e transformando o resultado em Json dispensando um Objeto "tipado". 



### Como usar? ###
Esse componente é baseado em Owin, ou seja seu Web Api tem que seguir esse padrão. 

**Criando Web API com [OWIN](http://owin.org/)**

Com o Visual Studio aberto, clique em File > New > New Project. 
Em Template navegue até "Other Project Type" selecione "Blank Solution"
Selecione a pasta e o nome do projeto. 

Agora podemos adicionar um novo Projeto. 
Adicione um Web Application, e selecione o projeto  **EMPTY** 
No termino, adicione as dependências do Web API e OWIN 

----------
1. Install-Package Microsoft.AspNet.WebApi.Owin
1. Install-Package Microsoft.Owin.Host.SystemWeb
1. Install-Package Microsoft.Owin.Security.OAuth
1. Install-Package Microsoft.Owin.Cors
1. Install-Package Unofficial.Ionic.Zip

----------

Crie uma Classe na raiz da aplicação, chamada **Startup.cs**

Nessa classe use como exemplo (Favor ler os comentários, e apagar quando for implementado) 


```
#!c#
using Microsoft.Owin;
using Owin;
using Sml.WebAPI;
using System;
using System.Web.Http;

//informa que essa é a classe owin, facilita o debug caso queira debugar essa classe
[assembly: OwinStartup(typeof(PROJETO.Startup))]

namespace Sml.PROJETO
{
    public class Startup
    {
        //A ordem dessa configuração afeta o funcionamento do site
        public static void Configuration(IAppBuilder app)
        {
            //Configuração do Web API
            var config = new HttpConfiguration();
            ConfigurationWebApi(config);

            // Adiciona o OAuth
            ConfigureOAuth(app);
          
            //Cors Options 
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            
            //Informa que estamos usando WebApi
            app.UseWebApi(config);
        }

        public static void ConfigurationWebApi(HttpConfiguration config)
        {
           /*
            Essa linha diz que todos as chamadas do WebAPI é 
            obrigatoria estar AUTENTICADA, caso exista um método publico
            favor decorar o metodo com "AllowAnonymous", ou Remova essa linha e
            para todos os metodos que não são publicos decore com "Authorize"
            */
            config.Filters.Add(new AuthorizeAttribute());
            /*
            Aqui temos um filtro que aciona o Log de todos as exceções 
            não tratada, você pode passar um path ou deixa no padrão que
            é a pasta da aplicação "../log", caso o path informado esteja
            invalido, ele irá assumir o padrão.
            */
            config.Filters.Add(new SMLExceptionHandlingAttribute());

            //Configuração para que você possa determinar a Rota por annotation.
            config.MapHttpAttributeRoutes();

            //configuração de rota padrão (opcional) 
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{version}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional, version = RouteParameter.Optional }
                );
        }

        public static void ConfigureOAuth(IAppBuilder app)
        {
            //Remove os HEADERS, opcional porem diminue a segurança  
            app.Use((context, next) =>
            {
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");
                context.Response.Headers.Remove("X-AspNet-Version");
                context.Response.Headers.Remove("X-AspNetMvc-Version");
                context.Response.Headers.Remove("Server");
                return next.Invoke();
            });

            // invoca o OAUTH, o parametro de TimeSpan, condiz com o tempo para expirar o token
            ConfigurationOAuth.ConfigureOAuth(app, TimeSpan.FromHours(1));
        }
    }

}

```

## Controllers ##

Para usar os recursos padrões:

```
#!c#
using Sml.WebAPI.Controller;
using Sml.WebAPI.JsonCompression;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sml.PROJETO.Controller
{
     // Utilize BaseApiController para herdar o padrão do resultado
    [RoutePrefix("api/v1/Values")]
    public class ValuesController : BaseApiController
    {
        /// metodo GET COPRIMIDO retorna o nome do usuario com URL: api/v1/Values
        [HttpGet, DeflateCompression]
        public async Task<HttpResponseMessage> Get()
            => await TaskHttpResponseMessage(HttpStatusCode.OK, User.Identity.Name);

        /// Metodo POST que aciona excessão a ser gravada no LOG com URL: api/v1/Values
        [HttpPost]
        public async Task<HttpResponseMessage> Post()
        {
            try
            {
                throw new Exception("teste ");
            }
            catch (Exception ex)
            {

                throw new Exception("ex2", ex);
            }
        }

        /// Metodo GET PUBLICO COPRIMIDO, com Alteração da ROTA com URL: /api/v1/Values/Teste
        [AllowAnonymous, HttpGet, Route("Teste"), DeflateCompression]
        public async Task<HttpResponseMessage> Teste() 
            => await TaskHttpResponseMessage(HttpStatusCode.OK, "Funcionando!");
    }
}
```


## **Acesso a dados** ##

```
#!c#

using Oracle.ManagedDataAccess.Client;
using Sml.WebAPI;
using Sml.WebAPI.BasicSql;
using Sml.WebAPI.JsonCompression;
using System;
using System.Configuration;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Sml.IF.ConsorcioAdesao.API.Controller
{
    [RoutePrefix("api/v1/Values")]
    public class ValuesController : BaseApiController
    {
        private readonly IDbConnection _iforacle = new OracleConnection(ConfigurationManager.ConnectionStrings["iforacle"].ToString());
      
     
        /// Usando Extensão de acesso a dados 
        [AllowAnonymous, HttpGet, Route("Teste"), DeflateCompression]
        public async Task<HttpResponseMessage> Teste() 
            => await TaskHttpResponseMessage(HttpStatusCode.OK, _iforacle.ExecuteToJson("select * from VW_BORDEROS2 where rownum = :qtd", false, new { qtd = 1 }));

        public void Dispose()
        {
            _iforacle.Dispose();
        }
    }
}
```

Resultado: 

```
#!json

[
   {
      "CONTRATO":113563684000758,
      "CPF_CNPJ":"8742095603",
      "TIPO_CONTRATO":"PROPOSTA ADESAO",
      "NOME":"CLOVIS MODESTO SILVA CASTRO",
      "TIPO_PESSOA":"F",
      "RG":null,
      "EMISSOR_RG":null,
      "EMISSAO":"2011-09-15T00:00:00",
      "BORDERO":113563684000758,
      "GRUPO":null,
      "COTA":null,
      "ADMINISTRADORA":null,
      "DT_FATURAMENTO":null,
      "SEQ_DOC":125919791
   }
]
```

Obs: os parâmetros no Oracle se utiliza ":" (dois pontos), já no Sql Server se utiliza "@"(arroba), ou seja, o parâmetro  ":qtd" ficaria "@ qtd" no sql server