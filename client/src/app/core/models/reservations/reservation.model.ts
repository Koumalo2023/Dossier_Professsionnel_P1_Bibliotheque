// Modèles de réservations

export interface Reservation {
  id: string;
  bookId: string;
  userId: string;
  bookTitle: string;
  userName: string;
  reservationDate: string;
  status: ReservationStatus;
  availableSince?: string;
  expiryDate: string;
}

export interface CreateReservationRequest {
  bookId: string;
}

export interface UpdateReservationStatusRequest {
  status: string;
  availableSince?: string;
}

// Statuts de réservation
export enum ReservationStatus {
  PENDING = 'PENDING',
  AVAILABLE = 'AVAILABLE',
  CANCELLED = 'CANCELLED',
  EXPIRED = 'EXPIRED'
}