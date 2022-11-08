using System.Text.Json;
using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;

namespace CommandsService.EventProcessing
{
  public class EventProcessor : IEventProcessor
  {
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IMapper _mapper;

    public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
    {
      _scopeFactory = scopeFactory;
      _mapper = mapper;
    }
    public void ProcessEvent(string message)
    {
      var eventType = DetermineEvent(message);


      switch (eventType)
      {
        case EventType.PlatformPublished:
          // To Do
          break;
        default:
          break;
      }
    }

    private EventType DetermineEvent(string notificationMessage)
    {
      Console.WriteLine("--> Determining Event Type");

      var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

      switch (eventType.Event)
      {
        case "Platform_Published":
          Console.WriteLine("--> Platform Published Event detected");
          return EventType.PlatformPublished;
        default:
          Console.WriteLine("--> Could not determine event type");
          return EventType.Undetermined;
      }
    }

    private void AddPlatform(string platformPublishedMessage)
    {
      using (var scope = _scopeFactory.CreateScope())
      {
        var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();

        var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

        try
        {
          var plat = _mapper.Map<Platform>(platformPublishedDto);
          if (!repo.ExternalPlatformExists(plat.ExternalId))
          {
            repo.CreatePlatform(plat);
            repo.SaveChanges();
          }
          else
          {
            Console.WriteLine("--> Platform already exitss");
          }

        }
        catch (System.Exception ex)
        {
          Console.WriteLine($"--> Could not add platform to teh db {ex.Message}");
        }

      }
    }
  }

  enum EventType
  {
    PlatformPublished,
    Undetermined
  }
}