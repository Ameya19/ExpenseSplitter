import { Routes } from '@angular/router';

export const groupRoutes: Routes = [
    {
        path:'',
        loadComponent: () =>
            import('./group-list/group-list.component').then(m => m.GroupListComponent)
    },
    {
        path:'create',
        loadComponent: () =>
            import('./group-form/group-form.component').then(m => m.GroupFormComponent)
    },
    {
        path:':id',
        loadComponent: () =>
            import('./group-detail/group-detail.component').then(m => m.GroupDetailComponent)
    },
    {
        path:':id/edit',
        loadComponent: () =>
            import('./group-form/group-form.component').then(m => m.GroupFormComponent)
    }
]