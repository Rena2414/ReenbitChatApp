import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  username = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  usernameError = '';
  passwordError = '';

  constructor(private authService: AuthService, private router: Router) {
    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  private validate(): boolean {
    this.usernameError = '';
    this.passwordError = '';

    if (!this.username.trim()) {
      this.usernameError = 'Username is required.';
    }
    if (!this.password) {
      this.passwordError = 'Password is required.';
    }

    return !this.usernameError && !this.passwordError;
  }

  onSubmit() {
    this.errorMessage = '';
    if (!this.validate()) return;

    this.isLoading = true;

    this.authService.login({ username: this.username.trim(), password: this.password })
      .subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => {
          this.errorMessage = err.error?.detail || err.error?.message || 'Invalid username or password.';
          this.isLoading = false;
        }
      });
  }
}
