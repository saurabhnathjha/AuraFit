import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService } from '../../services/auth.service';
import { UserService } from '../../shared/services/user.service';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { MatTooltip, MatTooltipModule } from '@angular/material/tooltip';
import { DialogComponent } from '../../shared/components/dialog/dialog.component';

@Component({
  selector: 'app-user',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatCheckboxModule,
    MatSnackBarModule,
    MatDialogModule,
    MatTooltip
  ],
  templateUrl: './user.component.html',
  styleUrls: ['./user.component.css'],
})
export class UserComponent implements OnInit {
  profile: any;
  goal: any;

  editProfileMode = false;
  editGoalMode = false;

  activityLevels = ['Sedentary', 'Lightly Active', 'Moderately Active', 'Very Active'];

  private router = inject(Router);

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadUserData();
  }

  loadUserData(): void {
    this.userService.getUserData().subscribe({
      next: (data) => {
        this.profile = data.profile;
        this.goal = data.goal;
      },
      error: (err) => {
        console.error('Failed to load user data', err);
        this.profile = null;
        this.goal = null;
      }
    });
  }

  updateProfile(): void {
    this.userService.createOrUpdateProfile(this.profile).subscribe({
      next: (updated) => {
        this.profile = updated;
        this.editProfileMode = false;

        const dialogRef = this.dialog.open(DialogComponent, {
          data: {
            title: 'Auto-Suggest Goal',
            message: 'Profile updated! Would you like us to auto-suggest a goal based on your updated data?',
            confirmText: 'Yes',
            cancelText: 'No'
          }
        });

        dialogRef.afterClosed().subscribe((result: boolean) => {
          if (result === true) {
            this.userService.getSuggestedGoal().subscribe({
              next: (suggestion) => {
                const newGoal = {
                  ...suggestion,
                  autoSuggested: true,
                  goalStartDate: new Date().toISOString().split('T')[0]
                };

                this.userService.createOrUpdateGoal(newGoal).subscribe({
                  next: (updatedGoal) => {
                    this.goal = updatedGoal;
                    this.snackBar.open('Goal updated with auto-suggested values!', 'Close', { duration: 3000 });
                  },
                  error: (err) => {
                    console.error("Failed to update goal with suggestion", err);
                    this.snackBar.open('Auto goal update failed. Try manually.', 'Close', { duration: 3000 });
                  }
                });
              },
              error: (err) => {
                console.error("Failed to fetch suggested goal", err);
                this.snackBar.open('Could not get auto-suggested goal.', 'Close', { duration: 3000 });
              }
            });
          } else {
            console.log('User declined auto-suggestion.');
          }
        });

      },
      error: (err) => {
        console.error("Failed to update profile", err);
        this.snackBar.open('Profile update failed.', 'Close', { duration: 3000 });
      }
    });
  }

  updateGoal(): void {
    this.userService.createOrUpdateGoal(this.goal).subscribe({
      next: (updated) => {
        this.goal = updated;
        this.editGoalMode = false;
        this.snackBar.open('Goal updated successfully!', 'Close', { duration: 3000 });
      },
      error: (err) => {
        console.error('Failed to update goal', err);
        this.snackBar.open('Goal update failed.', 'Close', { duration: 3000 });
      }
    });
  }

  onGoalTypeChange(newType: string): void {
    if (newType === 'Maintain') {
      this.goal.weeklyTargetKg = 0;
    }
  }

  onGoalAutoSuggestToggle(): void {
    if (this.goal.autoSuggested) {
      this.userService.getSuggestedGoal().subscribe({
        next: (suggestion) => {
          this.goal.goalType = suggestion.goalType;
          this.goal.weeklyTargetKg = suggestion.goalType === 'Maintain' ? 0 : suggestion.weeklyTargetKg;
          this.goal.targetCalories = suggestion.targetCalories;
        },
        error: () => {
          this.goal.autoSuggested = false;
          console.error('Failed to fetch suggested goal.');
          this.snackBar.open('Failed to auto-suggest goal.', 'Close', { duration: 3000 });
        }
      });
    }
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/login']);
  }
}
