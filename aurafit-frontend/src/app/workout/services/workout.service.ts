import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({ providedIn: 'root' })
export class WorkoutService {
  private baseUrl = 'http://localhost:5126/api';

  constructor(private http: HttpClient) { }

  getAllExercises() {
    return this.http.get<any[]>(`${this.baseUrl}/ExerciseCatalog`);
  }

  createWeeklyPlan(plan: any) {
    return this.http.post(`http://localhost:5126/api/WorkoutPlan/create`, plan);
  }

  //getTodayWorkout(userId: number, date: string) {
  //  return this.http.get(`${this.baseUrl}/WorkoutPlan/daily-plan?userId=${userId}&date=${date}`);
  //}
  getTodayWorkout(userId: number, date: string) {
    return this.http.get<any[]>(`http://localhost:5126/api/WorkoutPlan/daily-plan`, {
      params: {
        userId: userId.toString(),
        date
      }
    });
  }

 
  deleteDailyWorkout(userId: number, exerciseId: number, dayOfWeek: string) {
    return this.http.delete(`${this.baseUrl}/WorkoutPlan/delete`, {
      params: {
        userId: userId.toString(),
        exerciseId: exerciseId.toString(),
        dayOfWeek
      }
    });
  }

  addMultipleDailyWorkouts(payload: any) {
    return this.http.post('http://localhost:5126/api/WorkoutPlan/add-multiple', payload, {
      responseType: 'text'
    });
  }
  doesPlanExist(userId: number) {
    return this.http.get<boolean>(`${this.baseUrl}/WorkoutPlan/exists/${userId}`);
  }


}






