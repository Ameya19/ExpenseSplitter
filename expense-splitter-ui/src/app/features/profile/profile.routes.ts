import { Routes } from "@angular/router";

export const ProfileRoutes: Routes = [
    {
        path: '',
        loadComponent: () =>
            import('./profile-view/profile-view.component').then(m => m.ProfileViewComponent)
    }
]