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
  @Input() authForms;
  @Input() formsList;
  http: Http;
  userService: UserService
  user: User;
  displayForm = "none";
  displayRecap = "none";
  consts = new Constants();
  registrationComp = true;
  airfareComp = true;
  form = new AuthForm();
  oldForm: AuthForm;
  dhApprove: string;
  gmApprove: string;
  submitted = false;
  advanceLocked = true;
  advanceStatus: string;
  recapStatus: string;

  constructor(http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
    if (this.authForms == undefined) {
      console.log("in constructor: no forms to approve");
      this.formsList = false;
    }
  }

  ngOnInit() {
    console.log("formsList from in all forms html" + this.formsList);

  }

  /**
   * 
   * DisplaySelected will show information form the selected authform
   * @param authForm : form to be displayed
   * 
   */
  displaySelected(authForm: AuthForm) {
    if (authForm.RecapStatus == 0 || authForm.RecapStatus == 3 ) {
      this.submitted = false;
    } else if (authForm.RecapStatus == 1 || authForm.RecapStatus == 2) {
      this.submitted = true;
    }
    if (authForm.AdvanceStatus != 0 ) {
      this.advanceLocked = true;
    }

    this.form = authForm;
    console.log("recap status: " + this.form.RecapStatus);

    this.displayRecap = "none";
    if (this.form.ApprovalStatus == 'green') {
      this.dhApprove = "Approved";
      this.gmApprove = "Approved";
    } else {
      if (this.form.DHApproval.toString() == "yellow") {
        this.dhApprove = "Pending";
      } else if (this.form.DHApproval.toString() == "green") {
        this.dhApprove = "Approved";
      } else {
        this.dhApprove = "Denied";
        this.gmApprove = "Form will not be sent to the GM";
      }

      if (this.form.GMApproval.toString() == "yellow") {
        this.gmApprove = "Pending";
      } else if (this.form.GMApproval.toString() == "green") {
        this.gmApprove = "Approved";
      } else {
        this.gmApprove = "Denied";
      }
    }
    if (this.form.AdvanceStatus == 0) {
      this.advanceStatus = "Denied. Please see email for feedback.";
    } else if (this.form.AdvanceStatus == 1) {
      this.advanceStatus = "Approved";
    } else if (this.form.AdvanceStatus == 2) {
      this.advanceStatus = "Pending";
    }
    if (this.form.RecapStatus == 0) {
      this.recapStatus = "Denied. Please see email for feedback.";
    } else if (this.form.RecapStatus == 1) {
      this.recapStatus = "Approved";
    } else if (this.form.RecapStatus == 2) {
      this.recapStatus = "Pending";
    }else if (this.form.RecapStatus == 3) {
      this.recapStatus = "Not Started";
    }


    if (this.displayForm == "none") {
      this.displayForm = "block";
      this.displayRecap = "none";
    } else if (this.form !== this.oldForm) {
      this.displayForm = "block";
      this.displayRecap = "none";
    } else if (this.form == this.oldForm) {
      this.displayForm = "none";
      this.displayRecap = "none";
    }
    
    this.oldForm = this.form
    
  }
  /**
   * checkTot will calculate the total spent on the trip
   * and the total the traveler is owed for reimbursement 
   * */
  checkTot() {
    if (this.airfareComp) { this.form.RecapAirfare = 0; }
    if (this.registrationComp) { this.form.RecapRegistrationCost = 0; }

    this.form.RecapMileage = this.form.RecapMileageAmount;
    this.form.RecapMileageAmount = this.form.RecapMileage * this.consts.mileageRate;

    if (this.form.RecapMileageAmount > this.form.Mileage) {
      this.form.RecapMileageAmount = this.form.Mileage;
    }

    this.form.TotalRecap = 0;
    this.form.TotalReimburse = 0;
    var foodTravel = this.form.RecapPerDiem * this.consts.travelDayFood * this.form.RecapTravelDays;
    var foodFull = this.form.RecapPerDiem * this.form.RecapFullDays;

    var foodTravel = this.form.RecapPerDiem * this.consts.travelDayFood * this.form.RecapTravelDays;
    var foodFull = this.form.RecapPerDiem * this.form.RecapFullDays;

    this.form.TotalRecap = this.form.RecapRegistrationCost + this.form.RecapAirfare + this.form.RecapRentalCar +
      this.form.RecapFuel + this.form.RecapParkingTolls + this.form.RecapMileageAmount + this.form.RecapLodging +
      foodTravel + foodFull + this.form.RecapMisc;

    this.form.TotalReimburse = this.form.TotalRecap - this.form.AdvanceAmount;
  }

  /**
   * show recap uses the form from selected to update the recap fields  
   * */
  showRecap() {
    this.displayForm = "none";
    this.form.RecapAirfare = (this.form.RecapAirfare.toString() == '' || this.form.RecapAirfare == 0.0000 ? 0 : this.form.RecapAirfare);
    this.form.RecapRegistrationCost = (this.form.RecapRegistrationCost.toString() == '' || this.form.RecapRegistrationCost == 0.0000 ? 0 : this.form.RecapRegistrationCost);
    this.form.RecapRentalCar = (this.form.RecapRentalCar.toString() == '' || this.form.RecapRentalCar == 0.0000 ? 0 : this.form.RecapRentalCar);
    this.form.RecapFuel = (this.form.RecapFuel.toString() == '' || this.form.RecapFuel == 0.0000 ? 0 : this.form.RecapFuel);
    this.form.RecapParkingTolls = (this.form.RecapParkingTolls.toString() == '' || this.form.RecapParkingTolls == 0.0000 ? 0 : this.form.RecapParkingTolls);
    this.form.RecapMileage = (this.form.RecapMileage.toString() == '' || this.form.RecapMileage == 0.0000 ? 0 : this.form.RecapMileage);
    this.form.RecapMileageAmount = (this.form.RecapMileageAmount.toString() == '' || this.form.RecapMileageAmount == 0.0000 ? 0 : this.form.RecapMileageAmount);
    this.form.RecapLodging = (this.form.RecapLodging.toString() == '' || this.form.RecapLodging == 0.0000 ? 0 : this.form.RecapLodging);
    this.form.RecapTravelDays = (this.form.RecapTravelDays.toString() == '' || this.form.RecapTravelDays == 0.0000 ? 0 : this.form.RecapTravelDays);
    this.form.RecapFullDays = (this.form.RecapFullDays.toString() == '' || this.form.RecapFullDays == 0.0000 ? 0 : this.form.RecapFullDays);
    this.form.RecapPerDiem = (this.form.RecapPerDiem.toString() == '' || this.form.RecapPerDiem == 0.0000 ? 0 : this.form.RecapPerDiem);
    this.form.RecapMisc = (this.form.RecapMisc.toString() == '' || this.form.RecapMisc == 0.0000 ? 0 : this.form.RecapMisc);
    this.form.TotalRecap = (this.form.TotalRecap.toString() == '' || this.form.TotalRecap == 0.0000 ? 0 : this.form.TotalRecap);
    this.form.TotalReimburse = (this.form.TotalReimburse.toString() == '' || this.form.TotalReimburse == 0.0000 ? 0 : this.form.TotalReimburse);
 
    this.displayRecap = "block";

  }

  /**
   * submit the recap to the server to get approval
   * */
  submitUpdate() {
    this.form.AdvanceStatus = 2;
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
    this.http.put(this.consts.url + 'TravelAuth?restUserID=' + this.user.UserID, body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }
   /**
   * submit the recap to the server to get approval
   * */
  submitRecap() {
    this.form.RecapStatus = 2;
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
    this.http.post(this.consts.url + 'Recap?restUserID=' + this.user.UserID, body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }

  waitForHttp(data: string) {
    console.log("the data : " + data);
    if (data == "") {
      console.log("Data returned is null");
      this.formsList = false;
    } else {
      this.formsList = true;
      console.log("finishing waitForHttp");
    }
  }
}
