using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using System.Runtime.CompilerServices;
using KLEEBE.Models;
using KLEEBE.DTOs;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
//ADD CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000",
                                "http://localhost:7110")
                                .AllowAnyHeader()
                                .AllowAnyMethod();
        });
});

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// allows passing datetimes without time zone data 
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// allows our api endpoints to access the database through Entity Framework Core
builder.Services.AddNpgsql<KLEEDbContext>(builder.Configuration["KLEEDbConnectionString"]);

// Set the JSON serializer options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
var app = builder.Build();

//Add for Cors
app.UseCors(MyAllowSpecificOrigins);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// USERS
// Check User Exists
app.MapGet("/checkuser/{uid}", (KLEEDbContext db, string uid) =>
{
    var userExists = db.Users.Where(u => u.Uid == uid).FirstOrDefault();
    if (userExists == null)
    {
        return Results.StatusCode(204);
    }
    return Results.Ok(userExists);
});


// Register A New User
app.MapPost("/api/user/new", async (KLEEDbContext db, Users user) =>
{
    Users newUser = new()
    {
        DisplayName = user.DisplayName,
        Email = user.Email,
        IsAdmin = user.IsAdmin,
        Uid = user.Uid,
    };

    db.Users.Add(newUser);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Update An Existing User
app.MapPost("/api/user/edit/{id}", (KLEEDbContext db, int id, Users user) =>
{
    Users userToUpdate = db.Users.SingleOrDefault(u => u.Id == id);
    if (userToUpdate == null)
    {
        return Results.NotFound();
    }
    userToUpdate.DisplayName = user.DisplayName;
    userToUpdate.Email = user.Email;
    userToUpdate.IsAdmin = user.IsAdmin;
    db.SaveChanges();
    return Results.NoContent();
});

// Delete An Existing User & It's Restaurant Reviews
app.MapDelete("/api/user/{id}", (KLEEDbContext db, int id) =>
{
    Users user = db.Users
        .Include(u => u.Reviews)
        .SingleOrDefault(u => u.Id == id);

    if (user == null)
    {
        return Results.NotFound();
    }
    db.Reviews.RemoveRange(user.Reviews);
    db.Users.Remove(user);
    db.SaveChanges();
    return Results.NoContent();
});

// CATEGORIES
// Get All Categories
app.MapGet("/api/categories", (KLEEDbContext db) =>
{
    return db.Categories.ToList();
});

// Create A New Category
app.MapPost("/api/category/new", (KLEEDbContext db, Categories category) =>
{
    db.Categories.Add(category);
    db.SaveChanges();
    return Results.Created($"/api/category/{category.Id}", category);
});

// Update An Existing Category
app.MapPut("/api/category/edit/{id}", (KLEEDbContext db, int id, Categories category) =>
{
    Categories categoryToUpdate = db.Categories.SingleOrDefault(c => c.Id == id);
    if (categoryToUpdate == null)
    {
        return Results.NotFound();
    }
    categoryToUpdate.Title = category.Title;
    categoryToUpdate.PhotoUrl = category.PhotoUrl;
    db.SaveChanges();
    return Results.NoContent();
});

// Delete An Existing Category 
app.MapDelete("/api/category/{id}", (KLEEDbContext db, int id) =>
{
    Categories category = db.Categories.SingleOrDefault(c => c.Id == id);
    if (category == null)
    {
        return Results.NotFound();
    }

    foreach (var restaurant in db.Restaurants.Where(r => r.Categories.Any(c => c.Id == id)))
    {
        restaurant.Categories.Remove(category);
    }

    db.Categories.Remove(category);
    db.SaveChanges();
    return Results.NoContent();
});

//Add A Category To A Restaurant
app.MapPost("/api/restaurant/restaurantCategory/{restaurantId}/{categoryId}", (KLEEDbContext db, int restaurantId, int categoryId) =>
{
    Restaurants restaurant = db.Restaurants.SingleOrDefault(r => r.Id == restaurantId);
    Categories category = db.Categories.SingleOrDefault(c => c.Id == categoryId);

    if (restaurant.Categories == null)
    {
        restaurant.Categories = new List<Categories>();
    }
    restaurant.Categories.Add(category);
    db.SaveChanges();
    return restaurant;
});

// RESTAURANTS
// Get All Restaurants
app.MapGet("/api/restaurants", (KLEEDbContext db) =>
{
    return db.Restaurants.ToList();
});

// Get A Single Restaurant
app.MapGet("/api/restaurant/{id}", (KLEEDbContext db, int id) =>
{
    var restaurant = db.Restaurants.Where(r => r.Id == id)
        .Include(r => r.Reviews)
        .Include(r => r.Categories)
        .FirstOrDefault();
    if (restaurant == null)
    {
        return Results.NotFound();
    }
    return Results.Ok(restaurant);
});

// Create A New Restaurant
app.MapPost("/api/restaurant/new", (KLEEDbContext db, Restaurants restaurant) =>
{
    db.Restaurants.Add(restaurant);
    db.SaveChanges();
    return Results.Created($"/api/restaurant/{restaurant.Id}", restaurant);
});

// Update An Existing Restaurant
app.MapPut("/api/restaurant/edit/{id}", (KLEEDbContext db, int id, Restaurants restaurant) =>
{
    Restaurants restaurantToUpdate = db.Restaurants.SingleOrDefault(r => r.Id == id);
    if (restaurantToUpdate == null)
    {
        return Results.NotFound();
    }
    restaurantToUpdate.Name = restaurant.Name;
    restaurantToUpdate.Phone = restaurant.Phone;
    restaurantToUpdate.Website = restaurant.Website;
    restaurantToUpdate.PhotoUrl = restaurant.PhotoUrl;
    restaurantToUpdate.VideoUrl = restaurant.VideoUrl;
    restaurantToUpdate.Address = restaurant.Address;
    restaurantToUpdate.Description = restaurant.Description;
    restaurantToUpdate.UpdatedOn = DateTime.Now;
    db.SaveChanges();
    return Results.NoContent();
});

// Delete An Existing Restaurant & It's Reviews 
app.MapDelete("/api/restaurant/{id}", (KLEEDbContext db, int id) =>
{
    Restaurants restaurant = db.Restaurants
        .Include(r => r.Categories)
        .Include(r => r.Reviews)
        .SingleOrDefault(r => r.Id == id);
    if (restaurant == null)
    {
        return Results.NotFound();
    }

    foreach (var category in restaurant.Categories.ToList())
    {
        restaurant.Categories.Remove(category);
    }

    db.Reviews.RemoveRange(restaurant.Reviews);
    db.Restaurants.Remove(restaurant);
    db.SaveChanges();
    return Results.NoContent();
});


// REVIEWS
// Get All Reviews For A Single Restaurant
app.MapGet("/api/restaurant/{id}/reviews", (KLEEDbContext db, int id) =>
{
    var restaurant = db.Restaurants.SingleOrDefault(r => r.Id == id);
    if (restaurant == null)
    {
        return Results.NotFound();
    }

    var reviews = db.Reviews
        .Where(r => r.RestaurantId == id)
        .ToList();

    return Results.Ok(reviews);
});

// Create A Review
app.MapPost("/api/review/new", async (KLEEDbContext db, ReviewsDTO review) =>
{
    Reviews newReview = new()
    {
        Content = review.Content,
        Rating = review.Rating,
        UserId = review.UserId,
        RestaurantId = review.RestaurantId,
        PostedOn = review.PostedOn,
        UpdatedOn = review.UpdatedOn
    };

    newReview.User = await db.Users.FirstOrDefaultAsync(u => u.Id == review.UserId);
    newReview.Restaurant = await db.Restaurants.FirstOrDefaultAsync(r => r.Id == review.RestaurantId);

    db.Reviews.Add(newReview);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Update An Existing Review
app.MapPut("/api/review/edit/{id}", (KLEEDbContext db, int id, ReviewsDTO review) =>
{
    Reviews reviewToUpdate = db.Reviews.SingleOrDefault(r => r.Id == id);
    if (reviewToUpdate == null)
    {
        return Results.NotFound();
    }
    reviewToUpdate.Content = review.Content;
    reviewToUpdate.Rating = review.Rating;
    reviewToUpdate.UpdatedOn = DateTime.Now;
    db.SaveChanges();
    return Results.NoContent();
});

// Delete An Existing Review
app.MapDelete("/api/review/{id}", (KLEEDbContext db, int id) =>
{
    Reviews review = db.Reviews.SingleOrDefault(r => r.Id == id);
    if (review == null)
    {
        return Results.NotFound();
    }

    db.Reviews.Remove(review);
    db.SaveChanges();
    return Results.NoContent();
});

app.Run();
