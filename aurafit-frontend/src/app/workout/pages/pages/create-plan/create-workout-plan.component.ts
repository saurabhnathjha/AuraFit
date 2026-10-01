

//*********************************new code********************************************
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { WorkoutService } from '../../../services/workout.service';
import { MatSnackBar } from '@angular/material/snack-bar';

@Component({
  selector: 'app-create-workout-plan',
  templateUrl: './create-workout-plan.component.html',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatCheckboxModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatIconModule
  ],
  styleUrls: ['./create-workout-plan.component.scss']
})
export class CreateWorkoutPlanComponent implements OnInit {
  exercises: any[] = [];
  weekDays: string[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
  planName: string = '';
  selectedExercises: { [day: string]: number[] } = {};

  successMessageShown = false;
  todaysWorkouts: any[] = [];

  constructor(private workoutService: WorkoutService, private router: Router,private snackBar: MatSnackBar) { }

  ngOnInit(): void {
    this.workoutService.getAllExercises().subscribe(data => {
      this.exercises = data;
      console.log("Exercises received:", this.exercises);
      this.weekDays.forEach(day => {
        this.selectedExercises[day] = [];
      });
    });
  }

  toggleExercise(day: string, exerciseId: number): void {
    const list = this.selectedExercises[day];
    const index = list.indexOf(exerciseId);
    if (index > -1) {
      list.splice(index, 1);
    } else {
      list.push(exerciseId);
    }
  }

  onSelectionChange(day: string, selectedIds: number[]): void {
    this.selectedExercises[day] = selectedIds;
  }

 
  submitPlan(): void {
    const allDaysFilled = this.weekDays.every(day => this.selectedExercises[day]?.length > 0);

    if (!this.planName.trim()) {
      //alert('Please enter a plan name.');
      this.snackBar.open('Please enter a Plan Name.', 'Close', { duration: 3000 });

      return;
    }

    if (!allDaysFilled) {
      //alert('Please select at least one exercise for each day.');
      this.snackBar.open('Please select at least one exercise for each day.', 'Close', { duration: 3000 });
      return;
    }

    //const userId = Number(localStorage.getItem('userId'));
    const userId = Number(sessionStorage.getItem('userId'));
    console.log("userId is :", userId);

    if (!userId) {
      console.error('User ID not found in localStorage. User might not be logged in.');
      return;
    }
    const dailyWorkouts = this.weekDays.flatMap(day =>
      this.selectedExercises[day].map(exId => ({
        dayOfWeek: day,
        exerciseId: exId
      }))
    );

    const payload = {
      userId,
      planName: this.planName,
      dailyWorkouts
    };

    this.workoutService.createWeeklyPlan(payload).subscribe({
      next: () => {
        this.successMessageShown = true;

        // Wait 5 seconds then redirect
        setTimeout(() => {
          this.router.navigate(['/yourworkout/today']); // ✅ CORRECT
        }, 5000);
      },
      error: err => console.error('API Error:', err)
    });
  }


  loadTodaysPlan(dailyWorkouts: any[]): void {
    const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
    const today = days[new Date().getDay()];
    this.todaysWorkouts = dailyWorkouts.filter(w => w.dayOfWeek === today);
    this.router.navigate(['/your-workout/today'], { state: { workouts: this.todaysWorkouts } });
  }
}
