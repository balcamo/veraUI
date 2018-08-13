import { Component, OnInit, Input } from '@angular/core';
import { UserService } from '../../service/app.service.user';
import { User } from '../../classes/user';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { AuthForm } from '../../classes/travel-auth-form';

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
  //temp
  authForms = [];
  form: AuthForm;
  oldForm: AuthForm;
  auth1 = new AuthForm();
  auth2 = new AuthForm();
  constructor(http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;
    this.user = this.userService.getUser();
  }

  ngOnInit() {
    this.auth1.EventTitle = "Event 1";
    this.auth1.Supervisor = "red";
    this.auth2.EventTitle = "Event 2";
    this.auth2.Supervisor = "green";

    this.authForms.push(this.auth1);
    this.authForms.push(this.auth2);
  }
  displaySelected(authForm: AuthForm) {
    this.form = authForm;

    console.log(this.form);
    if (this.displayForm == "none") {
      this.displayForm = "block";
    } else if (this.form !== this.oldForm) {
      this.displayForm = "block"
    } else if (this.form == this.oldForm) {
      this.displayForm = "none"
    } 
    this.oldForm = this.form
  }

}
