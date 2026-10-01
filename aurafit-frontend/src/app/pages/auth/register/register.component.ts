import { Component } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService, RegisterRequest } from '../../../services/auth.service';
import { FormControl, FormsModule, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { FormErrorComponent } from '../../../shared/components/form-error/form-error.component';
import { MatCardModule } from '@angular/material/card';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
  standalone: true,
  imports: [FormsModule, CommonModule, RouterModule, MatFormFieldModule,
    MatInputModule, MatCardModule,
    MatButtonModule, FormErrorComponent]
})
export class RegisterComponent {
  username = '';
  password = '';
  errorMessage = '';
  successMessage = '';

  usernameControl = new FormControl('', [Validators.required]);
  passwordControl = new FormControl('', [Validators.required]);
  constructor(private authService: AuthService, private router: Router) { }

  //register(): void {
  //  const dto: RegisterRequest = {
  //    username: this.username,
  //    password: this.password,
  //  };

  //  this.authService.register(dto).subscribe({
  //    next: () => {
  //      // After successful registration, automatically login
  //      this.authService.login(dto).subscribe({
  //        next: (res) => {
  //          this.authService.storeToken(res.token);
  //          this.authService.handlePostLoginRedirect();

  //        },
  //        error: (err) => {
  //          console.error('Registration error', err);
  //          this.errorMessage = 'Registration failed. Try a different username.';
  //        },
  //      });
  //    },
  //    error: () => {
  //      this.errorMessage = 'Registration failed. Try a different username.';
  //    },
//  });
  register(): void {
    const dto: RegisterRequest = {
      username: this.username,
      password: this.password,
    };
  this.authService.register(dto).subscribe({
    next: () => {
      // After successful registration, automatically login
      this.authService.login(dto).subscribe({
        next: (res) => {
          this.authService.storeToken(res.token);

          // ✅ Fetch current user info and store userId
          this.authService.getCurrentUser().subscribe({
            next: (user) => {
              sessionStorage.setItem('userId', user.id.toString());
              console.log('✅ User ID stored in sessionStorage:', user.id);
              this.authService.handlePostLoginRedirect();
            },
            error: (err) => {
              console.error('Failed to fetch user info', err);
              this.errorMessage = 'User registered but user details not loaded.';
            }
          });
        },
        error: (err) => {
          console.error('Login after registration failed', err);
          this.errorMessage = 'Login failed after registration.';
        },
      });
    },
    error: () => {
      this.errorMessage = 'Registration failed. Try a different username.';
    },
  });
  }

}
