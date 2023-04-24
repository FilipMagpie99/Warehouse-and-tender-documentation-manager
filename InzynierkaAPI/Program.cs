global using InzynierkaAPI.Data;
using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using InzynierkaAPI.Controllers;
using Azure.Storage.Blobs;
using Azure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using InzynierkaAPI.Services;
using Microsoft.IdentityModel.Tokens;
using InzynierkaAPI.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;

var builder = WebApplication.CreateBuilder(args);
IEnumerable<int> przetargi;

// Add services to the container.

builder.Services.AddControllers(options =>
{

});
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseLazyLoadingProxies().UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IPlikiService, PlikiService>();
builder.Services.AddSingleton<IProducentService, ProducentService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateActor = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
    
});
builder.Services.AddAuthorization();

builder.Services.AddScoped(x => new BlobServiceClient(new Uri("https://inzynierkablob.blob.core.windows.net"), new StorageSharedKeyCredential("inzynierkablob", "aHBtwSwyQjiwH5QjLi/NuoFxuEwk0082HsNo68oImKBrAUJRIB7JKOcXfVY2VEcLPN2OWCmM/ziD+AStp22bdw==")));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseAuthentication();

app.UseAuthorization();
app.MapPost("/login",
(UserLogin user, IUserService service, DataContext db) => Login(user, service, db))
    .Accepts<UserLogin>("application/json")
    .Produces<string>();

app.MapPost("/register",
    (User user, DataContext db) => Register(user, db))
    .Accepts<User>("application/json");

app.UseHttpsRedirection();

app.UseAuthorization();

IResult Login(UserLogin user, IUserService service, DataContext db )
{
    if (!string.IsNullOrEmpty(user.Username) &&
        !string.IsNullOrEmpty(user.Password))
    {
        var loggedInUser = service.Get(user,db);
        if (loggedInUser is null) return Results.NotFound("User not found");

         var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, loggedInUser.Username),
            new Claim(ClaimTypes.Email, loggedInUser.EmailAdress),
            new Claim(ClaimTypes.GivenName, loggedInUser.GivenName),
            new Claim(ClaimTypes.Surname, loggedInUser.Surname),
            new Claim(ClaimTypes.Role, loggedInUser.Role)
        };

        token = new JwtSecurityToken
        (
            issuer: builder.Configuration["Jwt:Issuer"],
            audience: builder.Configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(60),
            notBefore: DateTime.UtcNow,
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                SecurityAlgorithms.HmacSha256)
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return Results.Ok(tokenString);
    }
    return Results.BadRequest("Invalid user credentials");
}

async Task<IResult> Register(User user, DataContext db)
{
    foreach(var usr in db.UserLogin)
    {
        if(user.Username == usr.Username)
        {
            return Results.BadRequest("Podany login jest już zajęty.");
        }
    }
    UserLogin userLogin = new();
    db.User.Add(user);
    userLogin.Username = user.Username;
    userLogin.Password = user.Password;
    db.UserLogin.Add(userLogin);
    await db.SaveChangesAsync();
    return Results.Ok();

}

/*IResult SendEmail(string body)
{
    var email = new MimeMessage();
    email.From.Add(MailboxAddress.Parse("sroczynski.f.99@gmail.com"));
    email.To.Add(MailboxAddress.Parse("filsro000@pbs.edu.pl"));
    email.Subject = "Powiadomienie dotyczące terminu przetargów";
    email.Body = new TextPart(TextFormat.Html) { Text = "Za 7 dni upływa termin przetargów: <br />"+body };

    using var smtp = new SmtpClient();
    smtp.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
    smtp.Authenticate("sroczynski.f.99@gmail.com", "iaozqtmxiknvfazw");
    smtp.Send(email);
    smtp.Disconnect(true); 

    return Results.Ok();
}
*/
async Task<IEnumerable<Przetarg>> GetNotifatedPrzetargi(DataContext db, int dni=7)
{
    return (await  db.Przetarg.ToListAsync()).Where(x => x.DataPrzetargu - DateTime.Now < TimeSpan.FromDays(dni) && x.DataPrzetargu > DateTime.Now && x.Status == Status.Wybrany);
     
}



app.MapControllers();

//app.MapPrzetargEndpoints();

//app.MapProduktEndpoints();


new Task(async () =>
{
    var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<DataContext>();
    while (true)
    {
        try
        {
            var czyPowiadomienia = await db.Ustawienia.FindAsync("Powiadomienia");
            if(czyPowiadomienia == null)
            {
                db.Ustawienia.Add(new Ustawienia
                {
                    Id = "Powiadomienia",
                    Wartosc = true
                });
            }

            var przetargi7 = await GetNotifatedPrzetargi(db);
            var przetargi14 = await GetNotifatedPrzetargi(db,14);
            var przetargi30 = await GetNotifatedPrzetargi(db,30);
            string ret = "";
            if (przetargi7.Any())
                ret += "Następujące przetargi kończą się za 7 dni: <br>";            
            foreach(var item in przetargi7)
            {
                ret += string.Format("<a href='https://localhost:7221/getprzetarg/{0}'>{1}</a><br />", item.Id, item.WystawcaPrzetargu.Nazwa);
            }
              if (przetargi14.Any())
                ret += "Następujące przetargi kończą się za 14 dni: <br>";      
            foreach(var item in przetargi14)
            {
                ret += string.Format("<a href='https://localhost:7221/getprzetarg/{0}'>{1}</a><br />", item.Id, item.WystawcaPrzetargu.Nazwa);
            }
			if (przetargi30.Any())
				ret += "Następujące przetargi kończą się za 30 dni: <br>";
			foreach (var item in przetargi7)
            {
                ret += string.Format("<a href='https://localhost:7221/getprzetarg/{0}'>{1}</a><br />", item.Id, item.WystawcaPrzetargu.Nazwa);
            }
            //if(!string.IsNullOrEmpty(ret))
                //SendEmail(ret);
        }
        catch (Exception e)
        {
            Debug.Write(e.Message);
        }

        await Task.Delay(1000*60*60*24);
    }
}).Start();

app.MapPrzetargEndpoints();

app.MapWystawcaPrzetarguEndpoints();

app.MapDostawcaEndpoints();

app.MapProducentEndpoints();

app.MapProduktEndpoints();

app.MapUserEndpoints();

app.MapRaportMagazynuEndpoints();

app.MapMagazynEndpoints();

app.MapKategoriaPlikEndpoints();


app.Run();


public partial class Program {
   public static  JwtSecurityToken token { get; set; }
}
