import { Component, OnInit } from '@angular/core';
import { AuthForm } from '../../classes/travel-auth-form';
import { Constants } from '../../classes/constants';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
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
  daysTravelBorder = "black";
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
    this.form.TotalEstimate = 0;
    var foodTravel = this.form.PerDiem * this.consts.travelDayFood * this.form.TravelDays;
    var foodFull = this.form.PerDiem * this.form.FullDays;
    this.form.TotalEstimate = this.form.RegistrationCost + this.form.Airfare + this.form.RentalCar +
      this.form.Fuel + this.form.ParkingTolls + this.form.Mileage + this.form.Lodging +
      foodTravel + foodFull + this.form.Misc;
  }

  // function to contact API and submit the form
  submitForm() {
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
      this.form.SubmitterSig = this.user.UserID;
      this.form.Email = (this.form.Email == null ? this.user.UserEmail : this.form.Email);
      if (this.user.EntryGroup[3] == 1) {
        this.form.DHApproval = 'green';
        this.form.DHID = this.user.UserID;
      }
      if (this.user.EntryGroup[3] == 99) {
        this.form.GMApproval = 'green';
        this.form.GMID = this.user.UserID;
      }
      var body = JSON.stringify(this.form);
      console.log('post' + this.consts.url + 'TravelAuth');
      this.http.post(this.consts.url + 'TravelAuth?restUserID=' + this.user.UserID, body, options)
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
    // make sure all needed fields are filled in
    if (this.form.FirstName == null || this.form.Phone == null || this.form.EventTitle == null
      || this.form.Location == null || this.form.TravelBegin == null || this.form.FullDays == null
      || this.form.Mileage == null || this.form.Lodging == null || this.form.PerDiem == null
      || this.form.RentalCar == null || this.form.Fuel == null || this.form.Misc == null
      || this.form.TravelEnd == null || this.form.DistVehicle == null || this.form.Airfare == null
      || this.form.RegistrationCost == null || this.form.Advance == null || this.form.DistVehicle == null
      || this.form.Policy == null || this.form.LastName == null || this.form.ParkingTolls == null
      || this.form.TravelDays == null
      ) {
      valid = false;
    }
    // change border colors to show which fields need to be filled in
    this.firstNameBorder = (this.form.FirstName == null ? "red" : "black");
    this.lastNameBorder = (this.form.LastName == null ? "red" : "black");
    this.phoneBorder = (this.form.Phone == null ? "red" : "black");
    this.eventTitleBorder = (this.form.EventTitle == null ? "red" : "black");
    this.eventLocBorder = (this.form.Location == null ? "red" : "black");
    this.travelStartBorder = (this.form.TravelBegin == null ? "red" : "black");
    this.travelEndBorder = (this.form.TravelEnd == null ? "red" : "black");
    this.distVehColor = (this.form.DistVehicle == null ? "red" : "black");
    this.registrationBorder = (this.form.RegistrationCost == null ? "red" : "black");
    this.airfareBorder = (this.form.Airfare == null ? "red" : "black");
    this.rentalBorder = (this.form.RentalCar == null ? "red" : "black");
    this.fuelBorder = (this.form.Fuel == null ? "red" : "black");
    this.parkingBorder = (this.form.ParkingTolls == null ? "red" : "black");
    this.mileageBorder = (this.form.Mileage == null ? "red" : "black");
    this.lodgingBorder = (this.form.Lodging == null ? "red" : "black");
    this.perDiemBorder = (this.form.PerDiem == null ? "red" : "black");
    this.daysBorder = (this.form.FullDays == null ? "red" : "black");
    this.miscBorder = (this.form.Misc == null ? "red" : "black");
    this.advanceColor = (this.form.Advance == null ? "red" : "black");
    this.policyColor = (this.form.Policy == null ? "red" : "black");
    this.daysTravelBorder = (this.form.TravelDays == null ? "red" : "black");
    // ensure traveler is aware of travel policy
    if (this.form.Policy == false) {
      valid = false;
      alert("Please read the district policy on travel.");
    }
    return valid;
  }

  /**
   * This function will reset the forms values
   **/
  setFormDefaults() {
    this.form.FirstName = null;
    this.form.LastName = null;
    this.form.Phone = null;
    this.form.Email = null;
    this.form.EventTitle = null;
    this.form.Location = null;
    this.form.TravelBegin = null;
    this.form.TravelEnd = null;
    this.form.DistVehicle = null;
    this.form.RegistrationCost = 0;
    this.form.Airfare = 0;
    this.form.RentalCar = 0;
    this.form.Fuel = 0;
    this.form.ParkingTolls = 0;
    this.form.Mileage = 0;
    this.form.Lodging = 0;
    this.form.PerDiem = 0;
    this.form.FullDays = 0;
    this.form.TravelDays = 0;
    this.form.Misc = 0;
    this.form.MiscExplain = null;
    this.form.TotalEstimate = 0;
    this.form.Advance = null;
    this.form.AdvanceAmount = 0;
    this.form.Policy = null;
    this.form.Preparer = false;
    this.form.SubmitterSig = null;
    this.form.DHApproval = null;
    this.form.GMApproval = null;
  }
}
