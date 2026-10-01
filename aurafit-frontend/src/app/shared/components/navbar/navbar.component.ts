import { Component, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { MatTooltipModule } from '@angular/material/tooltip';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { ThemeService } from '../../../services/theme.service';


@Component({
  selector: 'app-navbar',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatToolbarModule,
    MatButtonModule,
    MatTooltipModule
  ],
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent {
  @Output() toggleSidebar = new EventEmitter<void>();
  isDarkMode = false;

  constructor(private themeService: ThemeService) { }

  toggleTheme(): void {
    const html = document.documentElement;
    const isDark = html.classList.contains('dark-theme');
    const newTheme = !isDark;
    html.classList.toggle('dark-theme', newTheme);
    this.isDarkMode = newTheme;
    this.themeService.setDarkMode(newTheme);
  }

}
