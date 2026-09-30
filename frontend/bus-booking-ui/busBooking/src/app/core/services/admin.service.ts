import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AdminBookingResponse, AdminBusResponse, CreateBusRequest } from '../models/admin.model';

@Injectable({
  providedIn: 'root',
})
export class AdminService {
  base = `${environment.apiBaseUrl}/admin`;

  constructor(private http: HttpClient){}

  getBuses(): Observable<AdminBusResponse[]>{
    return this.http.get<AdminBusResponse[]>(`${this.base}/buses`);
  }

  getBookingsForBus(busId: number): Observable<AdminBookingResponse[]>{
    return this.http.get<AdminBookingResponse[]>(`${this.base}/buses/${busId}/bookings`);
  }

  createBus(request: CreateBusRequest): Observable<AdminBusResponse>{
    return this.http.post<AdminBusResponse>(`${this.base}/buses`, request);
  }

}
