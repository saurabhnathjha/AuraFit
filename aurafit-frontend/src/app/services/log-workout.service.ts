// src/app/services/workout-log.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface ExerciseDTO {
  exerciseCatalogId: number | null;
  reps?: string | null;
  sets?: number | null;
  weightKg?: number | null;
  durationMin?: number | null;
  personalRecord?: boolean;
  notes?: string;
  caloriesBurned?: number | null; // optional
}

export interface WorkoutLogCreateDTO {
  workoutTemplateId?: number | null;
  loggedAt: string;
  durationMin?: number | null;
  caloriesBurned?: number | null;
  exercises: ExerciseDTO[];
}

export interface WorkoutLogExerciseReadDTO {
  exerciseName: string;
  category: string;
  reps: number;
  sets: number;
  weightKg?: number;
  durationMin: number;
  caloriesBurned?: number;
  personalRecord: boolean;
  notes?: string;
}

export interface WorkoutLogReadDTO {
  workoutLogId: number;
  loggedAt: string;
  durationMin: number;
  caloriesBurned: number;
  workoutTemplateName?: string;
  exercises: WorkoutLogExerciseReadDTO[];
}

@Injectable({
  providedIn: 'root'
})
export class WorkoutLogService {
  private apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  private getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Authorization': `Bearer ${token || ''}`
    });
  }

  logWorkout(workoutDto: WorkoutLogCreateDTO): Observable<any> {
    return this.http.post(`${this.apiUrl}/workoutlog`, workoutDto, {
      headers: this.getAuthHeaders()
    });
  }

  getWorkoutsInRange(start: string, end: string): Observable<WorkoutLogReadDTO[]> {
    return this.http.get<WorkoutLogReadDTO[]>(
      `${this.apiUrl}/workoutlog/range?start=${start}&end=${end}`,
      { headers: this.getAuthHeaders() }
    );
  }

  updateWorkout(id: number, workoutDto: WorkoutLogCreateDTO): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/workoutlog/${id}`, workoutDto, {
      headers: this.getAuthHeaders()
    });
  }

  deleteWorkout(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/workoutlog/${id}`, {
      headers: this.getAuthHeaders()
    });
  }

  getWorkoutById(id: number): Observable<WorkoutLogReadDTO> {
    return this.http.get<WorkoutLogReadDTO>(`${this.apiUrl}/${id}`);
  }

}
