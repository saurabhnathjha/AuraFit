import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { WorkoutService } from '../../../services/workout.service';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatListModule } from '@angular/material/list';
import { MatIconModule } from '@angular/material/icon';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { NgIf, NgFor } from '@angular/common'; // Angular built-ins if not already present
import { FormsModule } from '@angular/forms';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatButtonModule } from '@angular/material/button';

import { WorkoutLogService, WorkoutLogCreateDTO, ExerciseDTO } from '../../../../services/log-workout.service';


@Component({
  selector: 'app-today-workout-plan',
  templateUrl: './today-workout-plan.component.html',
  standalone: true,
  imports: [ 
    CommonModule,
    MatProgressSpinnerModule,
    MatListModule,
    MatIconModule,
    CommonModule,
    NgIf,
    NgFor,
    MatCardModule,
    MatIconModule,
    MatListModule,
    MatProgressSpinnerModule,
    MatDividerModule,
    FormsModule,
    MatSnackBarModule // 👈 ADD THIS

  ],
  styleUrls: ['./today-workout-plan.component.scss']
})
export class TodayWorkoutPlanComponent implements OnInit {
  todayExercises: any[] = [];
  exercisesCatalog: any[] = [];
  loading = true;
  showAddSection = false;
  selectedExerciseIds: number[] = [];
  showValidationError = false;
  showLogWorkoutSection = false;
  loggedDurations: { [exerciseName: string]: number } = {}; 

  constructor(
    private workoutService: WorkoutService,
    private snackBar: MatSnackBar,
    private workoutLogService: WorkoutLogService, 

  ) { }

  ngOnInit(): void {
    this.loadExerciseCatalogAndTodayWorkout();
  }

  toggleAddExercises(): void {
    this.showAddSection = !this.showAddSection;
  }

  toggleLogWorkout(): void {
    this.showLogWorkoutSection = !this.showLogWorkoutSection;
    this.showValidationError = false; 
  }

  
  submitLoggedWorkouts(): void {
    //const userId = Number(localStorage.getItem('userId'));
    const userId = Number(sessionStorage.getItem('userId'));

    const allFilled = this.todayExercises.every(ex =>
      this.loggedDurations[ex.exerciseName] &&
      this.loggedDurations[ex.exerciseName] > 0
    );

    if (!allFilled) {
      this.showValidationError = true;
      return;
    }

    this.showValidationError = false;

    // Optional logging for debugging only
    const debugLog = this.todayExercises.map(ex => ({
      exerciseName: ex.exerciseName,
      duration: this.loggedDurations[ex.exerciseName]
    }));

    console.log('Logged Workouts:', debugLog);

    if (!userId) {
      console.error('User not found.');
      return;
    }

    const exercises: ExerciseDTO[] = Object.entries(this.loggedDurations).map(([exerciseName, duration]) => {
      const match = this.exercisesCatalog.find(e => e.name === exerciseName);
      if (!match) {
        console.warn(`Exercise "${exerciseName}" not found in catalog.`);
      }

      return {
        exerciseCatalogId: match?.exerciseCatalogId ?? null,
        durationMin: duration,
        personalRecord: false
      };
    });

    const workoutLogDto: WorkoutLogCreateDTO = {
      loggedAt: new Date().toISOString(),
      durationMin: exercises.reduce((sum, ex) => sum + (ex.durationMin || 0), 0),
      exercises
    };

    this.workoutLogService.logWorkout(workoutLogDto).subscribe({
      next: () => {
        this.snackBar.open('✅ Workout logged successfully!', 'Close', {
          duration: 3000,
          panelClass: ['snackbar-success']
        });
        this.toggleLogWorkout(); 
      },
      error: err => {
        console.error('Error logging workout:', err);
        this.snackBar.open('❌ Failed to log workout.', 'Close', {
          duration: 3000,
          panelClass: ['snackbar-error']
        });
      }
    });
  }


