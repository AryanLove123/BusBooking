import { computed, Injectable, signal } from '@angular/core';
import { LoginRequest, LoginResponse, RegisterRequest } from '../models/user.model';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { environment } from '../../environments/environment';

const STORAGE_KEY = "busbooking_auth";
@Injectable({
  providedIn: 'root',
})
export class AuthService {
  _currentUser = signal<LoginResponse | null >(this.readStorage());
  isLoggedIn = computed(() => !!this._currentUser());
  isAdmin = computed(() => this._currentUser()?.role == "Admin");

  constructor(private http: HttpClient) {}

  register(request:RegisterRequest): Observable<LoginResponse>{
    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/auth/register`, request)
      .pipe((tap((res) => this.setSession(res))));
  }
  
  login(request: LoginRequest): Observable<LoginResponse>{
    return this.http.post<LoginResponse>(`${environment.apiBaseUrl}/auth/login`, request)
      .pipe(tap((res => this.setSession(res))));
  }

  logout(): void{
    localStorage.removeItem(STORAGE_KEY);
    this._currentUser.set(null);
  }

  getToken(): string | null {
    return this._currentUser()?.token ?? null; 
  }

  setSession(res: LoginResponse): void{
    localStorage.setItem(STORAGE_KEY,JSON.stringify(res));
    this._currentUser.set(res);
  }

  readStorage(): LoginResponse | null {
    const raw = localStorage.getItem(STORAGE_KEY);
    if(!raw) return null;

    try{
      const parsed = JSON.parse(raw) as LoginResponse;
      if(new Date(parsed.expiresAtUtc).getTime() < Date.now()){
        localStorage.removeItem(STORAGE_KEY);
        return null;
      }
      return parsed; 
    }catch{
      return null;
    }
  }
}
