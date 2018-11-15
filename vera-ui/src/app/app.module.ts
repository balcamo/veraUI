import { BrowserModule } from '@angular/platform-browser';
import { NgModule, NO_ERRORS_SCHEMA } from '@angular/core';
import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { Router, ActivatedRoute } from '@angular/router';
import { Serialize, SerializeProperty, Serializable } from 'ts-serializer';

import { platformBrowserDynamic } from '@angular/platform-browser-dynamic';
import { Http, HttpModule } from '@angular/http';
//import { AdalService, Adal4HTTPService } from 'adal-angular4';
import { AppRoutingModule } from './app-routing.module';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { TravelComponent } from './travel/travel.component';
import { HomeComponent } from './home/home.component';
import { TravelAuthComponent } from './travel/travel-auth/travel-auth.component';
import { ViewAuthFormsComponent } from './travel/view-auth-forms/view-auth-forms.component';
import { UserService } from './service/app.service.user';
import { TavelAuthApproveComponent } from './travel/tavel-auth-approve/tavel-auth-approve.component';
import { ViewTravelFinanceComponent } from './travel/view-travel-finance/view-travel-finance.component';

@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    TravelComponent,
    HomeComponent,
    TravelAuthComponent,
    ViewAuthFormsComponent,
    TavelAuthApproveComponent,
    ViewTravelFinanceComponent,
    
  ],
  imports: [
    BrowserModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    HttpModule,
    AppRoutingModule,
    MDBBootstrapModule.forRoot(),
    NgbModule.forRoot(),

  ],
  exports: [
    HttpModule,
    HttpClientModule,

  ],
  providers: [UserService,
    /*AdalService,
    {
      provide: Adal4HTTPService,
      useFactory: Adal4HTTPService.factory,
      deps: [Http, AdalService]
    }*/
  ],
  bootstrap: [AppComponent],
  schemas: [NO_ERRORS_SCHEMA]
})
export class AppModule {
 
}
