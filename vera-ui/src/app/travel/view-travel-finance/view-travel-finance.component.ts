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
  displayForm = "none";
  displayRecap = "none";
  consts = new Constants();
  registrationComp = true;
  airfareComp = true;
  form = new AuthForm();
  oldForm: AuthForm;
  formType: string;

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

    if (this.displayForm == "none") {
      this.displayForm = "block";
    } else if (this.form !== this.oldForm) {
      this.displayForm = "block";
    } else if (this.form == this.oldForm) {
      this.displayForm = "none";
    }
    this.oldForm = this.form

  }
}
