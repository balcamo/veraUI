import { Component, OnInit, Input } from '@angular/core';
import { Headers, Http, URLSearchParams, RequestOptions, Response } from '@angular/http';
import { Constants } from '../classes/constants';
import { AuthForm } from '../classes/travel-auth-form';
import { User } from '../classes/user';
import { UserService } from '../service/app.service.user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-travel',
  templateUrl: './travel.component.html',
  styleUrls: ['./travel.component.scss']
})
export class TravelComponent implements OnInit {

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
   * the travel authorization form*/
  displayAuth() {
    if (this.authDisplay == "none") {
      this.authDisplay = "block";
      this.allAuthDisplay = "none";
    } else {
      this.authDisplay = "none"
    }
  }

  displayAllAuth() {
    let params: URLSearchParams = new URLSearchParams();
    var pageHeaders = new Headers({
      'Content-Type': 'application/json'
    });
    let options = new RequestOptions({
      search: params,
      headers: pageHeaders
    });
    var body = JSON.stringify(this.user.UserName);
    console.log(this.consts.url + 'TravelAuth');
    this.http.get(this.consts.url + 'TravelAuth?userEmail={' + this.user.UserEmail + '}')
      //.subscribe((data) => this.waitForHttp(data));
      .subscribe((data) => console.log(data.text()));

    if (this.allAuthDisplay == "none") {
      this.allAuthDisplay = "block";
      this.authDisplay = "none";
    } else {
      this.allAuthDisplay = "none"
    }
  }

  waitForHttp(data: any) {
    if (data == undefined) {
      alert()
    } else {
      this.authForms = data.text() as AuthForm[];

      console.log("finishing waitForHttp");
    }
  }

}
