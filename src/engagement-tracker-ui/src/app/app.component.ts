import { Component } from '@angular/core';
import { AuthService } from './core/services/auth.service';
import { Router } from '@angular/router';

@Component({
  standalone: false,
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  constructor(
    public authService: AuthService,
    private router: Router
  ) {}

  onLogout(): void {
    this.authService.logout();
  }

  isLoginPage(): boolean {
    return this.router.url === '/login';
  }
}
