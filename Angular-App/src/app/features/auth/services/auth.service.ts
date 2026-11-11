import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Using Angular Signals to track authentication state reactively
  isLoggedIn = signal<boolean>(this.loggedIn());

  private loggedIn(): boolean {
    return !!sessionStorage.getItem('username');
  }

  saveSession(username : string) {
    sessionStorage.setItem('username', username);
    this.isLoggedIn.set(true);
  }

  getUsername(): string | null {
    return sessionStorage.getItem('username');
  }

  logout() {
    sessionStorage.removeItem('username');
    this.isLoggedIn.set(false);
  }
}
