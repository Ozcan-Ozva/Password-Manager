using InformationSecurity.Data;
using InformationSecurity.Models;
using InformationSecurity.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Security.Cryptography;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<UploadFileSettings>(builder.Configuration.GetSection("UploadFileSettings"));
builder.Services.AddScoped<IUserRepository, EntityFrameworkUserRepository>();
builder.Services.AddScoped<IUnitOfWork, EntityFrameworkUnitOfWork>();
builder.Services.AddScoped<IUploadFileRepository, EntityFrameworkUploadFileRepository>();
builder.Services.AddScoped<IUserPasswordsRepository, EntityFrameworkUserPasswordRepository>();
builder.Services.AddScoped<IUploadFileRepository, EntityFrameworkUploadFileRepository>();
builder.Services.AddScoped<IUserKeyRepository, EntityFrameworkUserKeyRepository>();
builder.Services.AddSpaStaticFiles(config => config.RootPath = "wwwroot");
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")
    ));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseStaticFiles();
//app.UseSpaStaticFiles();

app.UseCors(x => x
 .AllowAnyOrigin()
 .AllowAnyMethod()
 .AllowAnyHeader()
);

app.UseRouting();

// this for implement webSocket
var wsOption = new WebSocketOptions { KeepAliveInterval = TimeSpan.FromSeconds(1000) };
app.UseWebSockets(wsOption);
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await Send(context, webSocket);
        }
        else
        {
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
    }
    else
    {
        await next();
    }

});
// webSocket end here

//app.UseSpa(config =>
//{
//    config.Options.SourcePath = Path.Join("D:/AngularProject/security-front");
//    config.UseAngularCliServer("start");
//});


app.UseAuthorization();

app.UseAuthentication();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

async Task Send(HttpContext context, WebSocket webSocket)
{
    //Guid key = Guid.NewGuid();
    var rsaServer = new RSACryptoServiceProvider(1024);
    var publicKeyXml = rsaServer.ToXmlString(false); // when you want to try socket in console mode send this as public key instead of RSAAsymmetric.ExportPublicKey(rsaServer)

    var buffer = new byte[1024 * 4];
    
    WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), System.Threading.CancellationToken.None);
    if (result != null)
    {
        while (!result.CloseStatus.HasValue)
        {
            // read the hello message from client
            string msg = Encoding.ASCII.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
            Console.WriteLine($"Client is Said: {msg}");
            // send the public key to the client.
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(RSAAsymmetric.ExportPublicKey(rsaServer))), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);
            // read the rsa encrypted message and try to decode it using the private key.
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
            msg = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
            Console.WriteLine($"Client is Said: {msg}");
            try // here we try to decode it if we cant we are going to close the session.
            {
                var sessionIdString = Encoding.UTF8.GetString(new ArraySegment<byte>(buffer, 0, result.Count));
                Console.WriteLine($"this is sessionId Before decrypt {sessionIdString}");
                var sessionId = rsaServer.Decrypt(Convert.FromBase64String(sessionIdString), false);
                Console.WriteLine($"this is session id {Encoding.UTF8.GetString(sessionId)}");
            }catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
            }
            // sending Approve session id for client
            await webSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes("Approve")), result.MessageType, result.EndOfMessage, System.Threading.CancellationToken.None);
            // if you want to stop here just comment the next line.
            result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), System.Threading.CancellationToken.None);
        }
    }
    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, System.Threading.CancellationToken.None);
}