import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { AuthResponseDto, LoginCommand, RegisterCommand } from '../../models/auth.models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private currentUserSubject = new BehaviorSubject<AuthResponseDto | null>(this.getStoredUser());
  public currentUser$ = this.currentUserSubject.asObservable();

  constructor(private http: HttpClient, private router: Router) {}

  public get currentUserValue(): AuthResponseDto | null {
    return this.currentUserSubject.value;
  }

  login(command: LoginCommand): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>('/api/auth/login', command).pipe(
      tap(response => this.storeUser(response))
    );
  }

  register(command: RegisterCommand): Observable<AuthResponseDto> {
    return this.http.post<AuthResponseDto>('/api/auth/register', command).pipe(
      tap(response => this.storeUser(response))
    );
  }

  logout() {
    localStorage.removeItem('chat_user');
    this.currentUserSubject.next(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return this.currentUserValue?.token || null;
  }

  private storeUser(user: AuthResponseDto) {
    localStorage.setItem('chat_user', JSON.stringify(user));
    this.currentUserSubject.next(user);
  }

  private getStoredUser(): AuthResponseDto | null {
    const userJson = localStorage.getItem('chat_user');
    return userJson ? JSON.parse(userJson) : null;
  }
}
