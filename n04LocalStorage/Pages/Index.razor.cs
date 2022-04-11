using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using n04LocalStorage.Data;

namespace n04LocalStorage.Pages;

public partial class Index
{
    [Inject] ILocalStorageService? localStore { get; set; }

    const string noteKey = "note";
    string? noteContent;

    public async void UpdateLocalStorage()
    {
        await localStore!.SetItemAsync(noteKey, noteContent);
    }

    public async void ClearLocalStorage()
    {
        noteContent = "";
        await localStore!.ClearAsync();
    }


    private List<WeatherForecast>? forecasts;
    private WeatherForecast? one=new WeatherForecast();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            noteContent = await localStore!.GetItemAsync<string>(noteKey);

            forecasts = await localStore!.GetItemAsync<List<WeatherForecast>>("forecasts");
            if (forecasts==null)
            {
                forecasts = new List<WeatherForecast>();
                await localStore!.SetItemAsync("forecasts", forecasts);
            }

            StateHasChanged();

        }
    }

    async void Add()
    {
        forecasts!.Add(one!);
        one = new WeatherForecast();
        await localStore!.SetItemAsync("forecasts", forecasts);
    }
    async void Edit()
    {
        await localStore!.SetItemAsync("forecasts", forecasts);
    }
    async void Delete(WeatherForecast weather)
    {
        forecasts!.Remove(weather);
        await localStore!.SetItemAsync("forecasts", forecasts);
    } 

    async void Clear()
    {
        forecasts!.Clear();
        await localStore!.ClearAsync();
    }

}
