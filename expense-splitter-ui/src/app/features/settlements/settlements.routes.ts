export const settlementRoutes = [
    {
        path: '',
        loadComponent: () => 
            import('./settlement-list/settlement-list.component').then(m => m.SettlementListComponent)
    }
];