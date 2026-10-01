// meals.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { Observable } from 'rxjs';

export interface MealLogRequest {
  name: string;
  description?: string;
  query: string;
}

export interface FoodItem {
  name: string;
  quantity?: string;
  calories: number;
}

export interface Meal {
  mealId: number;
  name: string;
  description?: string;
  loggedAt: string;
  totalCalories?: number;
  foodItems: FoodItem[];
}


@Injectable({
  providedIn: 'root'
})
export class MealsService {
  private apiUrl = environment.apiBaseUrl;

  constructor(private http: HttpClient) { }

  logMeal(dto: MealLogRequest): Observable<Meal> {
    return this.http.post<Meal>(`${this.apiUrl}/meals/log`, dto);
  }

  getMeals(): Observable<Meal[]> {
    return this.http.get<Meal[]>(`${this.apiUrl}/meals`);
  }

  getMealSummary(date: string): Observable<{ totalCaloriesConsumed: number }> {
    return this.http.get<{ totalCaloriesConsumed: number }>(`${this.apiUrl}/meals/summary?date=${date}`);
  }

  updateMeal(mealId: number, dto: MealLogRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/meals/${mealId}`, dto);
  }


  deleteMeal(mealId: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/meals/${mealId}`);
  }

}
