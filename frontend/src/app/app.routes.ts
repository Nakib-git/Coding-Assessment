import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'users',
    pathMatch: 'full',
  },
  {
    path: 'users',
    loadComponent: () =>
      import('./features/users/user-list/user-list.component').then(
        (m) => m.UserListComponent
      ),
    title: 'ERP Users',
  },
  {
    path: 'users/new',
    loadComponent: () =>
      import('./features/users/user-form/user-form.component').then(
        (m) => m.UserFormComponent
      ),
    title: 'Create User',
  },
  {
    path: 'users/:id/edit',
    loadComponent: () =>
      import('./features/users/user-form/user-form.component').then(
        (m) => m.UserFormComponent
      ),
    title: 'Edit User',
  },
  {
    path: '**',
    redirectTo: 'users',
  },
];