  loadExerciseCatalogAndTodayWorkout(): void {
    const userId = Number(localStorage.getItem('userId'));
    const date = new Date().toISOString().split('T')[0];

    if (!userId) {
      console.error('User ID not found in storage.');
      this.loading = false;
      return;
    }

    this.workoutService.getAllExercises().subscribe({
      next: (catalog) => {
        this.exercisesCatalog = catalog;

        this.workoutService.getTodayWorkout(userId, date).subscribe({
          next: (workoutData) => {
            if (!Array.isArray(workoutData)) {
              console.error('Expected workoutData to be an array');
              this.todayExercises = [];
              this.loading = false;
              return;
            }

            this.todayExercises = workoutData.map(w => {
              const match = this.exercisesCatalog.find(e => +e.exerciseCatalogId === +w.exerciseId);
              return {
                ...w,
                exerciseName: match?.name ?? 'Unknown'
              };
            });

            this.loading = false;
          },
          error: err => {
            console.error("Error loading today's workout:", err);
            this.loading = false;
          }
        });
      },
      error: err => {
        console.error("Error loading exercise catalog:", err);
        this.loading = false;
      }
    });
  }

  removeExerciseFromToday(exerciseId: number): void {
    const userId = Number(localStorage.getItem('userId'));
    const dayOfWeek = new Date().toLocaleDateString('en-US', { weekday: 'long' });

    if (!userId) {
      console.error('User ID not found.');
      return;
    }

    if (!confirm('Are you sure you want to remove this exercise from today\'s workout?')) return;

    this.workoutService.deleteDailyWorkout(userId, exerciseId, dayOfWeek).subscribe({
      next: () => {
        this.todayExercises = this.todayExercises.filter(e => e.exerciseId !== exerciseId);
      },
      error: (err) => {
        console.error('Failed to delete exercise:', err);
      }
    });
  }

  addSelectedExercises(): void {
    const userId = Number(localStorage.getItem('userId'));
    const dayOfWeek = new Date().toLocaleDateString('en-US', { weekday: 'long' });

    if (!userId || !this.selectedExerciseIds.length) {
      alert("Select at least one exercise to add.");
      return;
    }

    const payload = {
      userId,
      dayOfWeek,
      exerciseIds: this.selectedExerciseIds
    };

    this.workoutService.addMultipleDailyWorkouts(payload).subscribe({
      next: () => {
        this.snackBar.open('Exercises added successfully!', 'Close', {
          duration: 3000,
          panelClass: ['snackbar-success']
        });

        this.selectedExerciseIds = [];
        this.showAddSection = false;
        setTimeout(() => {
          this.loadExerciseCatalogAndTodayWorkout();
        }, 300);
      },
      error: (err) => {
        console.error('Failed to add exercises:', err);

        this.snackBar.open(' Failed to add exercises.', 'Close', {
          duration: 3000,
          panelClass: ['snackbar-error']
        });

        this.showAddSection = true;
      }
    });
  }
}


//export class TodayWorkoutPlanComponent implements OnInit {
//  todayExercises: any[] = [];
//  exercisesCatalog: any[] = [];
//  loading = true;
//  showAddSection = false;
//  selectedExerciseIds: number[] = [];
//  showLogWorkoutSection = false;
//  constructor(private workoutService: WorkoutService, private snackBar: MatSnackBar) { }

//  ngOnInit(): void {
//    this.loadExerciseCatalogAndTodayWorkout();
//  }
//  toggleAddExercises(): void {
//    this.showAddSection = !this.showAddSection;
//  }
//  loadExerciseCatalogAndTodayWorkout(): void {
//    const userId = Number(localStorage.getItem('userId'));
//    const date = new Date().toISOString().split('T')[0];

//    if (!userId) {
//      console.error('User ID not found in storage.');
//      this.loading = false;
//      return;
//    }

//    // ✅ First: Load Exercise Catalog
//    this.workoutService.getAllExercises().subscribe({
//      next: (catalog) => {
//        this.exercisesCatalog = catalog;

