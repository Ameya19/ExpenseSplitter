import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
    {
        path: '',
        redirectTo: 'dashboard',
        pathMatch: 'full'
    },
    {
        path: 'auth',
        loadChildren: () =>
            import('./features/auth/auth.routes').then(m => m.authRoutes)
    },
    {
        path: 'dashboard',
        canActivate: [authGuard],
        loadComponent: () =>
            import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
    },
    {
        path: 'groups',
        canActivate: [authGuard],
        loadChildren: () =>
            import('./features/groups/groups.routes').then(m => m.groupRoutes)
    },
    {
        path: 'expenses',
        canActivate: [authGuard],
        loadChildren: () =>
            import('./features/expenses/expenses.routes').then(m => m.expenseRoutes)
    },
    {
        path: 'settlements',
        canActivate: [authGuard],
        loadChildren: () =>
            import('./features/settlements/settlements.routes').then(m => m.settlementRoutes)
    },
    {
        path: 'reports',
        canActivate: [authGuard],
        loadChildren: () => 
            import('./features/reports/reports.routes').then(m => m.reportRoutes)
    },
    {
        path: 'notifications',
        canActivate: [authGuard],
        loadChildren: () => 
            import('./features/notifications/notifications.routes').then(m => m.notificationRoutes)
    },
    {
        path: 'profile',
        canActivate: [authGuard],
        loadChildren: () =>
            import('./features/profile/profile.routes').then(m => m.ProfileRoutes)
    },
    {
        path: '**',
        redirectTo: 'dashboard'
    }
];
