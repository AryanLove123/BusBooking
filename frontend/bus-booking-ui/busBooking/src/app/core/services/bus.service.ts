import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BusSearchParams, BusSearchResult } from '../models/bus.model';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class BusService {
  constructor(private http: HttpClient){}

  search(params: BusSearchParams): Observable<BusSearchResult[]>{
    let httpParams = new HttpParams()
      .set("source", params.source)
      .set("destination",params.destination);

    if (params.departureAfter) httpParams = httpParams.set("departureAfter", params.departureAfter);
    if (params.arrivalBefore) httpParams = httpParams.set("arrivalBefore", params.arrivalBefore);

    return this.http.get<BusSearchResult[]>(`${environment.apiBaseUrl}/buses/search`, {params: httpParams});
  }
}
