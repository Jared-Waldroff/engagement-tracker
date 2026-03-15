import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpRequest, HttpHandler, HttpEvent, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../services/auth.service';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {
  constructor(
    private authService: AuthService,
    private snackBar: MatSnackBar
  ) {}

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        switch (error.status) {
          case 401:
            this.authService.logout();
            break;
          case 403:
            this.snackBar.open('Access denied.', 'Dismiss', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
            break;
          case 0:
          case 500:
          case 502:
          case 503:
            this.snackBar.open('Something went wrong. Please try again.', 'Dismiss', {
              duration: 5000,
              panelClass: ['error-snackbar']
            });
            break;
        }
        return throwError(() => error);
      })
    );
  }
}
