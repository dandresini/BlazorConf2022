using BlazorConf2022.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using System.Security.Claims;

namespace BlazorConf2022.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {

        private readonly ILogger<UserController> logger;
        private readonly GraphServiceClient graphClient;
        private readonly string _customAttribute = "";
        public UserController(ILogger<UserController> Logger, GraphServiceClient GraphClient, IConfiguration Configuration)
        {
            logger = Logger;
            graphClient = GraphClient;

            _customAttribute = $"extension_{Configuration["AzureB2C:CLientIdCustom"].Replace("-", "")}_";
        }




        [HttpGet]
        [Route("me")]
        [RequiredScope(new string[] { "api.read" })]
        public async Task<IActionResult> Get()
        {
            //Richiesta informazioni utente
            //var result = await graphClient.Users[User.FindFirst(ClaimTypes.NameIdentifier)!.Value]
            //    .Request()
            //    .GetAsync();

            //Richiesta informazioni utente con select
            var result = await graphClient.Users[User.FindFirst(ClaimTypes.NameIdentifier)!.Value]
                .Request()
                .Select($"Surname, GivenName, City, PostalCode,userPrincipalName, Mail,{ _customAttribute}AnimalePreferito")
                .GetAsync();

            var user = new UserProfile
            {
                Surname = result.Surname,
                GivenName = result.GivenName,
                City = result.City,
                PostalCode = result.PostalCode,
                AnimalePreferito = result.AdditionalData.ContainsKey($"{_customAttribute}AnimalePreferito") ?
                                      (TipoAnimale)Convert.ToInt32(result.AdditionalData[$"{_customAttribute}AnimalePreferito"]!.ToString()) : null

            };
            return Ok(user);
        }




        [HttpPost]
        [Route("me/editprofile")]
        [RequiredScope(new string[] { "api.write" })]
        public async Task<IActionResult> Post(UserProfile user)
        {
            if (ModelState.IsValid)
            {
                var result = await graphClient.Users[User.FindFirst(ClaimTypes.NameIdentifier)!.Value]
                .Request()
                .UpdateResponseAsync(new User
                {
                    Surname = user.Surname,
                    GivenName = user.GivenName,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    AdditionalData = new Dictionary<string, object>
                    {
                        { $"{_customAttribute}AnimalePreferito",user.AnimalePreferito is null ? "": ((int)user.AnimalePreferito).ToString()  }
                    }

                });

                return result.StatusCode == System.Net.HttpStatusCode.NoContent || result.StatusCode == System.Net.HttpStatusCode.OK
                       ? Ok() : BadRequest();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

    }
}
