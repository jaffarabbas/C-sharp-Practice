import { Routes } from '@angular/router';
import { LoginComponent } from './components/auth/login/login.component';
import { RegisterComponent } from './components/auth/register/register.component';
import { HomeComponent } from './components/home/home/home.component';
import { TesterComponent } from './components/tester/tester.component';
export const routes: Routes = [
    {path: '', redirectTo: 'test2', pathMatch: 'full'},
    {path: 'login',component: LoginComponent},
    {path:'register',component:RegisterComponent},
    {path:'home',component:HomeComponent},
    {path:'test2',component:TesterComponent}
];