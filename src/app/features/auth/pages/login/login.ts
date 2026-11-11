import { Component, signal, inject } from '@angular/core';
import { FormControl, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { ValidatorUtil } from '../../utils/validator.util';
import { FetchRequest } from '../../../util/fetchRequest.util';
import { AuthService } from '../../services/auth.service';

@Component({
  imports: [ReactiveFormsModule],
  selector: 'app-login-page',
  styleUrl: './login.css',
  templateUrl: './login.html',
})
export class LoginPage {
  protected readonly submitted = signal(false);
  protected readonly statusMessage = signal('');
  protected readonly loading = signal(false);
  protected readonly url = 'http://localhost:5068/api/auth/login';
  protected authService = inject(AuthService);
  protected router = inject(Router);
  protected readonly loginForm = new FormGroup({
    email: new FormControl('', {
      nonNullable: true,
      validators: [ValidatorUtil.required(), ValidatorUtil.email()],
    }),
    password: new FormControl('', {
      nonNullable: true,
      validators: [ValidatorUtil.required()],
    }),
  });

  ngOnInit() {
    if(this.authService.isLoggedIn()) {
      this.router.navigate(['/purchase-bill']);
    }
  }

  protected async submit(): Promise<void> {
    if (this.loading()) {
      return;
    }

    this.submitted.set(true);
    this.statusMessage.set('');
    this.loginForm.controls.email.setValue(
      ValidatorUtil.sanitizeInput(this.loginForm.controls.email.value),
    );

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }


    const fetchRequest = new FetchRequest(
      this.url,
      {email: this.loginForm.controls.email.value, 
      password: this.loginForm.controls.password.value},
      'POST'
    );

    try {
      this.loading.set(true);
      const response = await fetchRequest.execute<any>();
      if(response.status === 401) {
        this.statusMessage.set('Invalid credentials. Please check your email and password.');
      }else if(response.status === 200) {
        console.log('Login successful:', response.data);
        if(response.data && response.data.loggedIn) {
          this.authService.saveSession(response.data.username);
          this.router.navigate(['/purchase-bill']);
        }
      }else{
        this.statusMessage.set('An unexpected error occurred. Please try again later.');
      }


      // Handle successful login response
    } catch (error) {
      this.statusMessage.set('An error occurred while trying to sign in. Please try again.');
    } finally {
      this.loading.set(false);
    }
  }

  protected showError(field: 'email' | 'password'): boolean {
    return this.loginForm.controls[field].invalid &&
      (this.loginForm.controls[field].touched || this.submitted());
  }
}
