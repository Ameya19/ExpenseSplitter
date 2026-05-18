import { Router } from '@angular/router';
import { AuthService } from './../../../core/services/auth.service';
import { Component } from "@angular/core";
import { AbstractControl, FormBuilder, FormGroup, Validators } from "@angular/forms";

@Component({
    selector: 'register.component.scss',
    template: 'register.component.html',
    imports: []
})
export class RegisterComponent {
    registerForm: FormGroup;
    isLoading = false;
    errorMessage = "";

    constructor(private fb: FormBuilder,
        private router: Router,
        private authService: AuthService
    ){
        this.registerForm = this.fb.group({
            email: ['', Validators.email, Validators.required],
            password: ['', Validators.required, Validators.minLength(6)],
            displayName: ['', Validators.required, Validators.minLength(2)],
            confirmPassword: ['', Validators.required]
        }, {Validators:  this.passwordMatchValidator });
    }

    passwordMatchValidator(control: AbstractControl) {
        const password = this.registerForm.get('password')?.value;
        const confirmPassword = this.registerForm.get('confirmPassword')?.value;

        return password == confirmPassword ? null : { passwordMismatch : true };
    }

    onSubmit(): void {
        if(this.registerForm.invalid)
        {
            return;
        }

        this.isLoading = true;
        this.errorMessage = "";

        this.authService.register(this.registerForm.value).subscribe({
            next: () =>{
                this.isLoading = false;
                this.router.navigate(["dashboard"]);
            },
            error: (err) => {
                this.isLoading = false;
                this.errorMessage = err.message;
            }
        });
    }

    get displayName() { return this.registerForm.get('displayName'); }
    get email() { return this.registerForm.get('email'); }
    get password() { return this.registerForm.get('password'); }
    get confirmPassword() { return this.registerForm.get('confirmPassword'); }
}