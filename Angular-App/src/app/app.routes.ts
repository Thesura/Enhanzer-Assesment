import { Routes } from '@angular/router';
import { LoginPage } from './features/auth/pages/login/login';
import { PurchaseBillPage } from './features/purchase/pages/purchase-bill/purchase-bill';
import { authGuard } from './features/util/authGuard.util';

export const routes: Routes = [
    {
        path: "",
        component: LoginPage
    },
    {
        path: "purchase-bill",
        component: PurchaseBillPage,
        canActivate: [authGuard]
    }
];
