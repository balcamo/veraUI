import { Component, OnInit } from '@angular/core';
import { AdalService } from 'adal-angular4';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {
  user: any;

  constructor(private adalService: AdalService, protected http: HttpClient) {
  }

  ngOnInit() {
    this.user = this.adalService.userInfo;

    this.user.token = this.user.token.substring(0, 10) + '...';
  }

}
