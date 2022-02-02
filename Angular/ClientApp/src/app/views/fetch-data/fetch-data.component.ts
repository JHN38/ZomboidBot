import { Component, OnInit } from '@angular/core';
import { WeatherForecast } from './fetch-data';
import { WeatherService } from './fetch-data.service';

@Component({
  selector: 'app-fetch-data',
  templateUrl: './fetch-data.component.html',
  styleUrls: ['./fetch-data.component.css']
})
export class FetchDataComponent implements OnInit {
  public forecasts: WeatherForecast[] = [];

  constructor(private weatherService: WeatherService) {
  }

  ngOnInit(): void {
    this.getWeather();
  }

  getWeather(): void {
    this.weatherService.getForecasts()
      .subscribe(forecasts => {
        this.forecasts = forecasts;
      }, error => console.error(error));
  }

  selectedForecast?: WeatherForecast;
  onSelect(forecast: WeatherForecast): void {
    this.selectedForecast = forecast;
  }
}
