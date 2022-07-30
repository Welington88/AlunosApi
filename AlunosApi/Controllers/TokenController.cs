using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AlunosApi.Context;
using AlunosApi.Models;
using AlunosApi.Services;
using AlunosApi.ViewModel;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AlunosApi.Controllers
{
    [ApiVersion("1.0", Deprecated = true)]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext _context;
        private readonly IUserService _authenticate;

        public TokenController(IConfiguration configuration, DataContext context, IUserService authenticate)
        {
            _configuration = configuration;
            _context = context;
            _authenticate = authenticate;
        }

        #region Query
        private String query = "SELECT `IdUsuario`, `Nome`, `Senha` FROM `Usuario`";
        #endregion

        [AllowAnonymous]
        private Usuario buscarUsuario(String nome, String senha)
        {

            Usuario? usuario = new Usuario();
            try
            {

                usuario = _context.Database.GetDbConnection().Query<Usuario>(query).Where(u => u.Nome == nome && u.Senha == senha).FirstOrDefault();
            }
            catch (Exception e)
            {
                throw new Exception ("Erro Buscar Usuarios" + e.InnerException);
            }

            if(usuario is null)
                throw new ArgumentNullException("Erro Buscar Usuarios");

            return usuario;

        }


        // POST: api/Token
        [AllowAnonymous]
        [HttpPost]
        public IActionResult RequestToken([FromBody] Usuario request)
        {
            if (request.Nome is null || request.Senha is null)
                throw new ArgumentNullException("Usuário ou senha igual a Nulo");

            var usuario = buscarUsuario(request.Nome, request.Senha);

            #region Token
            if (usuario.Nome == request.Nome && usuario.Senha == request.Senha)
            {
                //regras de claim
                var claims = new[] {
                    new Claim(ClaimTypes.Name, request.Nome)
                };

                //armazena a chave de token
                var key = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                    );

                //gera o token
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                        issuer: "API6",
                        audience: "API6",
                        claims: claims,
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: creds
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
                #endregion
            }
            

            return BadRequest("Credenciais Inválidas....");

        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserToken>> CreateUser([FromBody] RegisterModel model)
        {
            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "As senhas não conferem");
                return BadRequest(ModelState);
            }

            if (model.Email is null || model.Password is null)
            {
                throw new NullReferenceException("Email ou senha são nulos");
            }

            var result = await _authenticate.RegisterUser(model.Email, model.Password);

            if (result)
            {
                return Ok($"Usuario {model.Email} criado com sucesso!!!");
            }
            else
            {
                ModelState.AddModelError("CreateUser", "Registro inválido");
                return BadRequest(ModelState);
            }
        }

        [HttpPost("LoginUser")]
        public async Task<ActionResult<UserToken>> Login([FromBody] LoginModel userInfo)
        {
            if (userInfo.Email is null || userInfo.Password is null)
            {
                throw new NullReferenceException("Email ou senha são nulos");
            }
            var result = await _authenticate.Authenticate(userInfo.Email, userInfo.Password);

            if (result)
            {
                return GenerateToken(userInfo);
            }
            else
            {
                ModelState.AddModelError("LoginUser", "Login Invalido");
                return BadRequest(ModelState);
            }
        }

        private ActionResult<UserToken> GenerateToken(LoginModel userInfo)
        {
            if (userInfo.Email is null)
                throw new NullReferenceException("Email ou senha são nulos");

            //regras de claim
            var claims = new[] {
                new Claim("email", userInfo.Email),
                new Claim("meuToken", "token"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())

            };

            //armazena a chave de token
            var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])
                );

            //gera chave
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                    issuer: _configuration["Jwt:Issue"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(30),
                    signingCredentials: creds
                );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }
    }
}

