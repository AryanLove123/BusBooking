import { Routes } from '@angular/router';
import { adminGuard } from './core/guards/admin.guard';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./features/buses/search/search.component').then((m) => m.SearchComponent),
  },
  {
    path: 'results',
    loadComponent: () =>
      import('./features/buses/results/results.component').then((m) => m.ResultsComponent),
  },
  {
    path: 'login',
    loadComponent: () =>
      import('./features/auth/login/login.component').then((m) => m.LoginComponent),
  },
  {
    path: 'register',
    loadComponent: () =>
      import('./features/auth/register/register.component').then((m) => m.RegisterComponent),
  },
  {
    path: "view-bookings",
    loadComponent: () => import("./features/booking/view-bookings/view-bookings.component").then((m) => m.ViewBookingsComponent),
    canActivate: [authGuard]
  },
  {
    path: "admin",
    loadComponent: () => import("./features/admin/admin.component").then((m) => m.AdminComponent),
    canActivate: [adminGuard],
  },
  {
    path: "book/:busId",
    loadComponent: () => import("./features/booking/book/book.component").then((m) => m.BookComponent),
    canActivate: [authGuard]
  },
  {
    path: "booking-confirmation/:id",
    loadComponent: () => import("./features/booking/confirmation/confirmation.component").then((m) => m.ConfirmationComponent),
    canActivate: [authGuard]
  },
  {
    path: "bookings/:id/edit",
    loadComponent: () => import("./features/booking/book/book.component").then((m) => m.BookComponent),
    canActivate: [authGuard]
  },
  { path: "**", redirectTo: "" }
];
