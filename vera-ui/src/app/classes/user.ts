import { AuthForm } from './travel-auth-form';

export class User {
  public AuthForms: AuthForm[];
  public UserEmail: string;
  public UserID: string;
  public EntryGroup = 0;
  public nav = [];
  public token: string;


}

export class Auth {
  public UserID: number;
  public SessionToken: string;
  public UserType: number;

}

