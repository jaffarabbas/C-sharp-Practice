import { Injectable, Renderer2 } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BackgroundService {

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
