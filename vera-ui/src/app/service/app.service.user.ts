import { Injectable } from '@angular/core';
import { User } from '../classes/user';

@Injectable()

export class UserService {
  private userObject: User;

  public getUser(): User {
    return this.userObject;
  }

  public setUser(user: User) {
    this.userObject = user;
  }

}
