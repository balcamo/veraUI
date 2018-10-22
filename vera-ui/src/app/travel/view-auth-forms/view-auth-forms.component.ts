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
    this.form = authForm;
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
    if (this.displayForm == "none") {
      this.displayForm = "block";
    } else if (this.form !== this.oldForm) {
      this.displayForm = "block";
    } else if (this.form == this.oldForm) {
      this.displayForm = "none";
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
    this.form.TotalRecap = 0;
    this.form.RecapMileageAmount = this.form.RecapMileage * this.consts.mileageRate;
    if (this.form.RecapMileageAmount > this.form.Mileage) { this.form.RecapMileageAmount = this.form.Mileage; }
    var foodTravel = this.form.RecapPerDiem * this.consts.travelDayFood * this.form.RecapTravelDays;
    var foodFull = this.form.RecapPerDiem * this.form.RecapFullDays
    this.form.TotalRecap = this.form.RecapRegistrationCost + this.form.RecapAirfare + this.form.RecapRentalCar + foodFull +
      this.form.RecapFuel + this.form.RecapParkingTolls + this.form.RecapMileageAmount + this.form.RecapLodging + foodTravel + this.form.RecapMisc;
    this.form.TotalReimburse = this.form.TotalRecap - this.form.AdvanceAmount;
  }

  /**
   * show recap uses the form from selected to update the recap fields  
   * */
  showRecap() {
    this.displayForm = "none";
    this.form.RecapAirfare = 0;
    this.form.RecapRegistrationCost = 0;
    this.form.RecapRentalCar = 0;
    this.form.RecapFuel = 0;
    this.form.RecapParkingTolls = 0;
    this.form.RecapMileage = 0;
    this.form.RecapMileageAmount = 0;
    this.form.RecapLodging = 0;
    this.form.RecapTravelDays = 0;
    this.form.RecapFullDays = 0;
    this.form.RecapPerDiem = 0;
    this.form.RecapMisc = 0;
    this.form.TotalRecap = 0;
    this.form.TotalReimburse = 0;

    console.log("Form" + this.form);
    this.displayRecap = "block";

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
    this.http.post(this.consts.url + 'Recap?restUserID=', body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }
}
