import { Routes } from '@angular/router';
import { LoanListComponent } from './loan-list/loan-list.component';
import { LoanRequestComponent } from './loan-request/loan-request.component';
import { LoanReturnComponent } from './loan-return/loan-return.component';
import { LoansLayoutComponent } from './loans-layout/loans-layout.component';

export const loansRoutes: Routes = [
  {
    path: '',
    component: LoansLayoutComponent,
    children: [
      { path: '', component: LoanListComponent },
      { path: 'request', component: LoanRequestComponent },
      { path: 'request/:bookId', component: LoanRequestComponent },
      { path: 'return/:id', component: LoanReturnComponent }
    ]
  }
];