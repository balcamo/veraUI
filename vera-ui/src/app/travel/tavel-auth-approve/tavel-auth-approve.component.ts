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
  displayRecap = "none";
  consts = new Constants();
  registrationComp = true;
  airfareComp = true;
  form = new AuthForm();
  oldForm: AuthForm;

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
    if (this.airfareComp) { this.form.Decimal15 = 0; }
    if (this.registrationComp) { this.form.Decimal14 = 0; }
    this.form.Decimal24 = 0;
    var mileage = this.form.Decimal19 * this.consts.mileageRate;
    var foodTravel = this.form.Decimal21 * this.consts.firstLastDayFood;
    var foodFull = this.form.Decimal21 * this.form.Decimal22
    this.form.Decimal24 = this.form.Decimal14 + this.form.Decimal15 + this.form.Decimal16 + foodFull +
      this.form.Decimal17 + this.form.Decimal18 + mileage + this.form.Decimal20 + foodTravel + this.form.Decimal23;
    this.form.Decimal25 = this.form.Decimal24 - this.form.Decimal13;
  }

  /**
   * show recap uses the form from selected to update the recap fields  
   * */
  showRecap() {
    this.displayForm = "none";
    this.form.Decimal15 = 0;
    this.form.Decimal14 = 0;
    this.form.Decimal16 = 0;
    this.form.Decimal17 = 0;
    this.form.Decimal18 = 0;
    this.form.Decimal19 = 0;
    this.form.Decimal20 = 0;
    this.form.Decimal22 = 0;
    this.form.Decimal21 = 0;
    this.form.Decimal23 = 0;
    this.form.Decimal24 = 0;
    this.form.Decimal25 = 0;

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
    this.http.post(this.consts.url + 'Recap', body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }

}
