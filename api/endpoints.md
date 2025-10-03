# 📚 Liste des Endpoints API — Gestion de Bibliothèque

> **Technologies** : ASP.NET Core Web API + Angular 18  
> **Authentification** : JWT + Identity  
> **Rôles** : `User`, `Manager`, `Admin`

---

## 🔐 1. Authentification & Utilisateurs

### `POST /api/auth/login`
**Public** — Connexion utilisateur

**Payload :**
```json
{
  "email": "user@example.com",
  "password": "Password123!"
}
```

**Réponse (200) :**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "name": "John Doe",
    "roles": ["User"]
  }
}
```

### `POST /api/auth/register`
**Public** — Inscription utilisateur (rôle User par défaut)

**Payload :**
```json
{
  "email": "newuser@example.com",
  "password": "SecurePass!1",
  "name": "Jane Smith"
}
```

### `GET /api/users/profile`
**User, Manager, Admin** — Récupérer le profil de l'utilisateur connecté

**Réponse (200) :**
```json
{
  "id": "guid",
  "email": "user@example.com",
  "name": "John Doe",
  "createdAt": "2025-04-01T00:00:00Z"
}
```

### `PUT /api/users/profile`
**User, Manager, Admin** — Mettre à jour le profil

**Payload :**
```json
{
  "name": "John Updated",
  "email": "john.updated@example.com"
}
```

### `GET /api/users`
**Admin uniquement** — Lister tous les utilisateurs

**Query Params :** `?page=1&pageSize=10&role=Manager`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "email": "user1@example.com",
      "name": "Alice",
      "roles": ["User"],
      "createdAt": "2025-04-01T00:00:00Z"
    }
  ],
  "totalCount": 42
}
```

### `PUT /api/users/{id}/role`
**Admin uniquement** — Modifier le rôle d'un utilisateur

**Payload :**
```json
{
  "role": "Manager"
}
```

⚠️ **Valide les rôles :** User, Manager, Admin

---

## 📖 2. Gestion des Livres

### `GET /api/books`
**Tous les rôles** — Lister les livres (avec filtres et pagination)

**Query Params :** `?category=Science-Fiction&author=Asimov&availableOnly=true&page=1&pageSize=20`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "title": "Fondation",
      "author": "Isaac Asimov",
      "category": {
        "id": "guid",
        "name": "Science-Fiction"
      },
      "isbn": "978-0-330-21357-6",
      "availableCopies": 3,
      "totalCopies": 5,
      "coverUrl": "https://.../cover.jpg"
    }
  ],
  "totalCount": 120
}
```

### `GET /api/books/{id}`
**Tous les rôles** — Détail d'un livre

**Réponse (200) :**
```json
{
  "id": "guid",
  "title": "Fondation",
  "author": "Isaac Asimov",
  "category": {
    "id": "guid",
    "name": "Science-Fiction"
  },
  "isbn": "978-0-330-21357-6",
  "publicationDate": "1951-01-01",
  "availableCopies": 3,
  "totalCopies": 5,
  "coverUrl": "https://.../cover.jpg",
  "createdAt": "2025-01-15T00:00:00Z"
}
```

### `POST /api/books`
**Manager, Admin** — Ajouter un livre

**Payload :**
```json
{
  "title": "Dune",
  "author": "Frank Herbert",
  "categoryId": "guid-de-la-catégorie",
  "isbn": "978-0-441-17271-9",
  "publicationDate": "1965-08-01",
  "totalCopies": 10,
  "coverUrl": "https://.../dune.jpg"
}
```

### `PUT /api/books/{id}`
**Manager, Admin** — Modifier un livre

**Payload :** même structure que POST

### `DELETE /api/books/{id}`
**Admin uniquement** — Supprimer un livre

⚠️ Vérifie qu'aucun emprunt/réservation actif n'existe

### `GET /api/books/search`
**Tous les rôles** — Recherche avancée de livres

**Query Params :** `?q=asimov&searchIn=title,author,description&page=1&pageSize=20`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "title": "Fondation",
      "author": "Isaac Asimov",
      "category": {
        "id": "guid",
        "name": "Science-Fiction"
      },
      "isbn": "978-0-330-21357-6",
      "availableCopies": 3,
      "totalCopies": 5,
      "coverUrl": "https://.../cover.jpg",
      "matchScore": 0.95
    }
  ],
  "totalCount": 15,
  "searchTerm": "asimov"
}
```

