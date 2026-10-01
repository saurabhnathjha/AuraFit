// src/app/pages/profile-setup/profile-setup.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileService } from '../../services/profile.service';
import { GoalService } from '../../services/goal.service';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBarModule, MatSnackBar } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { DialogComponent } from '../../shared/components/dialog/dialog.component';

@Component({
  selector: 'app-profile-setup',
  templateUrl: './profile-setup.component.html',
  styleUrls: ['./profile-setup.component.css'],
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatSnackBarModule,
    MatDialogModule,
    DialogComponent
  ],
})
export class ProfileSetupComponent {
  age: number | null = null;
  gender = '';
  heightCm: number | null = null;
  weightKg: number | null = null;
  activityLevel = '';
  errorMessage = '';

  genders = ['Male', 'Female', 'Other'];
  activityLevels = ['Sedentary', 'Light', 'Moderate', 'Active', 'Very Active'];

  constructor(
    private profileService: ProfileService,
    private goalService: GoalService,
    private router: Router,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  removeLeadingZero(field: 'age' | 'heightCm' | 'weightKg') {
    const value = this[field];
    this[field] = value !== null && value !== undefined ? Number(value) || null : null;
  }

  submitProfile() {
    if (
      !this.age || this.age < 1 ||
      !this.heightCm || this.heightCm < 30 ||
      !this.weightKg || this.weightKg < 10 ||
      !this.gender ||
      !this.activityLevel
    ) {
      this.errorMessage = 'Please fill in all required fields correctly.';
      return;
    }

    const dto = {
      age: this.age,
      gender: this.gender,
      heightCm: this.heightCm,
      weightKg: this.weightKg,
      activityLevel: this.activityLevel,
    };

    this.profileService.createOrUpdateProfile(dto).subscribe({
      next: () => {
        const dialogRef = this.dialog.open(DialogComponent, {
          data: {
            title: 'Auto Goal Suggestion',
            message: 'Would you like us to auto-suggest your goal?',
            confirmText: 'Yes',
            cancelText: 'No'
          }
        });

        dialogRef.afterClosed().subscribe(result => {
          if (result) {
            const goalDto = {
              goalType: 'placeholder',
              weeklyTargetKg: 0,
              targetCalories: 0,
              autoSuggested: true,
              goalStartDate: new Date().toISOString().split('T')[0],
            };

            this.goalService.createOrUpdateGoal(goalDto).subscribe({
              next: () => this.router.navigate(['/dashboard']),
              error: err => {
                console.error(err);
                this.errorMessage = 'Auto-goal setup failed. Please set it manually.';
                this.router.navigate(['/goal-setup']);
              }
            });
          } else {
            this.router.navigate(['/goal-setup']);
          }
        });
      },
      error: () => {
        this.errorMessage = 'Failed to save profile. Please try again.';
      }
    });
  }
}
