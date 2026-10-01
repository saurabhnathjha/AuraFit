import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface ExerciseCatalogItem {
  exerciseCatalogId: number;
  name: string;
  // Add other fields as needed
}

@Injectable({
  providedIn: 'root'
})
export class ExerciseCatalogService {
  private apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token || ''}`
    });
  }

  getExerciseCatalog(): Observable<ExerciseCatalogItem[]> {
    return this.http.get<ExerciseCatalogItem[]>(`${this.apiUrl}/exercisecatalog`, {
      headers: this.getAuthHeaders()
    });
  }
}
