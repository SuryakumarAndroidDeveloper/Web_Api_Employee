using Ecommerce_WebApi_Application.DataAcessLayer;
using Ecommerce_WebApi_Application.Service;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ProductDAL>();
builder.Services.AddScoped<ProductCategoryDAL>();
builder.Services.AddScoped<CartDAL>();
builder.Services.AddScoped<CustomerDAL>();
builder.Services.AddScoped<OrderDAL>();
builder.Services.AddScoped<WishListDAL>();
builder.Services.AddScoped<PaymentDAL>();
builder.Services.AddScoped<LoginDAL>();
//builder.Services.AddHostedService<OtpCleanupService>();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");
app.UseRouting();

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.Run();
