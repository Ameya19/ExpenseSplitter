export interface User {
    id: string;
    displayName: string;
    email: string;
}

export interface AuthResponse {
    userId: string;
    token: string;
    email: string;
    displayName: string;
}

export interface RegisterRequest {
    displayName: string;
    email: string;
    password: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}