### `GET /api/books/{id}/stats`
**Manager, Admin** — Statistiques d'utilisation d'un livre

**Réponse (200) :**
```json
{
  "bookId": "guid",
  "title": "Fondation",
  "totalBorrows": 142,
  "currentActiveLoans": 3,
  "totalReservations": 89,
  "averageLoanDurationDays": 14.5,
  "popularityRank": 5,
  "lastBorrowed": "2025-04-04T00:00:00Z"
}
```

### `GET /api/books/categories/{categoryId}`
**Tous les rôles** — Livres par catégorie

**Query Params :** `?availableOnly=true&sortBy=title&sortOrder=asc&page=1&pageSize=20`

**Réponse (200) :** Même structure que GET /api/books

### `POST /api/books/{id}/copies`
**Manager, Admin** — Ajouter des exemplaires à un livre

**Payload :**
```json
{
  "quantity": 5,
  "reason": "Nouvelle acquisition"
}
```

**Réponse (200) :**
```json
{
  "message": "5 exemplaires ajoutés avec succès",
  "newTotalCopies": 10,
  "newAvailableCopies": 10
}
```

### `PUT /api/books/{id}/availability`
**Manager, Admin** — Modifier la disponibilité d'un livre

**Payload :**
```json
{
  "available": false,
  "reason": "Livre en réparation"
}
```

**Réponse (200) :**
```json
{
  "message": "Disponibilité mise à jour",
  "available": false,
  "reason": "Livre en réparation"
}
```

### `GET /api/books/recent`
**Tous les rôles** — Livres récemment ajoutés

**Query Params :** `?days=30&limit=10`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "title": "Dune Messiah",
      "author": "Frank Herbert",
      "category": {
        "id": "guid",
        "name": "Science-Fiction"
      },
      "isbn": "978-0-441-17272-6",
      "availableCopies": 2,
      "totalCopies": 2,
      "coverUrl": "https://.../dune2.jpg",
      "addedDate": "2025-04-01T00:00:00Z"
    }
  ]
}
```

### `GET /api/books/popular`
**Tous les rôles** — Livres populaires (plus empruntés)

**Query Params :** `?period=MONTH&limit=10`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "title": "Dune",
      "author": "Frank Herbert",
      "category": {
        "id": "guid",
        "name": "Science-Fiction"
      },
      "isbn": "978-0-441-17271-9",
      "availableCopies": 0,
      "totalCopies": 5,
      "coverUrl": "https://.../dune.jpg",
      "borrowCount": 42,
      "popularityRank": 1
    }
  ],
  "period": "MONTH",
  "totalBooks": 10
}
```

---

## 📥 3. Réservations

### `POST /api/reservations`
**User** — Réserver un livre

**Payload :**
```json
{
  "bookId": "guid-du-livre"
}
```

❗ Échoue si le livre est disponible — utiliser `/api/loans` à la place

**Réponse (201) :**
```json
{
  "id": "guid",
  "status": "PENDING",
  "reservationDate": "2025-04-05T10:00:00Z"
}
```

### `GET /api/reservations`
**User** — Mes réservations  
**Manager, Admin** — Toutes les réservations (avec filtres)

