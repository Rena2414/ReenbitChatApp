import { HttpInterceptorFn } from '@angular/common/http';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  return next(req).pipe(
    catchError(err => {
      console.error('API Error intercepted:', err);
      // In a real app, you might trigger a Toast/Snackbar notification here
      return throwError(() => err);
    })
  );
};
