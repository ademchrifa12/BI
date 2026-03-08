import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { roleGuard, adminGuard } from './core/guards/role.guard';

export const routes: Routes = [
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },
  {
    path: 'dashboard',
    loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent),
    canActivate: [authGuard]
  },
  {
    path: 'customers',
    loadComponent: () => import('./features/customers/customers.component').then(m => m.CustomersComponent),
    canActivate: [authGuard]
  },
  {
    path: 'customers/:id',
    loadComponent: () => import('./features/customer-detail/customer-detail.component').then(m => m.CustomerDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'orders',
    loadComponent: () => import('./features/orders/orders.component').then(m => m.OrdersComponent),
    canActivate: [authGuard]
  },
  {
    path: 'orders/:id',
    loadComponent: () => import('./features/order-detail/order-detail.component').then(m => m.OrderDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'invoices',
    loadComponent: () => import('./features/invoices/invoices.component').then(m => m.InvoicesComponent),
    canActivate: [authGuard]
  },
  {
    path: 'invoices/:id',
    loadComponent: () => import('./features/invoce-details/invoice-detail.component').then(m => m.InvoiceDetailComponent),
    canActivate: [authGuard]
  },
  {
    path: 'products',
    loadComponent: () => import('./features/products/products.component').then(m => m.ProductsComponent),
    canActivate: [authGuard]
  },
  {
    path: 'users',
    loadComponent: () => import('./features/users/users.component').then(m => m.UsersComponent),
    canActivate: [adminGuard]
  },
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
