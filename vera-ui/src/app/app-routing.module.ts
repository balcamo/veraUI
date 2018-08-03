import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from "./app.component";
import { TravelComponent } from './travel/travel.component';
import { HomeComponent } from './home/home.component';
import { TravelAuthComponent } from './travel/travel-auth/travel-auth.component';
import { NavComponent } from './nav/nav.component';

const routes: Routes = [
  {
    path: 'travel',
    component: TravelComponent,
  },
  {
    path: 'nav',
    component: NavComponent
  },
  {
    path: '',
    component: HomeComponent
  },
  {
    path: '404',
    component: HomeComponent,
    // pathMatch:'full'
  },
  {
    path: '**',
    component: HomeComponent,
    pathMatch:'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
