import { Component } from "@angular/core";
import { Validators, FormBuilder, FormGroup, ReactiveFormsModule } from "@angular/forms";
import { AuthService } from "../../../core/services/auth.service";
import { Router} from "@angular/router";
import { MatCard, MatCardHeader, MatCardTitle, MatCardSubtitle, MatCardContent, MatCardActions } from '@angular/material/card';
import { MatFormField, MatLabel, MatError } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInput } from '@angular/material/input';
import { MatButton, MatIconButton } from '@angular/material/button';
import { MatProgressSpinner } from '@angular/material/progress-spinner';
import { CommonModule } from "@angular/common";
import { RouterLink } from "@angular/router";

@Component({
    selector: 'app-login',
    templateUrl: 'login.component.html',
    styleUrl: 'login.component.scss',
    standalone: true,
    imports: [
        MatCard,
        MatCardHeader,
        MatCardTitle,
        MatCardSubtitle,
        MatCardContent,
        MatCardActions,
        MatFormField,
        MatLabel,
        MatInput,
        MatIcon,
        MatError,
        MatButton,
        MatIconButton,
        MatProgressSpinner,
        ReactiveFormsModule,
        CommonModule,
        RouterLink,
    ],
})
export class LoginComponent {
    loginForm: FormGroup;
    isLoading = false;
    errorMessage = '';
    hidePassword = true

    constructor(private fb:FormBuilder,
        private authService: AuthService,
        private router: Router
    ) {
        this.loginForm = this.fb.group({
            email: ['', [Validators.required, Validators.email]],
            password: ['', [Validators.required, Validators.minLength(6)]]
        });
    }

    onSubmit(): void {
        if(this.loginForm.invalid)
        {
            return;
        }

        this.isLoading = true;
        this.errorMessage = '';

        this.authService.login(this.loginForm.value).subscribe({
            next: () => {
                this.isLoading = false;
                this.router.navigate(['dashboard']);
            },
            error: (err) => {
                this.isLoading = false;
                this.errorMessage = err.error?.message || "Invalid email or password"
            }
        });
    }

    get email() { return this.loginForm.get('email')};
    get password() { return this.loginForm.get('password') };
}