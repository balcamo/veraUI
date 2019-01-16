import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AppComponent } from "./app.component";
import { TravelComponent } from './travel/travel.component';
import { MeterReadsComponent } from './meter-reads/meter-reads.component';
import { HomeComponent } from './home/home.component';
import { TravelAuthComponent } from './travel/travel-auth/travel-auth.component';
import { ViewAuthFormsComponent } from './travel/view-auth-forms/view-auth-forms.component';
import { TavelAuthApproveComponent } from './travel/tavel-auth-approve/tavel-auth-approve.component';
import { NavComponent } from './nav/nav.component';

const routes: Routes = [
  {
    path: 'travel',
    component: TravelComponent,
    children: [
      { path: 'authform', component: TravelAuthComponent },
      { path: 'activeAuth', component: ViewAuthFormsComponent },
      { path: 'financeAuth', component: ViewAuthFormsComponent },
      { path: 'approveAuth', component: TavelAuthApproveComponent }
    ] 
  },
  {
    path: 'meter-reads',
    component: MeterReadsComponent
  },
  {
    path: 'nav',
    component: NavComponent
  },
  {
    path: '',
    component: HomeComponent
  },
  /*{
    path: '404',
    component: HomeComponent,
    // pathMatch:'full'
  },*/
  {
    path: '**',
    redirectTo: '/',
    //pathMatch:'full'
  }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
