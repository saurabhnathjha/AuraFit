import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { DashboardSummary, DashboardService } from '../../services/dashboard.service';
import { ProfileService } from '../../services/profile.service';
import { ThemeService } from '../../services/theme.service';
import { SafeUrlPipe } from '../../services/safe-url.pipe';
import { Router } from '@angular/router';

import { MatCardModule } from '@angular/material/card';
import { MatProgressBarModule } from '@angular/material/progress-bar';
import { MatDividerModule } from '@angular/material/divider';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDialog, MatDialogModule } from '@angular/material/dialog';
import { DialogComponent } from '../../shared/components/dialog/dialog.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    HttpClientModule,
    MatCardModule,
    MatProgressBarModule,
    MatDividerModule,
    MatProgressSpinnerModule,
    MatDialogModule,
    SafeUrlPipe
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css',
})
export class DashboardComponent implements OnInit {
  summary!: DashboardSummary;
  Math = Math;
  isDarkMode = false;

  constructor(
    private dashboardService: DashboardService,
    private themeService: ThemeService,
    private profileService: ProfileService,
    private router: Router,
    private dialog: MatDialog
  ) { }

  ngOnInit(): void {
    this.dashboardService.getDashboardSummary().subscribe({
      next: data => this.summary = data,
      error: err => console.error('Failed to load dashboard summary:', err)
    });

    this.themeService.isDarkMode$.subscribe(isDark => {
      this.isDarkMode = isDark;
    });

    this.checkShouldPromptWeightUpdate();
  }

  private checkShouldPromptWeightUpdate(): void {
    this.profileService.getShouldUpdateWeight().subscribe({
      next: res => {
        if (res.shouldUpdate) {
          const dialogRef = this.dialog.open(DialogComponent, {
            width: '400px',
            data: {
              title: 'Update Needed',
              message: 'It’s been a week! Please update your weight to stay on track.'
            }
          });

          dialogRef.afterClosed().subscribe(result => {
            if (result === true) {
              this.router.navigate(['/user']);
              console.log('User chose to update weight.');
            } else {
              console.log('User canceled weight update.');
            }
          });
        }
      },
      error: err => {
        console.error('Failed to check weight update status:', err);
      }
    });
  }

  get spotifySrc(): string {
    const base = 'https://open.spotify.com/embed/playlist/5VNFfxcNDLVE5RqFTHN11Y?utm_source=generator';
    return this.isDarkMode ? `${base}&theme=0` : base;
  }

  caloriesLeft(): number {
    if (!this.summary) return 0;
    const net = this.summary.caloriesConsumed - this.summary.caloriesBurned;
    return Math.max(this.summary.targetCalories - net, 0);
  }
}
