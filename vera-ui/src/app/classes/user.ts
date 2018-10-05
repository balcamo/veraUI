import { AuthForm } from './travel-auth-form';

export class User {
  public AuthForms: AuthForm[];
  public UserEmail: string;
  public UserID: number;
  public EntryGroup = [];
  public nav = [];
  public token: string;


}

export class Auth {
  public UserID: number;
  public SessionKey: string;
  public AccessKey = [];

}

