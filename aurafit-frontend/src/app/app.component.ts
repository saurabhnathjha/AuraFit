import { Component } from '@angular/core';
import { BreakpointObserver, Breakpoints } from '@angular/cdk/layout';
import { MatSidenavModule } from '@angular/material/sidenav';
import { CommonModule } from '@angular/common';
import { Router, NavigationEnd } from '@angular/router';
import { RouterModule } from '@angular/router';
import { FooterComponent } from './shared/components/footer/footer.component';
import { NavbarComponent } from './shared/components/navbar/navbar.component';
import { SidebarComponent } from './shared/components/sidebar/sidebar.component';
import { filter } from 'rxjs';
import { LandingnavbarComponent } from './shared/components/landingnavbar/landingnavbar.component';
import { FaqComponent } from './pages/faq/faq.component';


@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    RouterModule,
    MatSidenavModule,
    NavbarComponent,
    SidebarComponent,
    FooterComponent,
    LandingnavbarComponent,
    FaqComponent

  ],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent {
  hideLayout = false;

  isMobile = false;
  drawerMode: 'side' | 'over' = 'side';
  sidebarOpened = true; // for mobile only

  constructor(private router: Router,private breakpointObserver: BreakpointObserver) {
    this.breakpointObserver.observe([Breakpoints.Handset]).subscribe(result => {
      this.isMobile = result.matches;
      this.drawerMode = this.isMobile ? 'over' : 'side';
      this.sidebarOpened = !result.matches;
    });

    this.router.events
      .pipe(filter(event => event instanceof NavigationEnd))
      .subscribe((event: NavigationEnd) => {
        const hiddenRoutes = [
          '/login',
          '/register',
          '/profile-setup',
          '/goal-setup',
          '/landing',
          '/faq',
          '/contact',
          '/about',
          '/team-form'
        ];

        const cleanUrl = event.urlAfterRedirects.split('?')[0]; // Ignore query params
        this.hideLayout = hiddenRoutes.includes(cleanUrl);

        if (this.drawerMode === 'over') {
          this.sidebarOpened = false;
        }
      });
  }

  onToggleSidebar() {
    if (this.drawerMode === 'over') {
      this.sidebarOpened = !this.sidebarOpened;
    } else {
      // On large screens, instead of closing drawer, toggle collapsed view
      this.isMobile = false;
      this.sidebarOpened = true; // keep it opened
      this.toggleCollapsedView = !this.toggleCollapsedView;
    }
  }

  toggleCollapsedView = false;

}
