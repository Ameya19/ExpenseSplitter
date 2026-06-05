import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatDividerModule } from '@angular/material/divider';
import { MatSnackBar, MatSnackBarModule } from '@angular/material/snack-bar';
import { MatTabsModule } from '@angular/material/tabs';
import { ProfileService } from '../../../core/services/profile.service';
import { UserProfile } from '../../../core/models/profile.model';
import { AuthService } from '../../../core/services/auth.service';

@Component({
    selector: 'app-profile-view',
    templateUrl: 'profile-view.component.html',
    styleUrl: 'profile-view.component.scss',
    imports: [MatCardModule, MatButtonModule, MatIconModule, MatInputModule, MatTabsModule, MatFormFieldModule, MatProgressSpinnerModule, MatDividerModule, MatSnackBarModule, ReactiveFormsModule, CommonModule]
})
export class ProfileViewComponent implements OnInit {
    profile: UserProfile | null = null;
    profileForm: FormGroup;
    passwordForm: FormGroup;
    isLoadingProfile = true;
    isSavingProfile = false;
    isSavingPassword = false;
    profileError = '';
    passwordError = '';
    hideCurrentPassword = true;
    hideNewPassword = true;
    hideConfirmPassword = true;

    constructor(
        private profileService: ProfileService,
        private fb: FormBuilder,
        private authService: AuthService,
        private snackBar: MatSnackBar
    ) {
        this.profileForm = this.fb.group({
            displayName: ['', [Validators.required, Validators.minLength(2)]],
            avatarUrl: ['']
        });

        this.passwordForm = this.fb.group({
            currentPassword: ['', [Validators.required]],
            newPassword: ['', [Validators.required, Validators.minLength(6)]],
            confirmPassword: ['', [Validators.required]]
        }, { validators: this.passwordMatchValidator });
    }

    ngOnInit(): void {
        this.isLoadingProfile = true;
        this.profileService.getProfile().subscribe({
            next: (profile) => {
                this.profile = profile;
                this.isLoadingProfile = false;
            },
            error: () => {
                this.isLoadingProfile = false;
            }
        });
    }

    passwordMatchValidator(control: AbstractControl) {
        const newPassword = control.get('newPassword')?.value;
        const confirmPassword = control.get('confirmPassword')?.value;
        return newPassword === confirmPassword ? null : { passwordMismatch: true };
    }

    onSaveProfile(): void {
        if(this.profileForm.invalid)
            return;

        this.isSavingProfile = true;
        this.profileError = '';

        this.profileService.updateProfile(this.profileForm.value).subscribe({
            next: (updated) => {
                this.profile = updated;
                this.isSavingProfile = false;

                const user = this.authService.getCurrentUser();
                if(user) {
                    localStorage.setItem('user', JSON.stringify({
                        ...user,
                        displayName: updated.displayName
                    }));
                }

                this.snackBar.open('Profile updated successfully!', 'Close', { duration: 3000 })
            },
            error: () => {
                this.isSavingProfile = false;
                this.profileError = 'Failed to update profile.';
            }
        });
    }

    onChangePassword(): void {
        if(this.passwordForm.invalid)
            return;

        this.isSavingPassword = true;
        this.passwordError = '';

        this.profileService.changePassword(this.passwordForm.value).subscribe({
            next: () => {
                this.isSavingPassword = false;
                this.passwordForm.reset();

                this.snackBar.open('Password updated sucessfully!', 'Close', { duration: 3000 });
            },
            error: () => {
                this.isSavingPassword = false;
                this.passwordError = 'Failed to update password.';
            }
        })
    }

    getInitial(): string {
        return this.profile?.displayName?.charAt(0)?.toUpperCase() || '?';
    }

    get displayName() { return this.profileForm.get('displayName'); }
    get currentPassword() { return this.profileForm.get('currentPassword'); }
    get newPassword() { return this.profileForm.get('newPassword'); }
    get confirmPassword() { return this.profileForm.get('confirmPassword'); }
}