import { Routes } from '@angular/router';
import { LoginComponent } from './features/auth/components/login/login';
import { LayoutComponent } from './shared/components/layout/layout';
import { RecordsComponent } from './features/tasks/components/records/records';
import { UsersComponent } from './features/users/components/users/users';
import { SettingsComponent } from './features/settings/components/settings/settings';
import { TaskFormComponent } from './features/tasks/components/task-form/task-form';
import { UserFormComponent } from './features/users/components/user-form/user-form';
import { AdminDashboard } from './features/dashboard/components/admin-dashboard/admin-dashboard';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  
  {
    path: '',
    component: LayoutComponent,
    children: [
      { path: 'dashboard', component: AdminDashboard },
      { path: 'records', component: RecordsComponent },
      { path: 'records/create', component: TaskFormComponent },
      { path: 'records/edit/:id', component: TaskFormComponent },
      { path: 'users', component: UsersComponent },
      { path: 'users/create', component: UserFormComponent },
      { path: 'users/edit/:id', component: UserFormComponent },
      { path: 'settings', component: SettingsComponent },
      { path: '', redirectTo: 'records', pathMatch: 'full' }
    ]
  },
  
  { path: '**', redirectTo: 'login' }
];