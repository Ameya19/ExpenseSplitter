export interface UpdateProfileRequest {
    displayName: string;
    avatarUrl?: string;
}

export interface ChangePasswordRequest {
    currentPassword: string;
    newPassword: string;
    confirmPassword: string;
}

export interface UserProfile {
    id: string;
    displayName: string;
    email: string;
    avatarUrl?: string;
    createdAt: Date;
}