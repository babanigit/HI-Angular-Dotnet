export interface IUser {
  id: number | string;
  name: string;
  mail: string;
  password: string;
  todo_id: number[];
}

export interface ILoggedIn_User {
  success: boolean;
  message: string;
  data: Data;
}
interface Data {
  id: string;
  email: string;
  userName: string;
}
