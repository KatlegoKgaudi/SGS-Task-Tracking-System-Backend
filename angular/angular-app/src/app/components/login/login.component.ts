import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { LoginRequest } from '../../models/auth';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  standalone: false,
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  loginRequest: LoginRequest = { username: '', password: '' };
  loading = false;
  error = '';

  constructor(private authService: AuthService, private router: Router) {}

  async onSubmit() {
  this.loading = true;
  this.error = '';
  try {
    const response = await this.authService.login(this.loginRequest);
    this.loading = false;
    if (response.success) {
      this.router.navigate(['/dashboard']);
    } else {
      this.error = response.message || 'Login failed';
    }
  } catch (err) {
    this.loading = false;
    this.error = 'Login failed. Please check your credentials.';
  }
}
}