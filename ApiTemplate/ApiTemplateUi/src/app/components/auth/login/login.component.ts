import { CommonModule } from '@angular/common';
import { AfterViewInit, Component, NgModule, Renderer2, Inject, PLATFORM_ID, } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { isPlatformBrowser } from '@angular/common';

@Component({
  selector: 'app-root',
  imports: [
    CommonModule,
    FormsModule
  ],
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements AfterViewInit {
  username = '';
  password = '';
  rememberMe = false;
  showPassword = false;
  loading = false;
  particles = Array(9).fill(0);

  constructor(
    private renderer: Renderer2,
    @Inject(PLATFORM_ID) private platformId: Object
  ) {}

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
    this.loading = true;
    setTimeout(() => {
      this.loading = false;
      alert('Login successful! 🎉\n\nIn a real Angular 19 app, this would:\n• Use Angular Reactive Forms\n• Implement proper authentication\n• Route to the dashboard\n• Use Angular Material or similar UI library');
    }, 2000);
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
