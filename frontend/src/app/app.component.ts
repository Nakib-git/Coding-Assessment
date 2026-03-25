import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  template: `
    <nav class="top-nav">
      <span class="brand">⚙ ERP User Manager</span>
    </nav>
    <main class="main-content">
      <router-outlet />
    </main>
  `,
  styles: [`
    .top-nav {
      background: #1a1a2e;
      color: #fff;
      padding: 0.85rem 2rem;
      display: flex;
      align-items: center;
    }
    .brand { font-size: 1.1rem; font-weight: 600; letter-spacing: 0.5px; }
    .main-content { padding: 2rem; max-width: 1200px; margin: 0 auto; }
  `],
})
export class AppComponent {}
