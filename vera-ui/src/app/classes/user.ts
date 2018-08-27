import { AuthForm } from './travel-auth-form';

export class User {
  public UserName: string;
  public AuthForms: AuthForm[];
  public UserEmail: string;
  public EntryGroup = 0;
  public nav = [];
  public token: string;

  /*public SetUserName(name: string) {
    this.UserName = name;
  }
  public GetUserName() {
    return this.UserName;
  }

  public SetAuthForms(forms: AuthForm[]) {
    this.AuthForms = forms;
  }
  public GetAuthForms() {
    return this.AuthForms;
  }

  public SetUserEmail(email: string) {
    this.UserEmail = email;
  }
  public GetUserEmail() {
    return this.UserEmail;
  }

  public SetEntryGroup(group: number) {
    this.EntryGroup = name;
  }
  public GetEntryGroup() {
    return this.EntryGroup;
  }*/
}

export class Auth {
  public SessionToken: string;
  public UserType: number;
}
