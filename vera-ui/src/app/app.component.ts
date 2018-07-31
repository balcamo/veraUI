import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { Headers, Http, URLSearchParams, RequestOptions } from '@angular/http';
import { User } from './classes/user';
import { UserService } from './service/app.service.user';
import { Constants } from './classes/constants';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  user: User;
  userName: string;
  http: Http;
  userService: UserService;
  userEntry = "block";
  mainPage = "none";
  consts = new Constants();

  constructor(private router: Router, http: Http, userService: UserService) {
    this.http = http;
    this.userService = userService;

  }

  reloadOverwrite() {
    console.log("this should print on refresh");
    this.router.navigate([''], { replaceUrl: true });
  }

  submitUserName() {
    if (this.userName == null) {
      alert("Please Enter your user name");
    } else {
      this.userEntry = "none";
      this.mainPage = "block";
      let params: URLSearchParams = new URLSearchParams();
      var pageHeaders = new Headers({
        'Content-Type': 'application/json'
      });
      let options = new RequestOptions({
        search: params,
        headers: pageHeaders
      });
      var body = JSON.stringify(this.userName);
      console.log(this.consts.url + 'User');
      this.http.get(this.consts.url + 'User?userName={' + this.userName+'}')
        .subscribe((data) => console.log(data.text()));
    }
  }
  
}
