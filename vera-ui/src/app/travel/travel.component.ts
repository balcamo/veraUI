import { Component, OnInit, Input } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { Constants } from '../classes/constants';
import { AuthForm } from '../classes/travel-auth-form';
import { User, Auth } from '../classes/user';
import { UserService } from '../service/app.service.user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-travel',
  templateUrl: './travel.component.html',
  styleUrls: ['./travel.component.scss']
})
export class TravelComponent implements OnInit {
  // variables needed for class 
  authDisplay = "none";
  allAuthDisplay = "none";
  consts = new Constants();
  http: Http;
  authForms = [];
  user = new User();
  // temperary vars
  auth1 = new AuthForm();
  auth2 = new AuthForm();

  constructor(private router: Router, http: Http, userService: UserService) {
    this.http = http;
    this.user = userService.getUser();
  }
  
  ngOnInit() {
    // temperary info to be place holder
    this.auth1.String4 = "water and power conference";
    this.auth1.String5 = "Washington";
    this.auth1.String8 = "red";
    this.auth2.String4 = "Event 2";
    this.auth2.String5 = "Texas";
    this.auth2.String8 = "green";
    this.authForms.push(this.auth1);
    this.authForms.push(this.auth2);
    // end temp info
  }

  /**
   * This function will toggle the display of
   * the new travel authorization form
   **/
  displayAuth() {
    if (this.authDisplay == "none") {
      this.authDisplay = "block";
      this.allAuthDisplay = "none";
    } else {
      this.authDisplay = "none"
    }
  }

  /**
   * this displays all active authforms found on in the database
   * */
  displayAllAuth() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    let authInfo = new Auth();
    authInfo.SessionToken = this.user.token.substring(0,36);
    console.log(authInfo.SessionToken);
    authInfo.UserType = this.user.EntryGroup;
    this.http.get(this.consts.url + 'TravelAuth?tokenHeader={' + authInfo.SessionToken )
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => console.log("the return from the get "+data.text()));

    if (this.allAuthDisplay == "none") {
      this.allAuthDisplay = "block";
      this.authDisplay = "none";
    } else {
      this.allAuthDisplay = "none"
    }
  }

  /**
   * this function will allow necessary item to be done after
   * the http request has returned data
   * @param data : return from the http request
   */
  waitForHttp(data: any) {
    if (data == undefined) {
      alert()
    } else {
      this.authForms = data.text() as AuthForm[];

      console.log("finishing waitForHttp");
    }
  }

}
