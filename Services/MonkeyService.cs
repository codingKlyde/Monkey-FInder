using System.Net.Http.Json;

namespace Siklab.Services
{
    public class MonkeyService
    {
        HttpClient httpClient;
        List<Monkey> monkeyList = new();

        public MonkeyService()
        {
            httpClient = new HttpClient();
        }

        public async Task<List<Monkey>> GetMonkeys()
        {
            if (monkeyList?.Count > 0)
                return monkeyList;
           
            var url = "https://www.montemagno.com/monkeys.json";
            var response = await httpClient.GetAsync(url);
            /* IF THE JSON FILE IS IN THE APP, USE THIS FOR LOCAL READING
            * using var stream = await FileSystem.OpenAppPackageFileAsync("monkeydata.json");
            * using var reader = new StreamReader(stream);
            * var contents = await reader.ReadToEndAsync();
            * monkeyList = JsonSerializer.Deserialize(contents, MonkeyContext.Default.ListMonkey);
            */

            if (response.IsSuccessStatusCode)
                monkeyList = await response.Content.ReadFromJsonAsync<List<Monkey>>();

            return monkeyList;
        }
    }
}