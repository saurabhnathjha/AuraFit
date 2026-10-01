import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkoutService } from '../../../services/workout.service';
import { CreateWorkoutPlanComponent } from '../create-plan/create-workout-plan.component';
import { TodayWorkoutPlanComponent } from '../today-plan/today-workout-plan.component';
@Component({
  selector: 'app-workout-page',
  standalone: true,
  imports: [CommonModule, CreateWorkoutPlanComponent, TodayWorkoutPlanComponent],  // Import child components
  template: `
    <ng-container *ngIf="loading; else loadedContent">
      <p>Loading...</p>
    </ng-container>

    <ng-template #loadedContent>
      <app-create-workout-plan *ngIf="!planExists"></app-create-workout-plan>
      <app-today-workout-plan *ngIf="planExists"></app-today-workout-plan>
    </ng-template>
  `
})
export class WorkoutPageComponent implements OnInit {
  planExists = false;
  loading = true;
  //userId = Number(localStorage.getItem('userId'));
  userId = Number(sessionStorage.getItem('userId'));


  constructor(private workoutService: WorkoutService) { }

  ngOnInit(): void {
    if (!this.userId) {
      console.error('User ID not found');
      this.loading = false;
      return;
    }

    this.workoutService.doesPlanExist(this.userId).subscribe({
      next: (exists: boolean) => {
        this.planExists = exists;
        this.loading = false;
      },
      error: (err) => {
        console.error('Error checking plan existence', err);
        this.planExists = false;
        this.loading = false;
      }
    });
  }
}


