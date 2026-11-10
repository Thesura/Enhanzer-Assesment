import { Component } from '@angular/core';
import { PurchaseBillPage } from './features/purchase/pages/purchase-bill/purchase-bill';
import { LoginPage } from './features/auth/pages/login/login';
import { RouterLink, RouterOutlet } from '@angular/router';

@Component({
  imports: [RouterOutlet],
  selector: 'app-root',
  template: '<router-outlet />',
})
export class App {
}
