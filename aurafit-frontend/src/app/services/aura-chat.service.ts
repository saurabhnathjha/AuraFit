import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface CreateQueryDTO {
  query?: string;
}

@Injectable({
  providedIn: 'root'
})
export class AuraChatService {
  private apiUrl = environment.apiBaseUrl;
  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token || ''}`
    });
  }

  askQuestion(dto: CreateQueryDTO): Observable<any> {
    return this.http.post(`${this.apiUrl}/AuraChat/ask`, dto, {
      headers: this.getAuthHeaders()
    });
  }
}
