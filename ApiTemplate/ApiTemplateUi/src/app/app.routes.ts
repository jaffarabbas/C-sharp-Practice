import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { HomeComponent } from './components/home/home/home.component';

export const routes: Routes = [
    // {path: '', redirectTo: 'home', pathMatch: 'full'},
    {path: 'login',component: LoginComponent},
    {path:'register',component:RegisterComponent},
    {path:'home',component:HomeComponent},
];