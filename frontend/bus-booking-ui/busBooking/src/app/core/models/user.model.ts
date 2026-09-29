export interface LoginResponse{
    token: string;
    userId: number;
    name: string;
    email: string;
    role: "User" | "Admin";
    expiresAtUtc: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}
