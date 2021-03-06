import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../service/app.service.user';
import { User } from '../../classes/user';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthForm } from '../../classes/travel-auth-form';
import { Constants } from '../../classes/constants';

@Component({
  selector: 'app-tavel-auth-approve',
  templateUrl: './tavel-auth-approve.component.html',
  styleUrls: ['./tavel-auth-approve.component.scss']
})
export class TavelAuthApproveComponent implements OnInit {

  @Input() authForms;
  @Input() formsList;
  http: Http;
  userService: UserService
  user: User;
  displayForm = "none";
  consts = new Constants();
  registrationComp = true;
  airfareComp = true;
  form = new AuthForm();
  oldForm: AuthForm;
  submitted = false;
  transType: string;
  food: number;
  mileage: number;

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
    console.log("formsList from in approve html" + this.formsList);
  }

  /**
   * 
   * DisplaySelected will show information form the selected authform
   * @param authForm : form to be displayed
   * 
   */
  displaySelected(authForm: AuthForm) {
    this.form = authForm;
    this.food = (this.form.PerDiem * this.form.FullDays) +
      (this.form.PerDiem * this.consts.travelDayFood * this.form.TravelDays);
    this.mileage = this.form.Mileage * this.consts.mileageRate;
    if ((this.user.EntryGroup[3] == 1 && this.form.DHApproval.toString() != 'yellow') ||
      (this.user.EntryGroup[3] == 99 && this.form.GMApproval.toString() != 'yellow')) {
      this.submitted = true;
    } else {
      this.submitted = false;
    }

    if (this.form.DistVehicle == true) {
      this.transType = "District Vehicle";
    } else if (this.form.Airfare != 0) {
      this.transType = "Plane";
    } else {
      this.transType = "Personal Vehicle"
    }

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
   * submit the updated form to reflect approval
   * */
  submitApproveStatus() {
    this.submitted = true;
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    this.displaySelected(this.form);
    if (this.user.EntryGroup[3] == 1) {
      this.form.DHApproval = 'green';
    } else if (this.user.EntryGroup[3] == 99) {
      this.form.GMApproval = 'green';
    }
    var body = JSON.stringify(this.form);
    console.log("the form being approved: " + this.form.toString());
    console.log('put.'+this.consts.url + 'TravelApproval?restUserID=' + this.user.UserID);
    this.http.put(this.consts.url + 'TravelApproval?restUserID=' + this.user.UserID, body, options)
      .subscribe((data) => alert("You have APPROVED this travel request"));
    
  }
  /**
 * submit the updated form to reflect denied
 * */
  submitDenyStatus() {
    this.submitted = true;
    console.log(this.submitted);
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    this.displaySelected(this.form);
    if (this.user.EntryGroup[3] == 1) {
      this.form.DHApproval = 'red';
      this.form.DHID = this.user.UserID;
      this.form.ApprovalStatus = 'red'
    } else if (this.user.EntryGroup[3] == 99) {
      this.form.GMApproval = 'red';
      this.form.GMID = this.user.UserID;
      this.form.ApprovalStatus = 'red'
    }
    var body = JSON.stringify(this.form);
    console.log('put.'+this.consts.url + 'TravelApproval');
    this.http.put(this.consts.url + 'TravelApproval?restUserID=' + this.user.UserID, body, options)
      .subscribe((data) => alert("You have DENIED this travel request"));

  }
}