**Query Params :** `?status=PENDING&userId=guid&bookId=guid`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "book": {
        "id": "guid",
        "title": "Fondation"
      },
      "status": "AVAILABLE",
      "availableSince": "2025-04-05T09:00:00Z",
      "expiryDate": "2025-04-07T09:00:00Z"
    }
  ]
}
```

### `DELETE /api/reservations/{id}`
**User** — Annuler sa propre réservation (si statut != FULFILLED ou EXPIRED)  
**Manager, Admin** — Annuler n'importe quelle réservation

---

## 📤 4. Emprunts

### `GET /api/loans/overdue`
**Manager, Admin** — Liste des emprunts en retard

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "book": {
        "title": "Dune",
        "author": "Frank Herbert"
      },
      "user": {
        "id": "guid",
        "name": "Alice",
        "email": "alice@example.com"
      },
      "loanDate": "2025-04-01T00:00:00Z",
      "dueDate": "2025-04-15T00:00:00Z",
      "daysOverdue": 8
    }
  ],
  "totalOverdue": 15
}
```

### `PUT /api/loans/{id}/notify-overdue`
**Manager, Admin** — Envoyer une notification de retard

**Réponse (200) :**
```json
{
  "message": "Notification envoyée avec succès",
  "notificationId": "guid"
}
```

### `GET /api/loans/user/{userId}`
**Manager, Admin** — Historique des emprunts d'un utilisateur spécifique

**Query Params :** `?status=BORROWED&limit=20`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "book": {
        "title": "Fondation",
        "author": "Isaac Asimov"
      },
      "loanDate": "2025-03-15T00:00:00Z",
      "dueDate": "2025-03-29T00:00:00Z",
      "returnDate": "2025-03-28T00:00:00Z",
      "status": "RETURNED"
    }
  ],
  "totalBorrowed": 42,
  "currentlyBorrowed": 3
}
```

### `POST /api/loans`
**User** — Emprunter un livre

**Payload :**
```json
{
  "bookId": "guid-du-livre"
}
```

❗ Échoue si aucune copie disponible ou si réservé par un autre

**Réponse (201) :**
```json
{
  "id": "guid",
  "dueDate": "2025-04-19T00:00:00Z",
  "status": "BORROWED"
}
```

### `PUT /api/loans/{id}/return`
**User** — Déclarer le retour d'un livre  
**Manager, Admin** — Forcer le retour

**Réponse (200) :**
```json
{
  "message": "Livre retourné avec succès",
  "status": "RETURNED"
}
```

### `PUT /api/loans/{id}/extend`
**User** — Prolonger l'emprunt (1x max, si pas en retard)

### `GET /api/loans`
**User** — Mes emprunts  
**Manager, Admin** — Tous les emprunts

**Query Params :** `?status=BORROWED&overdueOnly=true&userId=guid`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "book": {
        "title": "Dune",
        "author": "Frank Herbert"
      },
      "loanDate": "2025-04-01T00:00:00Z",
      "dueDate": "2025-04-15T00:00:00Z",
      "returnDate": null,
      "status": "LATE"
    }
  ]
}
```

---

## 🔔 5. Notifications

### `GET /api/notifications`
**User, Manager, Admin** — Lister ses notifications

**Query Params :** `?read=false&type=REMINDER`

**Réponse (200) :**
```json
{
  "items": [
    {
      "id": "guid",
      "message": "Votre réservation pour 'Dune' est disponible jusqu'au 07/04.",
      "type": "REMINDER",
      "read": false,
      "createdAt": "2025-04-05T09:00:00Z"
    }
  ]
}
```

### `PUT /api/notifications/{id}/read`
**User, Manager, Admin** — Marquer comme lu

### `PUT /api/notifications/mark-all-read`
**User, Manager, Admin** — Tout marquer comme lu

---

## 🔔 5. Notifications

### `POST /api/notifications`
**Manager, Admin** — Créer une notification personnalisée

**Payload :**
```json
{
  "userId": "guid-utilisateur-cible",
  "message": "Votre livre réservé est maintenant disponible",
  "type": "RESERVATION_AVAILABLE",
  "priority": "HIGH"
}
```

**Réponse (201) :**
```json
{
  "id": "guid",
  "message": "Notification créée avec succès",
  "createdAt": "2025-04-05T10:00:00Z"
}
```

