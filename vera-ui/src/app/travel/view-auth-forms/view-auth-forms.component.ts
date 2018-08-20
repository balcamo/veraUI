import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../service/app.service.user';
import { User } from '../../classes/user';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthForm } from '../../classes/travel-auth-form';
import { Constants } from '../../classes/constants';

@Component({
  selector: 'app-view-auth-forms',
  templateUrl: './view-auth-forms.component.html',
  styleUrls: ['./view-auth-forms.component.scss']
})
export class ViewAuthFormsComponent implements OnInit {
  //@Input() authForms;
  http: Http;
  userService: UserService
  user: User;
  displayForm = "none";
  displayRecap = "none";
  consts = new Constants();

  form = new AuthForm();
  oldForm: AuthForm;
  // temperary
  authForms = [];
  auth1 = new AuthForm();
  auth2 = new AuthForm();
  constructor(http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
  }

  ngOnInit() {
    this.auth1.EventTitle = "water and power conference";
    this.auth1.Location = "Washington";
    this.auth1.Supervisor = "red";
    this.auth2.EventTitle = "Event 2";
    this.auth2.Location = "Texas";
    this.auth2.Supervisor = "green";

    this.authForms.push(this.auth1);
    this.authForms.push(this.auth2);
  }

  /**
   * 
   * DisplaySelected will show information form the selected authform
   * @param authForm : form to be displayed
   * 
   */
  displaySelected(authForm: AuthForm) {
    this.form = authForm;
    this.displayRecap = "none";
    //console.log(this.form);
    if (this.displayForm == "none") {
      this.displayForm = "block";
    } else if (this.form !== this.oldForm) {
      this.displayForm = "block"
    } else if (this.form == this.oldForm) {
      this.displayForm = "none"
    } 
    this.oldForm = this.form
  }

  /**
   * checkTot will calculate the total spent on the trip
   * and the total the traveler is owed for reimbursement 
   * */
  checkTot() {
    this.form.TotalRecap = 0;
    var mileage = this.form.Mileage * this.consts.mileageRate;
    var foodTravel = this.form.RecapPerDiem * this.consts.firstLastDayFood;
    var foodFull = this.form.RecapPerDiem * this.form.RecapFullDays
    this.form.TotalRecap = this.form.RecapRegistrationCost + this.form.RecapAirfare + this.form.RecapRentalCar +
      this.form.RecapFuelParking + mileage + this.form.RecapLodging + foodTravel + foodFull + this.form.RecapMisc;
    this.form.TotalReimburse = this.form.TotalRecap - this.form.AdvanceAmount;
  }

  /**
   * show recap uses the form from selected to update the recap fields  
   * */
  showRecap() {
    this.displayForm = "none";
    this.displayRecap = "block";
    
    this.form.RecapAirfare = this.form.Airfare;
    this.form.RecapRegistrationCost = this.form.RegistrationCost;
    this.form.RecapRentalCar = this.form.RentalCar;
    this.form.RecapFuelParking = this.form.FuelParking;
    this.form.RecapMileage = this.form.Mileage;
    this.form.RecapLodging = this.form.Lodging;
    this.form.RecapFullDays = this.form.FullDays;
    this.form.RecapPerDiem = this.form.PerDiem;
    this.form.RecapMisc = this.form.Misc;
    this.form.TotalRecap = this.form.TotalEstimate;
    console.log("Form" +this.form);
  }

  /**
   * submit the recap to the server to get approval
   * */
  submitRecap() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify(this.form);
    console.log(this.consts.url + 'Recap');
    this.http.post(this.consts.url + 'Recap', body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }
}
