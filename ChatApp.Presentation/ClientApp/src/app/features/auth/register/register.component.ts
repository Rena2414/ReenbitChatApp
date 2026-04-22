import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
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

    const username = this.username.trim();
    const password = this.password;

    if (username.length < 3 || username.length > 50) {
      this.usernameError = 'Username must be 3–50 characters.';
    }

    if (
      password.length < 8 ||
      !/[A-Z]/.test(password) ||
      !/[a-z]/.test(password) ||
      !/[0-9]/.test(password)
    ) {
      this.passwordError = 'Password must be 8+ chars with upper, lower, and number.';
    }

    return !this.usernameError && !this.passwordError;
  }

  onSubmit() {
    this.errorMessage = '';
    if (!this.validate()) return;

    this.isLoading = true;

    this.authService.register({ username: this.username.trim(), password: this.password })
      .subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => {
          this.errorMessage = err.error?.detail || err.error?.message || 'Registration failed.';
          this.isLoading = false;
        }
      });
  }
}
