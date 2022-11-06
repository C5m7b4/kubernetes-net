using System.Text;
using System.Text.Json;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
  public class CommandDataClient : ICommandDataClient
  {
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public CommandDataClient(HttpClient httpClient, IConfiguration config)
    {
      _httpClient = httpClient;
      _config = config;
    }

    public async Task SendPlatformToCommand(PlatformReadDto plat)
    {
      var httpContent = new StringContent(
        JsonSerializer.Serialize(plat),
        Encoding.UTF8,
        "application/json");

      var response = await _httpClient.PostAsync(_config["CommandService"], httpContent);

      if (response.IsSuccessStatusCode)
      {
        Console.WriteLine("--> Sync POST to CommandSerice was OK!");
      }
      else
      {
        Console.WriteLine("--> Sync POST to CommandService was NOT OK!");
      }
    }
  }
}