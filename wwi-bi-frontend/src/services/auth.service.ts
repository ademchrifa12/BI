import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, from, tap, catchError, throwError, switchMap } from 'rxjs';
import { signInWithEmailAndPassword, signOut } from 'firebase/auth';
import { firebaseAuth } from '../app/firebase';
import { environment } from '../environments/environment';
import { AuthResponse, User } from '../models/models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly TOKEN_KEY = 'wwi_bi_token';
  private readonly USER_KEY = 'wwi_bi_user';

  private currentUserSignal = signal<User | null>(this.loadUserFromStorage());

  readonly currentUser = this.currentUserSignal.asReadonly();
  readonly isAdmin = computed(() => this.currentUserSignal()?.role === 'Admin');

  constructor(
    private http: HttpClient,
    private router: Router
  ) {}

  login(credentials: { username: string; password: string }): Observable<AuthResponse> {
    // Sign in with Firebase using email (username field is used as email)
    return from(signInWithEmailAndPassword(firebaseAuth, credentials.username, credentials.password)).pipe(
      switchMap(async (userCredential) => {
        const idToken = await userCredential.user.getIdToken();
        return idToken;
      }),
      switchMap((idToken: string) => {
        // Send Firebase ID token to backend for verification and role mapping
        return this.http.post<AuthResponse>(`${environment.apiUrl}/auth/login`, { idToken });
      }),
      tap(response => {
        if (response.success && response.token && response.user) {
          this.setSession(response.token, response.user);
        }
      }),
      catchError(error => {
        console.error('Login error:', error);
        // Map Firebase error codes to user-friendly messages
        const firebaseMessage = this.mapFirebaseError(error?.code);
        if (firebaseMessage) {
          return throwError(() => ({ error: { message: firebaseMessage } }));
        }
        return throwError(() => error);
      })
    );
  }

  logout(): void {
    signOut(firebaseAuth).catch(() => {});
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSignal.set(null);
    this.router.navigate(['/login']);
  }

  getToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  isAuthenticated(): boolean {
    const token = this.getToken();
    if (!token) return false;

    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      const expiry = payload.exp * 1000;
      return Date.now() < expiry;
    } catch {
      return false;
    }
  }

  hasRole(role: string): boolean {
    return this.currentUserSignal()?.role === role;
  }

  getCurrentUser(): Observable<any> {
    return this.http.get<any>(`${environment.apiUrl}/auth/me`);
  }

  private setSession(token: string, user: User): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.currentUserSignal.set(user);
  }

  private loadUserFromStorage(): User | null {
    const userJson = localStorage.getItem(this.USER_KEY);
    if (userJson) {
      try {
        return JSON.parse(userJson);
      } catch {
        return null;
      }
    }
    return null;
  }

  private mapFirebaseError(code: string | undefined): string | null {
    if (!code) return null;
    const map: Record<string, string> = {
      'auth/user-not-found': 'No account found with this email',
      'auth/wrong-password': 'Invalid password',
      'auth/invalid-email': 'Invalid email address',
      'auth/user-disabled': 'Account has been disabled',
      'auth/too-many-requests': 'Too many attempts. Try again later.',
      'auth/invalid-credential': 'Invalid email or password'
    };
    return map[code] || null;
  }
}
