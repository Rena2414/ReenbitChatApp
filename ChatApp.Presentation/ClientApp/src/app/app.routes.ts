import { Routes } from '@angular/router';
import { ChatComponent } from './features/chat/chat.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  { path: '', component: ChatComponent, canActivate: [authGuard] }, // Protected route!
  { path: '**', redirectTo: '' }
];
