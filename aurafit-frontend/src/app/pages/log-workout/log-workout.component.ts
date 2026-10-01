import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormArray,
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatChipsModule } from '@angular/material/chips';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { MatDividerModule } from '@angular/material/divider';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatTabsModule } from '@angular/material/tabs';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { ExerciseCatalogService } from '../../services/exercise-catalog.service';
import {
  WorkoutLogService,
  WorkoutLogCreateDTO,
  WorkoutLogReadDTO
} from '../../services/log-workout.service';
import { DialogComponent } from '../../shared/components/dialog/dialog.component';

@Component({
  selector: 'app-log-workout',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatCardModule,
    MatToolbarModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatDividerModule,
    MatDatepickerModule,
    MatNativeDateModule,
    MatTabsModule,
    MatExpansionModule,
    MatChipsModule,
    MatIconModule,
    MatSnackBarModule,
    MatDialogModule,
    DialogComponent
  ],
  templateUrl: './log-workout.component.html',
  styleUrls: ['./log-workout.component.css']
})
export class LogWorkoutComponent implements OnInit {
  workoutForm: FormGroup;
  exerciseOptions: any[] = [];
  pastWorkouts: WorkoutLogReadDTO[] = [];
  showForm = true;
  currentEditId: number | null = null;
  isSmallScreen = false;

  constructor(
    private fb: FormBuilder,
    private exerciseCatalogService: ExerciseCatalogService,
    private workoutLogService: WorkoutLogService,
    private router: Router,
    private route: ActivatedRoute,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) {
    const today = new Date();
    this.workoutForm = this.fb.group({
      workoutTemplateId: [null],
      loggedAt: [today, Validators.required],
      durationMin: [null],
      caloriesBurned: [null],
      exercises: this.fb.array([])
    });
  }

  ngOnInit(): void {
    this.isSmallScreen = window.innerWidth <= 600;
    window.addEventListener('resize', () => {
      this.isSmallScreen = window.innerWidth <= 600;
    });
    this.loadExerciseOptions();
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id) {
        this.loadWorkoutForEdit(parseInt(id, 10));
      } else {
        this.addExercise();
      }
    });
    this.loadPastWorkouts();
  }

  loadExerciseOptions(): void {
    this.exerciseCatalogService.getExerciseCatalog().subscribe({
      next: data => (this.exerciseOptions = data),
      error: err => console.error('Failed to load exercises', err)
    });
  }

  loadPastWorkouts(): void {
    const start = new Date();
    start.setDate(start.getDate() - 30);
    const end = new Date();

    this.workoutLogService.getWorkoutsInRange(
      start.toISOString(),
      end.toISOString()
    ).subscribe({
      next: logs => (this.pastWorkouts = logs),
      error: err => console.error('Failed to fetch past workouts', err)
    });
  }

  loadWorkoutForEdit(id: number): void {
    this.currentEditId = id;
    this.workoutLogService.getWorkoutById(id).subscribe({
      next: log => this.onEditWorkout(log),
      error: err => console.error('Failed to load workout for editing', err)
    });
  }

  get exercises(): FormArray {
    return this.workoutForm.get('exercises') as FormArray;
  }

  getExerciseName(id: number): string {
    const found = this.exerciseOptions.find(e => e.exerciseCatalogId === id);
    return found ? found.name : 'Unknown';
  }

  getCatalogIdFromName(name: string): number | null {
    const match = this.exerciseOptions.find(
      opt => opt.name.trim().toLowerCase() === name.trim().toLowerCase()
    );
    return match ? match.exerciseCatalogId : null;
  }

  addExercise(): void {
    this.exercises.push(
      this.fb.group({
        exerciseCatalogId: [null, Validators.required],
        reps: [null],
        sets: [null],
        weightKg: [null],
        durationMin: [null],
        personalRecord: [false],
        notes: [''],
        caloriesBurned: [null]
      })
    );
  }

  removeExercise(index: number): void {
    this.exercises.removeAt(index);
  }

  resetForm(): void {
    this.currentEditId = null;
    this.workoutForm.reset();
    this.exercises.clear();
    this.addExercise();
  }

  onEditWorkout(log: WorkoutLogReadDTO): void {
    this.currentEditId = log.workoutLogId;
    this.exercises.clear();

    for (const ex of log.exercises) {
      const catalogId = this.getCatalogIdFromName(ex.exerciseName);
      if (catalogId == null) {
        console.warn(`⚠️ No catalogId found for "${ex.exerciseName}"`);
        continue;
      }

      this.exercises.push(
        this.fb.group({
          exerciseCatalogId: [catalogId, Validators.required],
          reps: [ex.reps ?? null],
          sets: [ex.sets ?? null],
          weightKg: [ex.weightKg ?? null],
          durationMin: [ex.durationMin ?? null],
          personalRecord: [ex.personalRecord ?? false],
          notes: [ex.notes ?? ''],
          caloriesBurned: [ex.caloriesBurned ?? null]
        })
      );
    }

    this.workoutForm.patchValue({
      loggedAt: new Date(log.loggedAt),
      durationMin: log.durationMin ?? null,
      caloriesBurned: log.caloriesBurned ?? null
    });

    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  onDeleteWorkout(workoutLogId: number): void {
    const dialogRef = this.dialog.open(DialogComponent, {
      width: '350px',
      data: {
        title: 'Confirm Deletion',
        message: 'Are you sure you want to delete this workout?',
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe(result => {
      if (result) {
        this.workoutLogService.deleteWorkout(workoutLogId).subscribe({
          next: () => {
            this.pastWorkouts = this.pastWorkouts.filter(log => log.workoutLogId !== workoutLogId);
            this.snackBar.open('Workout deleted!', 'Close', { duration: 3000 });
          },
          error: err => console.error('Failed to delete workout', err)
        });
      }
    });
  }

  submit(): void {
    if (this.workoutForm.invalid) {
      this.workoutForm.markAllAsTouched();
      return;
    }

    const workoutDto: WorkoutLogCreateDTO = this.workoutForm.value;

    if (this.currentEditId !== null) {
      this.workoutLogService.updateWorkout(this.currentEditId, workoutDto).subscribe({
        next: () => {
          console.log('Workout updated');
          this.resetForm();
          this.loadPastWorkouts();
        },
        error: err => console.error('Failed to update workout', err)
      });
    } else {
      this.workoutLogService.logWorkout(workoutDto).subscribe({
        next: () => {
          console.log('Workout logged');
          this.loadPastWorkouts();
          this.resetForm();
          this.showForm = false;
          this.snackBar.open('Workout logged successfully!', 'Close', {
            duration: 3000
          });
        },
        error: err => console.error('Failed to log workout', err)
      });
    }
  }
}
