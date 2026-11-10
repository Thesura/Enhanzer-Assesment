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
  protected readonly url = 'https://ez-staging-api.azurewebsites.net/api/External_Api/POS_Api/Invoke';
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

  protected async submit(): Promise<any> {
    this.submitted.set(true);
    this.statusMessage.set('');
    this.loginForm.controls.email.setValue(
      ValidatorUtil.sanitizeInput(this.loginForm.controls.email.value),
    );

    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    

    const payload = {
        API_Action: "GetLoginData",
        Device_Id: "D001",
        Sync_Time: "",
        Company_Code: this.loginForm.controls.email.value,
        API_Body: {
            Username: this.loginForm.controls.email.value,
            Pw: this.loginForm.controls.password.value,
        },
      };

    const fetchRequest = new FetchRequest(
      this.url,
      payload,
      'POST'
    );

    console.log('Submitting login request with payload:', payload);

    try {
      const response = await fetchRequest.execute<any>();
      let body: any;
      let locations: any[];
      console.log('Login successful:', response);
      console.log('Response status:', response.status);
      console.log('Response data:', response.data.Response_Body[0].User_Display_Name);
      if(response.status === 401) {
        this.statusMessage.set('Invalid credentials. Please check your email and password.');
      }else if(response.status === 200) {
        body = response.data.Response_Body[0];
        locations = body.User_Locations;
        this.authService.saveSession(body.User_Display_Name);
        this.router.navigate(['/purchase-bill']);
      }else{
        this.statusMessage.set('An unexpected error occurred. Please try again later.');
      }


      // Handle successful login response
    } catch (error) {
      this.statusMessage.set('An error occurred while trying to sign in. Please try again.');
    }
  }

  protected showError(field: 'email' | 'password'): boolean {
    return this.loginForm.controls[field].invalid &&
      (this.loginForm.controls[field].touched || this.submitted());
  }
}
