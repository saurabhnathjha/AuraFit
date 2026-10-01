// src/app/pages/profile-setup/profile-setup.component.ts
import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileService } from '../../../services/profile.service';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-profile-setup',
  templateUrl: './profile-setup.component.html',
  standalone: true,
  imports: [CommonModule, FormsModule],
})
export class ProfileSetupComponent {
  age = 0;
  gender = '';
  heightCm = 0;
  weightKg = 0;
  activityLevel = '';

  errorMessage = '';

  genders = ['Male', 'Female', 'Other'];
  activityLevels = ['Sedentary', 'Light', 'Moderate', 'Active', 'Very Active'];

  constructor(private profileService: ProfileService, private router: Router) { }

  submitProfile() {
    const dto = {
      age: this.age,
      gender: this.gender,
      heightCm: this.heightCm,
      weightKg: this.weightKg,
      activityLevel: this.activityLevel,
    };

    this.profileService.createOrUpdateProfile(dto).subscribe({
      next: () => this.router.navigate(['/goal-setup']),
      error: () => this.errorMessage = 'Failed to save profile. Please try again.'
    });
  }
}
