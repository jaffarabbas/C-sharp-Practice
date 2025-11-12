import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, NgModule, Renderer2, Inject, PLATFORM_ID, } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { isPlatformBrowser } from '@angular/common';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { AuthStorageService } from '../../../services/auth-storage.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements AfterViewInit {
  loginForm: FormGroup;
  showPassword = false;
  loading = false;
  errorMessage = '';
  successMessage = '';
  particles = Array(9).fill(0);

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private authStorage: AuthStorageService,
    private router: Router,
    private renderer: Renderer2,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {
    this.loginForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      rememberMe: [false]
    });
  }

  ngAfterViewInit() {
    if (isPlatformBrowser(this.platformId)) {
      // Floating input animation
      const inputs = document.querySelectorAll('.form-input') as NodeListOf<HTMLInputElement>;
      inputs.forEach(input => {
        input.addEventListener('focus', () => input.style.transform = 'translateY(-2px)');
        input.addEventListener('blur', () => input.style.transform = 'translateY(0)');
      });

      // Ripple effect
      const rippleTargets = document.querySelectorAll('button, .social-btn');
      rippleTargets.forEach(btn => {
        btn.addEventListener('click', (e: Event) => this.createRipple(e as MouseEvent, btn as HTMLElement, this.renderer));
      });

      // Inject ripple keyframe style
      const style = this.renderer.createElement('style');
      style.textContent = `
        @keyframes ripple {
          to {
            transform: scale(4);
            opacity: 0;
          }
        }
      `;
      this.renderer.appendChild(document.head, style);
    }
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  onSubmit() {
    // Clear previous messages
    this.errorMessage = '';
    this.successMessage = '';

    // Check if form is valid
    if (this.loginForm.invalid) {
      this.errorMessage = 'Please fill in all required fields correctly.';
      Object.keys(this.loginForm.controls).forEach(key => {
        const control = this.loginForm.get(key);
        if (control?.invalid) {
          control.markAsTouched();
        }
      });
      return;
    }

    this.loading = true;

    const credentials = {
      username: this.loginForm.value.username,
      password: this.loginForm.value.password
    };

    this.authService.login(credentials).subscribe({
      next: (response) => {
        this.loading = false;

        if (response.statusCode === '200' && response.data) {
          // Save authentication data to local storage
          const authData = {
            token: response.data.token,
            loginDate: response.data.loginDate,
            roles: response.data.roles,
            username: credentials.username
          };

          this.authStorage.saveAuthData(authData);

          this.successMessage = 'Login successful! Redirecting...';

          // Redirect to home page after a short delay
          setTimeout(() => {
            this.router.navigate(['/home']);
          }, 1000);
        } else {
          this.errorMessage = response.message || 'Login failed. Please try again.';
        }
      },
      error: (error) => {
        this.loading = false;
        console.error('Login error:', error);

        if (error.status === 401) {
          this.errorMessage = 'Invalid credentials or inactive account.';
        } else if (error.status === 429) {
          this.errorMessage = 'Too many login attempts. Please try again later.';
        } else if (error.status === 0) {
          this.errorMessage = 'Cannot connect to server. Please check your connection.';
        } else {
          this.errorMessage = error.error?.message || 'Login failed. Please try again later.';
        }
      }
    });
  }

  // Helper methods for form validation in template
  get username() {
    return this.loginForm.get('username');
  }

  get password() {
    return this.loginForm.get('password');
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.loginForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  getFieldError(fieldName: string): string {
    const field = this.loginForm.get(fieldName);
    if (field?.errors) {
      if (field.errors['required']) {
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} is required`;
      }
      if (field.errors['minlength']) {
        const minLength = field.errors['minlength'].requiredLength;
        return `${fieldName.charAt(0).toUpperCase() + fieldName.slice(1)} must be at least ${minLength} characters`;
      }
    }
    return '';
  }

  public createRipple(event: MouseEvent, element: HTMLElement, renderer: Renderer2) {
    const ripple = renderer.createElement('span');
    const rect = element.getBoundingClientRect();
    const size = Math.max(rect.width, rect.height);
    const x = event.clientX - rect.left - size / 2;
    const y = event.clientY - rect.top - size / 2;

    Object.assign(ripple.style, {
      position: 'absolute',
      width: `${size}px`,
      height: `${size}px`,
      left: `${x}px`,
      top: `${y}px`,
      background: 'rgba(255, 255, 255, 0.3)',
      borderRadius: '50%',
      transform: 'scale(0)',
      animation: 'ripple 0.6s linear',
      pointerEvents: 'none'
    });

    renderer.appendChild(element, ripple);
    setTimeout(() => renderer.removeChild(element, ripple), 600);
  }
}
