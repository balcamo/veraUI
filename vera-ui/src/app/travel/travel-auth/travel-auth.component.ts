import { Component, OnInit } from '@angular/core';
import { AuthForm } from '../../classes/travel-auth-form';
import { Constants } from '../../classes/constants';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { Observable } from 'rxjs';
import { User } from '../../classes/user';
import { UserService } from '../../service/app.service.user';


@Component({
  selector: 'app-travel-auth',
  templateUrl: './travel-auth.component.html',
  styleUrls: ['./travel-auth.component.scss']
})
export class TravelAuthComponent implements OnInit {
  /** variables needed to change colors and fill in the form **/
  form = new AuthForm();
  consts = new Constants();
  total=0;
  http: Http;
  user: User;
  // variables for border colors of required fields
  firstNameBorder = "black";
  lastNameBorder = "black";
  phoneBorder = "black";
  eventTitleBorder = "black";
  eventLocBorder = "black";
  travelStartBorder = "blcak";
  travelEndBorder = "blcak";
  registrationBorder = "black";
  airfareBorder = "black";
  rentalBorder = "black";
  fuelBorder = "black";
  parkingBorder = "black";
  mileageBorder = "black";
  lodgingBorder = "black";
  perDiemBorder = "black";
  daysBorder = "black";
  miscBorder = "black";
  advanceColor = "black";
  distVehColor = "black";
  policyColor = "black";
  // variables to display sections
  distVehDisplay = "none";
  rentalCarDisplay = "block";
  advaneDisplay = "none";
  
  // set up HTTP var
  constructor(http: Http, userService: UserService) {
    this.http = http;
    this.user = userService.getUser();
    this.setFormDefaults();
  }

  ngOnInit() {
  }


  // calculate the estimated total 
  checkTot() {
    this.total = 0;
    var mileage = this.form.Decimal7 * this.consts.mileageRate;
    var foodTravel = this.form.Decimal9 * this.consts.firstLastDayFood;
    var foodFull = this.form.Decimal9 * this.form.Decimal10;
    this.total = this.form.Decimal2 + this.form.Decimal3 + this.form.Decimal4 +
      this.form.Decimal5 + this.form.Decimal6 + mileage + this.form.Decimal8 +
      foodTravel + foodFull + this.form.Decimal11;
    this.form.Decimal12 = this.total;
  }

  // function to contact API and submit the form
  submitForm() {
    console.log("submitting form");
    console.log(this.form);
    console.log("user email is" + this.user.UserEmail);
    // this will check if all required fields have been 
    //   filled in before submitting the form to the API
    if (this.checkRequired()) {
      let params: URLSearchParams = new URLSearchParams();
      var pageHeaders = new Headers({
        'Content-Type': 'application/json'
         });
      let options = new RequestOptions({
        search: params,
        headers: pageHeaders
      });
      this.form.String7 = this.user.UserEmail;
      console.log(this.form);
      var body = JSON.stringify(this.form);
      console.log(this.consts.url +'TravelAuth');
      this.http.post(this.consts.url + 'TravelAuth', body, options)
          .subscribe((data) => alert(data.text()));
      this.setFormDefaults();
    } else {
      alert("Please fill in the required fields");
    }
    
  }

  /**
   * this function will check if all required fields are
   * filled in and toggle the border colors of the required
   * fields
   **/
  checkRequired() {
    var valid = true;

    if (this.form.String1 == null || this.form.decimal1 == null || this.form.String4 == null
      || this.form.String5 == null || this.form.Date1 == null || this.form.Decimal10 == null
      || this.form.Decimal7 == null || this.form.Decimal8 == null || this.form.Decimal9 == null
      || this.form.Decimal4 == null || this.form.Decimal5 == null || this.form.Decimal11 == null
      || this.form.Date2 == null || this.form.Bool2 == null || this.form.Decimal3 == null
      || this.form.Decimal2 == null || this.form.Bool3 == null || this.form.Bool2 == null
      || this.form.Bool4 == null || this.form.String2 == null || this.form.Decimal6 == null
      ) {
      valid = false;
    }

    this.firstNameBorder = (this.form.String1 == null ? "red" : "black");
    this.lastNameBorder = (this.form.String2 == null ? "red" : "black");
    this.phoneBorder = (this.form.decimal1 == null ? "red" : "black");
    this.eventTitleBorder = (this.form.String4 == null ? "red" : "black");
    this.eventLocBorder = (this.form.String5 == null ? "red" : "black");
    this.travelStartBorder = (this.form.Date1 == null ? "red" : "black");
    this.travelEndBorder = (this.form.Date2 == null ? "red" : "black");
    this.distVehColor = (this.form.Bool2 == null ? "red" : "black");
    this.registrationBorder = (this.form.Decimal2 == null ? "red" : "black");
    this.airfareBorder = (this.form.Decimal3 == null ? "red" : "black");
    this.rentalBorder = (this.form.Decimal4 == null ? "red" : "black");
    this.fuelBorder = (this.form.Decimal5 == null ? "red" : "black");
    this.parkingBorder = (this.form.Decimal6 == null ? "red" : "black");
    this.mileageBorder = (this.form.Decimal7 == null ? "red" : "black");
    this.lodgingBorder = (this.form.Decimal8 == null ? "red" : "black");
    this.perDiemBorder = (this.form.Decimal9 == null ? "red" : "black");
    this.daysBorder = (this.form.Decimal10 == null ? "red" : "black");
    this.miscBorder = (this.form.Decimal11 == null ? "red" : "black");
    this.advanceColor = (this.form.Bool3 == null ? "red" : "black");
    this.policyColor = (this.form.Bool4 == null ? "red" : "black");

    if (this.form.Bool4 == false) {
      valid = false;
      alert("Please read the district policy on travel.");
    }
    return valid;
  }

  /**
   * This function will reset the forms values
   **/
  setFormDefaults() {
    this.form.String1 = null;
    this.form.String2 = null;
    this.form.decimal1 = null;
    this.form.String3 = null;
    this.form.String4 = null;
    this.form.String5 = null;
    this.form.Date1 = null;
    this.form.Date2 = null;
    this.form.Bool2 = null;
    this.form.Decimal2 = 0;
    this.form.Decimal3 = 0;
    this.form.Decimal4 = 0;
    this.form.Decimal5 = 0;
    this.form.Decimal6 = 0;
    this.form.Decimal7 = 0;
    this.form.Decimal8 = 0;
    this.form.Decimal9 = 0;
    this.form.Decimal10 = 0;
    this.form.Decimal11 = 0;
    this.form.String6 = null;
    this.form.Decimal12 = null;
    this.form.Bool3 = null;
    this.form.Decimal13 = 0;
    this.form.Bool4 = null;
    this.form.Bool1 = false;
    this.form.String7 = null;
    this.form.String8 = "red";
    this.total = 0; 
  }
}
