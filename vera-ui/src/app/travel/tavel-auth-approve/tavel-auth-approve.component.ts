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
  }

  ngOnInit() {
  }

  /**
   * 
   * DisplaySelected will show information form the selected authform
   * @param authForm : form to be displayed
   * 
   */
  displaySelected(authForm: AuthForm) {
    this.form = authForm;
    this.food = (this.form.PerDiem * this.form.FullDays) + (this.form.PerDiem * this.consts.firstLastDayFood);
    this.mileage = this.form.Mileage * this.consts.mileageRate;
    if ((this.user.EntryGroup[3] == 99 && this.form.DHApproval.toString() != '') ||
      (this.user.EntryGroup[3] == 3 && this.form.GMApproval.toString() != '')) {
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
    if (this.user.EntryGroup[3] == 99) {
      this.form.DHApproval = true;
      this.form.DHID = this.user.UserID;
    } else if (this.user.EntryGroup[3] == 2) {
      this.form.GMApproval = true;
      this.form.GMID = this.user.UserID;
    }
    var body = JSON.stringify({ userID: this.user.UserID, value: this.form });
    console.log(this.consts.url + 'Recap');
    this.http.post(this.consts.url + 'Recap', body, options)
      .subscribe((data) => alert(data.text()));
    this.form.ApprovalStatus = 'green'
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
    if (this.user.EntryGroup[3] == 99) {
      this.form.DHApproval = false;
      this.form.DHID = this.user.UserID;
    } else if (this.user.EntryGroup[3] == 2) {
      this.form.GMApproval = false;
      this.form.GMID = this.user.UserID;
    }
    var body = JSON.stringify(this.form);
    console.log(this.consts.url + 'Recap');
    this.http.post(this.consts.url + 'Recap?restUserID=' + this.user.UserID, body, options)
      .subscribe((data) => alert(data.text()));
    this.form.ApprovalStatus = 'red'
  }
}
