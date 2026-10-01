import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ProfileService {
  private apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  createOrUpdateProfile(dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/userprofile`, dto, {
      headers: this.getAuthHeaders()
    });
  }

  getShouldUpdateWeight(): Observable<{ shouldUpdate: boolean }> {
    return this.http.get<{ shouldUpdate: boolean }>(
      `${this.apiUrl}/userprofile/shouldUpdateWeight`
    );
  }

  getProfile(): Observable<any> {
    return this.http.get(`${this.apiUrl}/userprofile/me`, {
      headers: this.getAuthHeaders()
    });
  }
}
