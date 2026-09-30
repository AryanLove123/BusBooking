import { Routes } from '@angular/router';

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
    path: "book/:busId",
    loadComponent: () => import("./features/booking/book/book.component").then((m) => m.BookComponent),
  },
  {
    path: "booking-confirmation/:id",
    loadComponent: () => import("./features/booking/confirmation/confirmation.component").then((m) => m.ConfirmationComponent),
  },
  {
    path: "view-bookings",
    loadComponent: () => import("./features/booking/view-bookings/view-bookings.component").then((m) => m.ViewBookingsComponent),
  }
];
