using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TestApiProject.DataContext;
using TestApiProject.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// db connection
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// add identity
builder.Services.AddIdentity<Customer, IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
               .AddJwtBearer(options =>
               {
                   options.TokenValidationParameters = new TokenValidationParameters()
                   {
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Tokens:JwtKey"])),

                       ValidateIssuer = true,
                       ValidIssuer = builder.Configuration["Tokens:JwtIssuer"],

                       ValidateAudience = true,
                       ValidAudience = builder.Configuration["Tokens:JwtAudience"],
                       ValidateLifetime = true,
                       ClockSkew = TimeSpan.Zero
                   };
               });


//builder.Services.AddSwaggerGen(options =>
//{
//    options.AddSecurityDefinition(name: "Bearer", securityScheme: new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Description = "Jwt: `Bearer Token`",
//        In = ParameterLocation.Header,
//        Type = SecuritySchemeType.ApiKey,
//        Scheme = "Bearer"
//    });
//    options.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//    {
//        new OpenApiSecurityScheme
//        {
//            Name = "Bearer",
//            In = ParameterLocation.Header,
//            Reference = new OpenApiReference
//            {
//                Id = "Bearer",
//                Type = ReferenceType.SecurityScheme
//            }
//        },
//        new List<string>()
//    }
//});
//});

builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "TestApi", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Bearer Jwt Token",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            new string[]{}
        }
    });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
