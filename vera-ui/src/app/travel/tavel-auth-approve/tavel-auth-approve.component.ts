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
  submitted = false;

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
   * submit the recap to the server to get approval
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
    if (!this.form.Bool5) {
      this.form.Bool5 = true;
      this.form.Decimal26 = this.user.UserID;
    } else if (this.form.Bool5 && !this.form.Bool6) {
      this.form.Bool6 = true;
      this.form.Decimal27 = this.user.UserID;
    }
    var body = JSON.stringify(this.form);
    console.log(this.form);

    console.log(this.consts.url + 'Recap');
    this.http.post(this.consts.url + 'Recap', body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }
  /**
 * submit the recap to the server to get approval
 * */
  submitDenyStatus() {
    this.submitted = true;
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    if (!this.form.Bool5 && !this.form.Bool6) {
      this.form.Bool5 = false;
      this.form.Decimal26 = this.user.UserID;
    } else if (this.form.Bool5 && !this.form.Bool6) {
      this.form.Bool6 = false;
      this.form.Decimal27 = this.user.UserID;
    }
    console.log(this.form);
      var body = JSON.stringify(this.form);
    console.log(this.consts.url + 'Recap');
    this.http.post(this.consts.url + 'Recap', body, options)
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => alert(data.text()));
  }
}
