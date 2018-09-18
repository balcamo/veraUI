import { Component, EventEmitter } from '@angular/core';
import { Router } from '@angular/router';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { User, Auth } from './classes/user';
import { UserService } from './service/app.service.user';
import { Observable } from 'rxjs';
import { Constants } from './classes/constants';
import { NavComponent } from './nav/nav.component';
import { Serialize, SerializeProperty, Serializable } from 'ts-serializer';


@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  outputs: ['notify'],
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  userEntry = "block";
  mainPage = "none";
  userService: UserService;
  user = new User();;
  consts = new Constants();
  http: Http;
  notify: EventEmitter<string> = new EventEmitter<string>();
  nav: NavComponent;
  useremail: string;
  value = new Auth()
  password: string;

  constructor(private router: Router, http: Http, userService: UserService) {
    this.userService = userService;
    this.http = http;

  }

  public emit_event(location: string) {
    this.notify.emit(location);
  }

  /**
   * take imput for user email, send it to the server to check if valid
   * */
  submitUserEmail() {
    this.user.UserEmail = this.useremail;
    if (this.user.UserEmail == null || this.user.UserEmail == '' || this.password == null) {
      alert("Please Enter your email and password");
    } else {
      console.log("user email " + this.user.UserEmail);
      let params: URLSearchParams = new URLSearchParams();
      var pageHeaders = new Headers({
        'Content-Type': 'application/json'
      });
      let options = new RequestOptions({
        search: params,
        headers: pageHeaders
      });
      console.log("just before post");
      var body = JSON.stringify({ UserName:this.useremail, UserPwd:this.password });
      console.log(this.consts.url + 'LDAP');
      this.http.post(this.consts.url + 'LDAP', body, options)
        .subscribe((data) => this.waitForHttp(data));
    }
  }

  waitForHttp(data: any) {
    console.log("data " + data.text());
    this.value = JSON.parse(data.text()) as Auth;
    console.log("in the wait " + this.value);
    this.user.EntryGroup = this.value.UserType;
    this.user.token = this.value.SessionKey;
    this.user.UserID = this.value.UserID;
    if (data == undefined) {
      alert("no data");
    } else if (this.user.EntryGroup != 0) {
      if (this.user.EntryGroup == 1) { this.user.nav = this.consts.employee; }
      this.userService.setUser(this.user);
      this.emit_event("nav");
      this.userEntry = "none";
      this.mainPage = "block";
    } else {
      alert("Not a valid email. Please try again");
    }
    console.log("waitForHttp is complete");
  }
  
}
