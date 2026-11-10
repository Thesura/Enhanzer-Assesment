import { Component, signal } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-login-page',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class LoginPage {
  protected readonly submitted = signal(false);
  protected readonly statusMessage = signal('');

  protected readonly loginForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required, Validators.email],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [Validators.required],
    }),
  });

  protected submit(): void {
    this.submitted.set(true);
    this.statusMessage.set('');

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    this.statusMessage.set('Sign-in is not connected yet. Please contact your administrator.');
  }

  protected showError(field: 'email' | 'password'): boolean {
    return this.loginForm.controls[field].invalid &&
      (this.loginForm.controls[field].touched || this.submitted());
  }
}
