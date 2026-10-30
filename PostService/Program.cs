using PostService.BusinessLogic;
using PostService.DataAccess;
using PostService.Dtos;
using PostService.Mappings;
using PostService.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PostingDbContext>();
builder.Services.AddScoped<IPostingRepository, PostingRepository>();
builder.Services.AddScoped<IPostingService, PostingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<PostingDbContext>();
    dbContext.Database.EnsureCreated();
}

app.MapGet("/postings", (IPostingService postingService) =>
    Results.Ok(postingService.GetAll().Select(PostingMapper.ToPostingGetDto)));

app.MapGet("/postings/{id}", (int id, IPostingService postingService) =>
{
    var posting = postingService.Find(id);
    return posting == null ? Results.NotFound() : Results.Ok(PostingMapper.ToPostingGetDto(posting));
});

app.MapPost("/postings", (PostingPostDto postDto, IPostingService postingService) =>
{
    var newPosting = PostingMapper.ToPosting(postDto);
    var savedObject = postingService.Create(newPosting);
    var resultDto = PostingMapper.ToPostingGetDto(savedObject);
    return Results.Created($"/postings/{resultDto.Id}", resultDto);
});

app.MapPut("/postings/{id}", (int id, PostingPutDto putDto, IPostingService postingService) =>
{
    if (id != putDto.Id) return Results.BadRequest();

    var postingToUpdate = new Posting
    {
        Id = putDto.Id,
        From = putDto.From,
        To = putDto.To,
        Content = putDto.Content,
        DeliveryType = putDto.DeliveryType,
        Weight = putDto.Weight,
        Width = putDto.Width,
        Height = putDto.Height,
        Depth = putDto.Depth,
        Value = putDto.Value,
        Price = putDto.Price
    };

    var updatedPosting = postingService.Update(postingToUpdate);
    return updatedPosting == null ? Results.NotFound() : Results.Ok(PostingMapper.ToPostingGetDto(updatedPosting));
});

app.MapDelete("/postings/{id}", (int id, IPostingService postingService) =>
    postingService.Delete(id) ? Results.NoContent() : Results.NotFound());

app.Run();