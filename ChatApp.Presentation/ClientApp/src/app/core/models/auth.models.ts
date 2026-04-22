export interface AuthResponseDto {
  id: string;
  username: string;
  token: string;
}

export interface LoginCommand {
  username: string;
  password: string;
}

export interface RegisterCommand {
  username: string;
  password: string;
}
