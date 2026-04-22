import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth/auth.service';

@Component({
  selector: 'app-register', // Fixed selector
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'] // Fixed stylesheet reference
})
export class RegisterComponent {
  username = '';
  password = '';
  errorMessage = '';
  isLoading = false;

  constructor(private authService: AuthService, private router: Router) {
    if (this.authService.currentUserValue) {
      this.router.navigate(['/']);
    }
  }

  onSubmit() {
    if (!this.username || !this.password) return;
    this.isLoading = true;
    this.errorMessage = '';

    this.authService.register({ username: this.username, password: this.password })
      .subscribe({
        next: () => this.router.navigate(['/']),
        error: (err) => {
          // Adjusted error message for registration
          this.errorMessage = err.error?.message || 'Registration failed. Username may already be taken.';
          this.isLoading = false;
        }
      });
  }
}
