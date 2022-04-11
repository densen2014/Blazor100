namespace n04LocalStorage.Data;

public class WeatherForecast
{
    public DateTime Date { get; set; }=DateTime.Now;

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; } = "阳光明媚";

    public string? SkyColor { get; set; } 

}
