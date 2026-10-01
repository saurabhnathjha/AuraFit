import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-not-found-redirect',
  standalone: true,
  template: '',
})
export class NotFoundRedirectComponent {
  private authService = inject(AuthService);
  private router = inject(Router);

  constructor() {
    const isLoggedIn = this.authService.isLoggedIn();
    this.router.navigate([isLoggedIn ? '/dashboard' : '/landing']);
  }
}