//        // ✅ Second: Load Today's Workout after catalog is ready
//        this.workoutService.getTodayWorkout(userId, date).subscribe({
//          next: (workoutData) => {
//            if (!Array.isArray(workoutData)) {
//              console.error('Expected workoutData to be an array, but got:', typeof workoutData);
//              this.todayExercises = [];
//              this.loading = false;
//              return;
//            }

//            // ✅ Map ExerciseId to Exercise Name
//            this.todayExercises = workoutData.map(w => {
//              const match = this.exercisesCatalog.find(e => +e.exerciseCatalogId === +w.exerciseId);
//              if (!match) console.warn(`No match found for ExerciseId ${w.exerciseId}`);
//              return {
//                ...w,
//                exerciseName: match?.name ?? 'Unknown'
//              };
//            });

//            this.loading = false;
//          },
//          error: err => {
//            console.error("Error loading today's workout:", err);
//            this.loading = false;
//          }
//        });
//      },
//      error: err => {
//        console.error("Error loading exercise catalog:", err);
//        this.loading = false;
//      }
//    });
//  }


//  removeExerciseFromToday(exerciseId: number): void {
//    const userId = Number(localStorage.getItem('userId'));
//    const dayOfWeek = new Date().toLocaleDateString('en-US', { weekday: 'long' });
//    console.log(`Attempting to delete: UserId=${userId}, ExerciseId=${exerciseId}, DayOfWeek=${dayOfWeek}`);

//    if (!userId) {
//      console.error('User ID not found.');
//      return;
//    }

//    if (!confirm('Are you sure you want to remove this exercise from today\'s workout?')) return;

//    this.workoutService.deleteDailyWorkout(userId, exerciseId, dayOfWeek).subscribe({
//      next: () => {
//        this.todayExercises = this.todayExercises.filter(e => e.exerciseId !== exerciseId);
//      },
//      error: (err) => {
//        console.error('Failed to delete exercise:', err);
//      }
//    });
//  }
//  addSelectedExercises(): void {
//    const userId = Number(localStorage.getItem('userId'));
//    const dayOfWeek = new Date().toLocaleDateString('en-US', { weekday: 'long' });

//    if (!userId || !this.selectedExerciseIds.length) {
//      alert("Select at least one exercise to add.");
//      return;
//    }

//    const payload = {
//      userId,
//      dayOfWeek,
//      exerciseIds: this.selectedExerciseIds
//    };

//    console.log("Payload being sent:", payload);

//    this.showAddSection = false;

//    this.workoutService.addMultipleDailyWorkouts(payload).subscribe({
//      next: () => {
//        this.snackBar.open('✅ Exercises added successfully!', 'Close', {
//          duration: 3000, // 3 seconds
//          horizontalPosition: 'center',
//          verticalPosition: 'bottom',
//          panelClass: ['snackbar-success'] // Optional: for custom style
//        });

//        this.selectedExerciseIds = [];

//        setTimeout(() => {
//          this.loadExerciseCatalogAndTodayWorkout();
//        }, 300);
//      },
//      error: (err) => {
//        console.error('Failed to add exercises:', err);

//        this.snackBar.open('❌ Failed to add exercises.', 'Close', {
//          duration: 3000,
//          horizontalPosition: 'right',
//          verticalPosition: 'top',
//          panelClass: ['snackbar-error']
//        });

//        this.showAddSection = true;
//      }
//    });

//    loggedDurations: { [exerciseName: string]: number } = { };

//    toggleLogWorkout() {
//      this.showLogWorkoutSection = !this.showLogWorkoutSection;
//    }

//    // Send data to another service/component
//    submitLoggedWorkouts() {
//      const logData = Object.entries(this.loggedDurations).map(([name, duration]) => ({
//        exerciseName: name,
//        duration
//      }));

//      console.log('Logged Workouts:', logData); // ✅ Or send to service

//      // 👉 You can now call a service to persist this
//      // this.workoutLoggerService.logWorkouts(logData).subscribe(...)

//      this.toggleLogWorkout(); // Close modal
//    }

//  }



//}
