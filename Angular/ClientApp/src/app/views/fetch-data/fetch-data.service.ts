import { Inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WeatherForecast } from './fetch-data';
import { MessageService } from '../../messages/messages.service';

@Injectable({
  providedIn: 'root',
})
export class WeatherService {
  public forecasts: WeatherForecast[] = [];

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string, private messageService: MessageService) {
    this.baseUrl = baseUrl + 'weatherforecast';

    console.log('WeatherForecastUrl: ' + baseUrl);
  }

  getForecasts(): Observable<WeatherForecast[]> {
    this.messageService.add('Fetched the Forecasts');
    return this.http.get<WeatherForecast[]>(this.baseUrl);
  }
}