### `GET /api/notifications/unread-count`
**User, Manager, Admin** — Nombre de notifications non lues

**Réponse (200) :**
```json
{
  "count": 5,
  "highPriority": 2
}
```

---

## 📊 6. Statistiques & Dashboard

### `GET /api/analytics/categories/popular`
**Manager, Admin** — Catégories les plus populaires

**Query Params :** `?period=MONTH&limit=5`

**Réponse (200) :**
```json
[
  {
    "categoryId": "guid",
    "name": "Science-Fiction",
    "borrowCount": 156,
    "percentage": 35.2
  },
  {
    "categoryId": "guid",
    "name": "Fantasy",
    "borrowCount": 89,
    "percentage": 20.1
  }
]
```

### `GET /api/analytics/trends/borrows`
**Manager, Admin** — Tendances des emprunts par période

**Query Params :** `?period=MONTH&months=6`

**Réponse (200) :**
```json
{
  "period": "MONTH",
  "data": [
    {
      "month": "2025-03",
      "borrowCount": 142,
      "growth": 8.5
    },
    {
      "month": "2025-04",
      "borrowCount": 154,
      "growth": 12.3
    }
  ],
  "averageGrowth": 10.4
}
```

### `GET /api/analytics/reservations/stats`
**Manager, Admin** — Statistiques des réservations

**Réponse (200) :**
```json
{
  "totalReservations": 245,
  "pendingReservations": 22,
  "fulfilledReservations": 189,
  "expiredReservations": 34,
  "averageFulfillmentTimeHours": 48.5,
  "mostReservedBook": "Dune"
}
```

### `GET /api/analytics/books/top`
**Manager, Admin** — Livres les plus empruntés

**Query Params :** `?period=MONTH&limit=10`

**Réponse (200) :**
```json
[
  {
    "bookId": "guid",
    "title": "Dune",
    "borrowCount": 42
  },
  {
    "bookId": "guid",
    "title": "Fondation",
    "borrowCount": 38
  }
]
```

### `GET /api/analytics/users/active`
**Manager, Admin** — Utilisateurs les plus actifs

**Réponse (200) :**
```json
[
  {
    "userId": "guid",
    "name": "Alice",
    "loanCount": 15
  },
  {
    "userId": "guid",
    "name": "Bob",
    "loanCount": 12
  }
]
```

### `GET /api/analytics/overview`
**Manager, Admin** — Vue d'ensemble

**Réponse (200) :**
```json
{
  "totalBooks": 245,
  "availableBooks": 189,
  "activeLoans": 56,
  "overdueLoans": 8,
  "pendingReservations": 22,
  "activeUsersLast30Days": 142
}
```

---

## 🧾 7. Catégories

### `GET /api/categories`
**Tous les rôles** — Lister les catégories

**Réponse (200) :**
```json
[
  {
    "id": "guid",
    "name": "Science-Fiction",
    "icon": "bi-rocket"
  },
  {
    "id": "guid",
    "name": "Fantasy",
    "icon": "bi-magic"
  }
]
```

### `POST /api/categories`
**Manager, Admin** — Créer une catégorie

### `PUT /api/categories/{id}`
**Manager, Admin** — Modifier une catégorie

### `DELETE /api/categories/{id}`
**Admin uniquement** — Supprimer une catégorie (si non utilisée)

---

## 🕵️ 8. Audit

### `GET /api/audit/logs`
**Admin uniquement** — Journal des actions sensibles

**Query Params :** `?userId=guid&action=BOOK_DELETED&startDate=2025-04-01`

**Réponse (200) :**
```json
[
  {
    "id": "guid",
    "userName": "admin@lib.com",
    "action": "USER_ROLE_CHANGED",
    "entityName": "User",
    "entityId": "guid-target-user",
    "details": "{\"oldRole\":\"User\",\"newRole\":\"Manager\"}",
    "createdAt": "2025-04-05T08:30:00Z",
    "ipAddress": "192.168.1.10"
  }
]