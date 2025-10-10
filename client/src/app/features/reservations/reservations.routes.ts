import { Routes } from '@angular/router';
import { ReservationListComponent } from './reservation-list/reservation-list.component';
import { ReservationCreateComponent } from './reservation-create/reservation-create.component';
import { ReservationCancelComponent } from './reservation-cancel/reservation-cancel.component';
import { ReservationsLayoutComponent } from './reservations-layout/reservations-layout.component';

export const reservationsRoutes: Routes = [
  {
    path: '',
    component: ReservationsLayoutComponent,
    children: [
      { path: '', component: ReservationListComponent },
      { path: 'create', component: ReservationCreateComponent },
      { path: 'create/:bookId', component: ReservationCreateComponent },
      { path: 'cancel/:id', component: ReservationCancelComponent }
    ]
  }
];