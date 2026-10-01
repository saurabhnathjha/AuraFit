import { Routes } from '@angular/router';
import { LoginComponent } from './pages/auth/login/login.component';
import { DashboardComponent } from './pages/dashboard/dashboard.component';
import { AuthGuard } from './pages/auth/guards/auth.guard';
import { RegisterComponent } from './pages/auth/register/register.component';
import { UserComponent } from './pages/user/user.component';
import { FoodLogComponent } from './pages/food-log/food-log.component';
import { LogWorkoutComponent } from './pages/log-workout/log-workout.component';
import { AuraChatComponent } from './pages/aura-chat/aura-chat.component';
import { LandingPageComponent } from './pages/landing-page/landing-page.component'
import { FaqComponent } from './pages/faq/faq.component';
import { ContactComponent } from './shared/components/contact/contact.component';
import { TeamFormComponent } from './shared/components/team-form/team-form.component';
import { AboutComponent } from './shared/components/about/about.component';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'landing',
    pathMatch: 'full'
  },
  {
    path: 'yourworkout',
    loadChildren: () => import('./workout/workout.module').then(m => m.WorkoutModule)
  },
  {
    path: 'landing',
    component: LandingPageComponent,
  },
  {
    path: 'login',
    component: LoginComponent
  },
  {
    path: 'register',
    component: RegisterComponent,
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./pages/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'profile-setup',
    loadComponent: () => import('./pages/profile-setup/profile-setup.component').then(m => m.ProfileSetupComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'goal-setup',
    loadComponent: () => import('./pages/goal-setup/goal-setup.component').then(m => m.GoalSetupComponent),
    canActivate: [AuthGuard]
  },
  {
    path: 'user',
    component: UserComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'food-log',
    component: FoodLogComponent,
    canActivate: [AuthGuard]
  },
  {
    path: 'log-workout',
    component: LogWorkoutComponent,
    canActivate: [AuthGuard]  
  },

  {
    path: 'ask',
    component: AuraChatComponent,
    canActivate: [AuthGuard]  
  },

  {
    path: 'faq',
    component:FaqComponent
  },
  {
    path: 'contact',
    component:ContactComponent
  },
  {
    path: 'team-form',
    component: TeamFormComponent
  },
  {
    path: 'about',
    component: AboutComponent
  },
  {
    path: '**',
    loadComponent: () =>
      import('./pages/not-found-redirect/not-found-redirect.component').then(m => m.NotFoundRedirectComponent)
  }


  


];
