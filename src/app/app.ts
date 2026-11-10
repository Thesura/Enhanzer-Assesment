import { Component } from '@angular/core';
import { LoginPage } from './features/auth/pages/login/login';

@Component({
  imports: [LoginPage],
  selector: 'app-root',
  template: '<app-login-page />',
})
export class App {
}
