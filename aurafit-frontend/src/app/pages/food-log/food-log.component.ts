import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatCardModule } from '@angular/material/card';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatDividerModule } from '@angular/material/divider';
import { PageEvent } from '@angular/material/paginator';
import { MatPaginatorModule } from '@angular/material/paginator';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';

import { Meal, MealsService, MealLogRequest } from '../../services/meals.service';
import { DialogComponent } from '../../shared/components/dialog/dialog.component';

@Component({
  selector: 'app-food-log',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatProgressSpinnerModule,
    MatCardModule,
    MatIconModule,
    MatSnackBarModule,
    MatDividerModule,
    MatPaginatorModule,
    MatChipsModule,
    MatSelectModule,
    MatAutocompleteModule,
    MatExpansionModule,
    MatToolbarModule,
    MatDialogModule,
    DialogComponent
  ],
  templateUrl: './food-log.component.html',
  styleUrls: ['./food-log.component.css']
})
export class FoodLogComponent implements OnInit {
  meals: Meal[] = [];
  mealDescription = '';
  mealQuery = '';
  loading = false;
  mealType: string = '';
  predefinedMealTypes: string[] = ['Breakfast', 'Lunch', 'Dinner', 'Snack'];

  showLogForm = false;
  submitted = false;

  editingMeal: Meal | null = null;
  editName = '';
  editDescription = '';
  editQuery = '';

  pageSize = 3;
  pagedMeals: Meal[] = [];

  constructor(
    private mealsService: MealsService,
    private snackBar: MatSnackBar,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.loadMeals();
  }

  toggleForm(): void {
    this.showLogForm = !this.showLogForm;
  }

  loadMeals(): void {
    this.mealsService.getMeals().subscribe({
      next: (meals) => {
        this.meals = meals;
        this.pagedMeals = this.meals.slice(0, this.pageSize);
      },
      error: () => this.showError('Failed to load meals.')
    });
  }

  logMeal(): void {
    const now = new Date();
    const hour = now.getHours();
    let finalMealType = this.mealType;
    if (this.mealType === 'Snack' && hour >= 0 && hour <= 3) {
      finalMealType = 'Midnight Snack';
    }

    const mealToLog = {
      name: finalMealType,
      description: this.mealDescription,
      query: this.mealQuery
    };

    this.loading = true;

    this.mealsService.logMeal(mealToLog).subscribe({
      next: () => {
        this.mealDescription = '';
        this.mealQuery = '';
        this.mealType = '';
        this.loading = false;
        this.submitted = true;
        setTimeout(() => (this.submitted = false), 2000);

        const form = document.querySelector('form') as HTMLFormElement;
        form?.reset();

        this.snackBar.open('Meal logged successfully!', 'Close', { duration: 3000 });
        this.loadMeals();
      },
      error: () => {
        this.loading = false;
        this.snackBar.open('Failed to log meal. Try again.', 'Close', { duration: 3000 });
      }
    });
  }

  deleteMeal(mealId: number): void {
    const dialogRef = this.dialog.open(DialogComponent, {
      width: '350px',
      data: {
        title: 'Delete Meal',
        message: 'Are you sure you want to delete this meal?',
        confirmText: 'Delete',
        cancelText: 'Cancel'
      }
    });

    dialogRef.afterClosed().subscribe((confirmed: boolean) => {
      if (confirmed) {
        this.mealsService.deleteMeal(mealId).subscribe({
          next: () => {
            this.snackBar.open('Meal deleted', 'Close', { duration: 3000 });
            this.loadMeals();
          },
          error: () => this.showError('Failed to delete meal.')
        });
      }
    });
  }

  editMeal(meal: Meal): void {
    this.editingMeal = meal;
    this.editName = meal.name;
    this.editDescription = meal.description || '';
    this.editQuery = meal.foodItems.map(f => `${f.quantity} ${f.name}`).join(', ');
  }

  cancelEdit(): void {
    this.editingMeal = null;
    this.editName = '';
    this.editDescription = '';
    this.editQuery = '';
  }

  onPageChange(event: PageEvent): void {
    const start = event.pageIndex * event.pageSize;
    const end = start + event.pageSize;
    this.pagedMeals = this.meals.slice(start, end);
  }

  updateMeal(mealId: number): void {
    if (!this.editName.trim() || !this.editQuery.trim()) {
      this.showError('Meal name and query are required.');
      return;
    }

    const updatedMeal: MealLogRequest = {
      name: this.editName.trim(),
      description: this.editDescription.trim() || undefined,
      query: this.editQuery.trim()
    };

    this.mealsService.updateMeal(mealId, updatedMeal).subscribe({
      next: () => {
        this.snackBar.open('Meal updated successfully!', 'Close', { duration: 3000 });
        this.editingMeal = null;
        this.loadMeals();
      },
      error: () => this.showError('Failed to update meal.')
    });
  }

  calculateCalories(meal: Meal): number {
    return meal.foodItems?.reduce((sum, item) => sum + (item.calories || 0), 0) || 0;
  }

  private showError(message: string): void {
    this.snackBar.open(message, 'Close', { duration: 3000, panelClass: 'snack-error' });
  }
}
