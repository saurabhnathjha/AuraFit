import { Injectable } from '@angular/core';
import { Observable, forkJoin } from 'rxjs';
import { GoalService } from '../../services/goal.service';
import { ProfileService } from '../../services/profile.service';


@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(
    private profileService: ProfileService,
    private goalService: GoalService
  ) { }

  getUserProfile(): Observable<any> {
    return this.profileService.getProfile();
  }

  createOrUpdateProfile(data: any): Observable<any> {
    return this.profileService.createOrUpdateProfile(data);
  }

  getGoal(): Observable<any> {
    return this.goalService.getGoal();
  }

  createOrUpdateGoal(data: any): Observable<any> {
    return this.goalService.createOrUpdateGoal(data);
  }


  // In UserService
  getSuggestedGoal(): Observable<{ goalType: string; weeklyTargetKg: number; targetCalories: number }> {
    return this.goalService.getSuggestedGoal();
  }

  getUserData(): Observable<any> {
    // Combine profile and goal
    return forkJoin({
      profile: this.getUserProfile(),
      goal: this.getGoal()
    });
  }
}
