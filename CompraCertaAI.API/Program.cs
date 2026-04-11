using System.Data;
using System.Text;
using CompraCertaAI.Aplicacao.Aplicacao;
using CompraCertaAI.Aplicacao.Interfaces;
using CompraCertaAI.Repositorio.Contexto;
using CompraCertaAI.Repositorio.Interfaces;
using CompraCertaAI.Repositorio;
using CompraCertaAI.Service.Interface;
using CompraCertaAI.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);

#region Banco de Dados (Dapper)

builder.Services.AddScoped<IDbConnection>(_ =>
    new SqlConnection(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDbContext<CompraCertaAIContexto>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

#endregion

#region Repositórios

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IHistoricoPesquisaRepositorio, HistoricoPesquisaRepositorio>();
builder.Services.AddScoped<ICategoriaRepositorio, CategoriaRepositorio>();
builder.Services.AddScoped<IUsuarioCategoriaRepositorio, UsuarioCategoriaRepositorio>();
builder.Services.AddScoped<IProdutoRepositorio, ProdutoRepositorio>();

#endregion

#region Aplicação

builder.Services.AddScoped<IUsuarioAplicacao, UsuarioAplicacao>();
builder.Services.AddScoped<IAuthAplicacao, AuthAplicacao>();
builder.Services.AddScoped<IHistoricoPesquisaAplicacao, HistoricoPesquisaAplicacao>();
builder.Services.AddScoped<ICategoriaAplicacao, CategoriaAplicacao>();
builder.Services.AddScoped<IBuscaProdutoAplicacao, BuscaProdutoAplicacao>();

#endregion

#region IA - Groq

builder.Services.AddHttpClient<IIAService, AiService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.UserAgent.ParseAdd("CompraCertaAI");
});
builder.Services.AddScoped<IRecomendacaoService, RecomendacaoService>();
builder.Services.AddScoped<IBuscaProdutoService, BuscaProdutoService>();
builder.Services.AddScoped<IHistoricoPesquisaService, HistoricoPesquisaService>();

#endregion

#region JWT

var jwt = builder.Configuration.GetSection("Jwt");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt["Key"]!)
            ),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

#endregion

#region Controllers

builder.Services.AddControllers();

#endregion

#region Swagger + JWT

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CompraCertaAI API",
        Version = "v1",
        Description = "API do CompraCertaAI com IA e autenticação JWT"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe: Bearer {seu token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins(
                "http://localhost:5173"
            );
    });
});


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("FrontendPolicy");
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();