import { Routes } from '@angular/router';

export const reportRoutes: Routes = [
    {
        path: '',
        loadComponent: () => 
            import('./report-view/report-view.component').then(m => m.ReportViewComponent)
    }
];