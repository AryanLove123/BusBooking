import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { BookingResponse, CreateBookingRequest, UpdateBookingRequest } from '../models/booking.model';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class BookingService {
  base = `${environment.apiBaseUrl}/bookings`;

  constructor(private http: HttpClient){

  }

  create(request: CreateBookingRequest): Observable<BookingResponse>{
    return this.http.post<BookingResponse>(this.base,request);
  }

  getMine(): Observable<BookingResponse[]>{
    return this.http.get<BookingResponse[]>(`${this.base}/my`);
  }

  getById(id: number): Observable<BookingResponse> {
    return this.http.get<BookingResponse>(`${this.base}/${id}`);
  }

  update(id: number, request: UpdateBookingRequest): Observable<BookingResponse> {
    return this.http.put<BookingResponse>(`${this.base}/${id}`, request);
  }

  cancel(id: number): Observable<BookingResponse> {
    return this.http.delete<BookingResponse>(`${this.base}/${id}`);
  }
}
