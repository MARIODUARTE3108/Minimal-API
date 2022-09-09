using Bogus;
using Microsoft.EntityFrameworkCore;
using Projeto02.Services.Api.Contexts;
using Projeto02.Services.Api.Entities;
using Projeto02.Services.Api.Helpers;
using Projeto02.Services.Api.Security;
using Projeto02.Services.Api.Setup;
using Projeto02.Services.Api.ViewModels;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();

SwaggerSetup.AddSwaggerSetup(builder);
CorsSetup.AddCorsSetup(builder);

var connectionString = builder.Configuration.GetConnectionString("BDProjeto02");
builder.Services.AddDbContext<SqlServerContext>
    (options => options.UseSqlServer(connectionString));

JwtConfiguration.AddJwtBearerConfiguration(builder);

builder.Services.AddTransient<EmailHelper>(
    map => new EmailHelper(
            builder.Configuration.GetValue<string>("EmailSerrings:Conta"),
            builder.Configuration.GetValue<string>("EmailSerrings:Senha"),
            builder.Configuration.GetValue<string>("EmailSerrings:Smtp"),
            builder.Configuration.GetValue<int>("EmailSerrings:Porta")
        )
    );

var app = builder.Build();

SwaggerSetup.UseSwaggerSetup(app);
CorsSetup.UseCorsSetup(app);

app.UseAuthentication();

#region Minimal API
app.MapPost("/api/register", (SqlServerContext context,RegisterViewModel model) =>
{
    var usuario = model.MapTo();
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    if(context.Usuarios.FirstOrDefault(u => u.Email.Equals(model.Email))!=null)
        return Results.BadRequest(new { message = "O email informado já está cadastrado, tente outro." });

 
    context.Usuarios.Add(usuario);
    context.SaveChanges();

    return Results.Ok(new {usuario.Id, usuario.Nome, usuario.Email, usuario.DataCriacao});
});

app.MapPost("/api/login", (SqlServerContext context, JwtTokenService service, LoginViewModel model) =>
{
    var login = model.MapTo();
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    var usuario = context.Usuarios.FirstOrDefault(u => u.Email.Equals(login.Email)
                                                  && u.Senha.Equals(login.Senha));

    if (usuario == null)
        return Results.BadRequest(new { message = "Acesso negado. Usuário inválido." });

    return Results.Ok(
        new
        {
            usuario.Id,
            usuario.Nome,
            usuario.Email,
            usuario.DataCriacao,
            accessToken = service.Get(usuario.Email)
        }
        );
});

app.MapPost("/api/password-recover", (SqlServerContext context, EmailHelper emailHelper, PasswordRecoverViewModel model) =>
{
    var passwordRecover = model.MapTo();
    if (!model.IsValid)
        return Results.BadRequest(model.Notifications);

    var usuario = context.Usuarios.FirstOrDefault(u=>u.Email.Equals(passwordRecover.Email));
    if (usuario == null) 
        return Results.BadRequest(new { message = "Email não cadastrado." });

    var faker = new Faker();
    var novaSenha = faker.Internet.Password(8);

    var texto = $@"
        <div>
                Olá, <strong>{usuario.Nome}</strong> <br/><br/>
                Foi gerada uma nova senha de acesso para o seu usuário.<br/>
                Utilize a senha: <strong>{novaSenha}</strong> para acessar o sistema.<br/><br/>
                Você poderá utilizar está senha para criar outra de sua preferência.<br/><br/>
                Att,<br/>
                Mário Duarte
       </div>
                ";

    emailHelper.Send(usuario.Email, "Nova senha gerada com sucesso", texto);
    usuario.Senha = MD5Helper.Get(novaSenha);
    context.Entry(usuario).State = EntityState.Modified;
    context.SaveChanges();  

    return Results.Ok(new {usuario.Id, usuario.Nome, usuario.Email, usuario.DataCriacao});
});
#endregion

app.Run();

public partial class Program{}