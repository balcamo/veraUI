import { Component, Input, OnInit, Output } from '@angular/core';
import { Constants } from '../classes/constants';
import { User } from '../classes/user';
import { UserService } from '../service/app.service.user';

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

  constructor(userService: UserService) {
    this.userService = userService;
  }

  ngOnInit() {}

}
