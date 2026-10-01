import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { CreateWorkoutPlanComponent } from './pages/pages/create-plan/create-workout-plan.component';
import { TodayWorkoutPlanComponent } from './pages/pages/today-plan/today-workout-plan.component';
import { WorkoutPageComponent } from './pages/pages/workout-page/workout-page.component'
const routes: Routes = [
  //{ path: 'yourworkout', component: WorkoutPageComponent },  // Changed from '' to 'yourworkout'
  //{ path: 'create', component: CreateWorkoutPlanComponent },
  //{ path: 'today', component: TodayWorkoutPlanComponent },
  //{ path: '', redirectTo: 'create', pathMatch: 'full' }
  { path: '', component: WorkoutPageComponent }, // this means: '/yourworkout'
  { path: 'create', component: CreateWorkoutPlanComponent }, // '/yourworkout/create'
  { path: 'today', component: TodayWorkoutPlanComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  //imports: [RouterModule.forRoot(routes)],

  exports: [RouterModule]
})
export class WorkoutRoutingModule { }
