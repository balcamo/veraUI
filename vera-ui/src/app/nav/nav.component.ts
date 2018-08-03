import { Component, OnInit } from '@angular/core';
import { Constants } from '../classes/constants';
import { User } from '../classes/user';
import { UserService } from '../service/app.service.user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.scss']
})
export class NavComponent implements OnInit {
  const = new Constants()
  public nav = [];
  user = new User();
  userService: UserService;

  constructor(userService: UserService) {
    this.userService = userService;
    
    this.checkNavList();

  }

  ngOnInit() {}

  checkNavList() {
    this.user = this.userService.getUser();
    console.log(this.user);
    if (this.user !== undefined) {
      this.nav = this.user.nav
    }
    console.log(this.nav);
  }
}
