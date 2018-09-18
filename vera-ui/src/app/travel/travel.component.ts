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

  constructor(private router: Router, http: Http, userService: UserService) {
    this.http = http;
    this.user = userService.getUser();
  }
  
  ngOnInit() {

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
    authInfo.SessionKey = this.user.token.substring(0,36);
    console.log(authInfo.SessionKey);
    authInfo.UserType = this.user.EntryGroup;
    this.http.get(this.consts.url + 'TravelAuth?tokenHeader=' + authInfo.SessionKey )
      .subscribe((data) => this.waitForHttp(data));
      //.subscribe((data) => console.log("the return from the get "+data.text()));

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
    console.log("the data : " + data.text());
    if (JSON.parse(data.text()) == []) {
      console.log("Data returned is null");

    } else {
      this.authForms = JSON.parse(data.text()) as AuthForm[];

      console.log("finishing waitForHttp");
    }
  }

}
