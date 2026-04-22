export interface AuthResponseDto {
  id: string;
  username: string;
  token: string;
}

// These match the Commands we created in the C# backend
export interface LoginCommand {
  username: string;
  password: string;
}

export interface RegisterCommand {
  username: string;
  password: string;
}
