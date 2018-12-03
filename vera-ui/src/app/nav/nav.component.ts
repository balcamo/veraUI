import { Component, Input, OnInit, Output } from '@angular/core';
import { Constants } from '../classes/constants';
import { User } from '../classes/user';
import { UserService } from '../service/app.service.user';
import { AdalService } from 'adal-angular4';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  @Input() nav;
  //@Output() navList = new EventEmitter();
  const = new Constants()
 // public nav = [];
  user = new User();
  userService: UserService;

  constructor(userService: UserService, private adalService: AdalService) {
    this.userService = userService;
  }

  ngOnInit() { }

  logout() {
    this.adalService.logOut();
  }
}
