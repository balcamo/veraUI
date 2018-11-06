import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../service/app.service.user';
import { User } from '../../classes/user';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthForm } from '../../classes/travel-auth-form';
import { Constants } from '../../classes/constants';

@Component({
  selector: 'app-view-travel-finance',
  templateUrl: './view-travel-finance.component.html',
  styleUrls: ['./view-travel-finance.component.scss']
})
export class ViewTravelFinanceComponent implements OnInit {
  @Input() authForms;
  @Input() formsList;
  http: Http;
  userService: UserService
  user: User;
  displayAdvance = "none";
  displayRecap = "none";
  consts = new Constants();
  registrationComp = true;
  airfareComp = true;
  form = new AuthForm();
  oldForm: AuthForm;
  formType: string;
  food: number;
  denyExplain: string;

  constructor(http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
    if (this.authForms == undefined) {
      console.log("in constructor: no forms to approve");
      this.formsList = false;
    }
  }

  ngOnInit() {  }

  /**
   * 
   * DisplaySelected will show information form the selected authform
   * @param authForm : form to be displayed
   * 
   */
  displaySelected(authForm: AuthForm) {
    this.form = authForm;
    this.form.MailMessage = '';
    // this if makes sure that old views are removed
    if (this.form == this.oldForm) {
      this.displayRecap = "none";
      this.displayAdvance = "none"
    } else {
      // select display for recap or advance
      if (this.form.AdvanceStatus == 1 && this.form.RecapStatus == 2) {
        this.food = (this.form.RecapPerDiem * this.consts.travelDayFood * this.form.RecapTravelDays)
          + (this.form.RecapPerDiem * this.form.RecapFullDays);
        this.displayAdvance = "none"
        if (this.displayRecap == "none") {
          this.displayRecap = "block";
        } else if (this.form !== this.oldForm) {
          this.displayRecap = "block";
        } else if (this.form == this.oldForm) {
          this.displayRecap = "none";
        }
      } else {
        this.food = (this.form.PerDiem * this.consts.travelDayFood * this.form.TravelDays)
          + (this.form.PerDiem * this.form.FullDays);
        this.displayRecap = "none"
        if (this.displayAdvance == "none") {
          this.displayAdvance = "block";
        } else if (this.form !== this.oldForm) {
          this.displayAdvance = "block";
        } else if (this.form == this.oldForm) {
          this.displayAdvance = "none";
        }
      }
    }
    this.oldForm = this.form

  }

  approveAdvance() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify(this.form);
    console.log('approve addvance put.'+this.consts.url + 'TravelFinance');
    this.displaySelected(this.form);
    this.http.put(this.consts.url + 'TravelFinance?restUserID=' + this.user.UserID
      + '&restButtonID=0', body, options)
      .subscribe((data) => alert("Advance form is being processed"));
  }
  denyAdvance() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify(this.form);
    console.log('deny addvance put.' +this.consts.url + 'TravelFinance');
    this.displaySelected(this.form);
    this.http.put(this.consts.url + 'TravelFinance?restUserID=' + this.user.UserID
      + '&restButtonID=2', body, options)
      .subscribe((data) => alert("Advance form is being returned to the traveler"));
  }

  approveRecap() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify(this.form);
    console.log('approve recap put.' +this.consts.url + 'TravelFinance');
    this.displaySelected(this.form);
    this.http.put(this.consts.url + 'TravelFinance?restUserID=' + this.user.UserID
      + '&restButtonID=1', body, options)
      .subscribe((data) => alert("Recap form is being processed"));
  }
  denyRecap() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify(this.form);
    console.log('deny recap put.' +this.consts.url + 'TravelFinance');
    this.displaySelected(this.form);
    this.http.put(this.consts.url + 'TravelFinance?restUserID=' + this.user.UserID
      + '&restButtonID=3', body, options)
      .subscribe((data) => alert("Recap form is being returned to the traveler"));
  }
}
