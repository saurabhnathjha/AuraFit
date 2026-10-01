import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GoalService {
  private apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token}`
    });
  }

  getGoal(): Observable<any> {
    return this.http.get(`${this.apiUrl}/usergoals/me`, {
      headers: this.getAuthHeaders()
    });
  }

  getSuggestedGoal(): Observable<any> {
    return this.http.get(`${this.apiUrl}/usergoals/suggested`, {
      headers: this.getAuthHeaders()
    });
  }


  createOrUpdateGoal(dto: any): Observable<any> {
    return this.http.post(`${this.apiUrl}/usergoals`, dto, {
      headers: this.getAuthHeaders()
    });
  }
}
