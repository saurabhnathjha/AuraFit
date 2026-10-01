import { Component, OnInit } from '@angular/core';
import { Router, RouterModule } from '@angular/router';
import { AuthService, LoginRequest } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  standalone: true,
  imports: [
    FormsModule,
    CommonModule,
    RouterModule,
    MatInputModule,
    MatCardModule,
    MatButtonModule
  ],
})
export class LoginComponent implements OnInit {
  username = '';
  password = '';
  errorMessage = '';

  constructor(private authService: AuthService, private router: Router) { }

  ngOnInit(): void {
    // Redirect to dashboard if already logged in
    if (this.authService.isLoggedIn()) {
      this.authService.handlePostLoginRedirect();
    }
  }

  //login(): void {
  //  const dto: LoginRequest = {
  //    username: this.username,
  //    password: this.password
  //  };

  //  this.authService.login(dto).subscribe({
  //    next: (res) => {
  //      this.authService.storeToken(res.token);
  //      this.authService.handlePostLoginRedirect();
  //    },
  //    error: () => {
  //      this.errorMessage = 'Invalid username or password.';
  //    }
  //  });
  //}
  login(): void {
    const dto: LoginRequest = {
      username: this.username,
      password: this.password
    };

    this.authService.login(dto).subscribe({
      next: (res) => {
        // Store the token
        this.authService.storeToken(res.token);

        // ✅ Now fetch the current user and store userId
        this.authService.getCurrentUser().subscribe({
          next: (user) => {
            console.log('✅ Logged-in user ID:', user.id);
            localStorage.setItem('userId', user.id.toString());

            // Continue login flow (e.g., redirect)
            this.authService.handlePostLoginRedirect();
          },
          error: (err) => {
            console.error('❌ Failed to get current user info', err);
          }
        });
      },
      error: () => {
        this.errorMessage = 'Invalid username or password.';
      }
    });
  }
}
