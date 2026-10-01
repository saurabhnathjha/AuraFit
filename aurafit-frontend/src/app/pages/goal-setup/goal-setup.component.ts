import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GoalService } from '../../services/goal.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule } from '@angular/material/snack-bar';

@Component({
  selector: 'app-goal-setup',
  templateUrl: './goal-setup.component.html',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatCheckboxModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule,
  ],
})
export class GoalSetupComponent implements OnInit {
  goalType = '';
  weeklyTargetKg: number | null = null;
  targetCalories: number | null = null;
  autoSuggested = false; // 👈 default now false

  errorMessage = '';

  goalTypes = ['Lose Weight', 'Maintain Weight', 'Gain Muscle'];

  constructor(private goalService: GoalService, private router: Router) { }

  ngOnInit(): void {
    this.goalService.getGoal().subscribe({
      next: (goal) => {
        if (goal) {
          this.goalType = goal.goalType || '';
          this.weeklyTargetKg = goal.weeklyTargetKg ?? null;
          this.targetCalories = goal.targetCalories ?? null;
          this.autoSuggested = goal.autoSuggested ?? false;

          // Handle maintain weight pre-selection
          if (this.goalType === 'Maintain Weight') {
            this.weeklyTargetKg = 0;
          }
        }
      },
      error: () => {
        // Fine to leave empty
      }
    });
  }

  toggleAutoSuggested() {
    if (this.autoSuggested) {
      // Switching to auto-suggest
      this.goalType = '';
      this.weeklyTargetKg = null;
      this.targetCalories = null;
    } else {
      // Switching to manual — do not toggle again
      this.goalType = '';
      this.weeklyTargetKg = null;
      this.targetCalories = null;
    }
  }

  onGoalTypeChange() {
    if (this.goalType === 'Maintain Weight') {
      this.weeklyTargetKg = 0;
    }
  }

  submitGoal() {
    if (!this.autoSuggested && (!this.goalType || this.weeklyTargetKg === null || this.targetCalories === null)) {
      this.errorMessage = 'Please fill in all goal fields or choose auto-suggestion.';
      return;
    }

    const dto = {
      goalType: this.goalType,
      weeklyTargetKg: this.weeklyTargetKg,
      targetCalories: this.targetCalories,
      autoSuggested: this.autoSuggested,
      goalStartDate: new Date().toISOString().split('T')[0],
    };

    this.goalService.createOrUpdateGoal(dto).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: () => this.errorMessage = 'Failed to save goal. Please try again.'
    });
  }
}
